using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using CodingTrainer.CSharpRunner.CodeHost;
using System.Collections.Concurrent;
using Microsoft.CodeAnalysis.Scripting;
using CodingTrainer.CSharpRunner.CodeHost.Factories;
using CodingTrainer.CodingTrainerModels.Models.Security;
using Microsoft.AspNet.Identity;
using System.Security.Principal;
using CodingTrainer.CodingTrainerWeb.Hubs.Helpers;
using CodingTrainer.CodingTrainerModels.Models;
using CodingTrainer.CodingTrainerModels.Repositories;

namespace CodingTrainer.CodingTrainerWeb.Hubs
{
    public class CodeRunnerHub : Hub<ICodeRunnerHubClient>, ICodeRunnerHubServer
    {
        private enum QueueItemType
        {
            ConsoleOut,
            Run,
            Complete
        }

        // Static fields

        // All current connections
        static ConcurrentDictionary<string, Connection> connections = new ConcurrentDictionary<string, Connection>();

        // Instance fields

        // Dependencies
        ICodeRunnerFactory runnerFactory;
        IHubContextRepository hubRep;
        ICodingTrainerRepository sqlRep;

        // Constructors
        public CodeRunnerHub(ICodeRunnerFactory runnerFactory=null, IHubContextRepository hubRepository = null, ICodingTrainerRepository dbRepository = null)
        {
            hubRep = hubRepository ?? new HubContextRepository();
            sqlRep = dbRepository ?? new SqlCodingTrainerRepository();
            this.runnerFactory = runnerFactory ?? new CodeRunnerWithLoggerFactory(hubRep, sqlRep);
        }
        public CodeRunnerHub()
            :this(null, null, null)
        { }

        public async Task Run(string code)
        {
            try
            {
                string userId = hubRep.GetUserIdFromContext(Context);

                IRequiresContext contextFactory = runnerFactory as IRequiresContext;
                if (contextFactory != null)
                    contextFactory.Context = Context;

                ICodeRunner runner = runnerFactory.GetCodeRunner();
                var connection = new Connection(Context.ConnectionId, runner, Clients.Caller, userId);
                var runnerHandler = new ConsoleOut(connection);
                runner.ConsoleWrite += runnerHandler.OnConsoleWrite;
                connections[Context.ConnectionId] = connection;

                Task queueProcess = Task.Run(() => ProcessQueue(connection));

                if (userId == null)
                {
                    connection.ConsoleQueue.Add((QueueItemType.ConsoleOut, "Can't run code because you are not logged in"));
                    connection.ConsoleQueue.Add((QueueItemType.Complete, null));
                }
                else
                {
                    connection.ConsoleQueue.Add((QueueItemType.Run, code));
                }

                await queueProcess;
            }
            catch (Exception e)
            {
                LogError(e, code);
                throw new HubException("An error occured when running your code", e);
            }
        }

        private void LogError(Exception e, string code)
        {
            ExceptionLog log = new ExceptionLog
            {
                ExceptionText = e.ToString(),
                ExceptionDateTime = DateTimeOffset.Now,
                UserCode = $"<Not from user code>{Environment.NewLine}{code}"
            };

            if (Context != null) log.UserId = hubRep?.GetUserIdFromContext(Context);
            try
            {
                // Log to database
                sqlRep.InsertExceptionLog(log);
            }
            catch
            {
                // Ignore exceptions - no way of logging them if the logger fails, but want
                // to allow program flow to continue anyway in order to show message to user
            }
        }

        private void ProcessQueue(Connection connection)
        {
            bool complete = false;

            while (!complete)
            {
                var (type, message) = connection.ConsoleQueue.Take();

                switch (type)
                {
                    case QueueItemType.Complete:
                        connection.Caller.Complete();
                        connections.TryRemove(connection.ConnectionID, out var ignore);
                        complete = true;
                        break;
                    case QueueItemType.ConsoleOut:
                        connection.Caller.ConsoleOut(message);
                        break;
                    case QueueItemType.Run:
                        RunCode(connection, message);
                        break;
                }
            }
        }

        private void RunCode(Connection connection, string message)
        {
            Task.Run(() =>
            {
                try
                {
                    connection.Runner.RunCode(message);
                }
                catch (CompilationErrorException ex)
                {
                    Clients.Caller.ConsoleOut("Compiler error: " + Environment.NewLine + Environment.NewLine);
                    foreach (var error in ex.Diagnostics.OrderBy(e => e.Location.SourceSpan.Start))
                    {
                        Clients.Caller.ConsoleOut("  " + error.GetMessage() + Environment.NewLine);
                        Clients.Caller.ConsoleOut("    " + error.Location + Environment.NewLine);
                    }
                }
                catch (Exception e)
                {
                    Clients.Caller.ConsoleOut("Error: " + e.Message + Environment.NewLine);

                    if (e.InnerException != null)
                    {
                        Clients.Caller.ConsoleOut(e.InnerException.Message);
                    }
                }
                finally
                {
                    connection.ConsoleQueue.Add((QueueItemType.Complete, null));
                }
            });
        }

        public void ConsoleIn(string message)
        {
            ICodeRunner runner = connections[Context.ConnectionId].Runner;
            runner.ConsoleIn(message);
        }

        private class Connection
        {
            public string ConnectionID { get; private set; }
            public ICodeRunner Runner { get; private set; }
            public ICodeRunnerHubClient Caller { get; private set; }
            public BlockingCollection<(QueueItemType type, string message)> ConsoleQueue { get; private set; }
            public string UserId { get; private set; }


            public Connection(string connectionId, ICodeRunner runner, ICodeRunnerHubClient caller, string userId)
            {
                ConnectionID = connectionId;
                Runner = runner;
                Caller = caller;
                UserId = userId;
                ConsoleQueue = new BlockingCollection<(QueueItemType type, string message)>();
            }
        }

        private class ConsoleOut
        {
            Connection connection;

            public ConsoleOut(Connection connection)
            {
                this.connection = connection;
            }

            public void OnConsoleWrite(object sender, ConsoleWriteEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.Message))
                {
                    connection.ConsoleQueue.Add((QueueItemType.ConsoleOut, e.Message));
                }
            }
        }

    }
}