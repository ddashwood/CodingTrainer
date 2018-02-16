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

using CodingTrainer.CodingTrainerWeb.Hubs.Helpers;
using CodingTrainer.CodingTrainerModels.Models;
using CodingTrainer.CodingTrainerModels.Repositories;
using System.Threading.Tasks.Dataflow;

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
        public CodeRunnerHub(ICodeRunnerFactory runnerFactory = null, IHubContextRepository hubRepository = null, ICodingTrainerRepository dbRepository = null)
        {
            hubRep = hubRepository ?? new HubContextRepository();
            sqlRep = dbRepository ?? new SqlCodingTrainerRepository();
            this.runnerFactory = runnerFactory ?? new CodeRunnerWithLoggerFactory(hubRep, sqlRep);
        }
        public CodeRunnerHub()
            : this(null, null, null)
        { }

        public async Task Run(string code)
        {
            try
            {
                string userId = hubRep.GetUserIdFromContext(Context);

                if (runnerFactory is IRequiresContext contextFactory)
                    contextFactory.Context = Context;

                ICodeRunner runner = runnerFactory.GetCodeRunner();
                var connection = new Connection(Context.ConnectionId, runner, Clients.Caller, userId);
                var runnerHandler = new ConsoleOut(connection);
                runner.ConsoleWrite += runnerHandler.OnConsoleWrite;
                connections[Context.ConnectionId] = connection;

                Task queueProcess = ProcessQueue(connection);

                if (userId == null)
                {
                    connection.ConsoleQueue.Post((QueueItemType.ConsoleOut, "Can't run code because you are not logged in"));
                    connection.ConsoleQueue.Post((QueueItemType.Complete, null));
                }
                else
                {
                    connection.ConsoleQueue.Post((QueueItemType.Run, code));
                }

                await queueProcess;
            }
            catch (Exception e)
            {
                await LogError(e, code);
                throw new HubException("An error occured when running your code", e);
            }
        }

        private async Task LogError(Exception e, string code)
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
                await sqlRep.InsertExceptionLogAsync(log);
            }
            catch
            {
                // Ignore exceptions - no way of logging them if the logger fails, but want
                // to allow program flow to continue anyway in order to show message to user
            }
        }

        private async Task ProcessQueue(Connection connection)
        {

            await Task.Run(async () =>
            {
                bool complete = false;

                while (!complete)
                {
                    var (type, message) = await connection.ConsoleQueue.ReceiveAsync();

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
                            // t is there to avoid compiler warnings, not actually used
                            Task t = RunCode(connection, message);
                            break;
                    }
                }
            });
        }

        private async Task RunCode(Connection connection, string message)
        {
            try
            {
                await connection.Runner.RunCode(message);
            }
            catch (CompilationErrorException ex)
            {
                Clients.Caller.CompilerError(CompilerError.ArrayFromException(ex));
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
                connection.ConsoleQueue.Post((QueueItemType.Complete, null));
            }
        }

        public void ConsoleIn(string message)
        {
            if (connections.ContainsKey(Context.ConnectionId))
            {
                ICodeRunner runner = connections[Context.ConnectionId].Runner;
                runner.ConsoleIn(message);
            }
        }

        private class Connection
        {
            public string ConnectionID { get; private set; }
            public ICodeRunner Runner { get; private set; }
            public ICodeRunnerHubClient Caller { get; private set; }
            public BufferBlock<(QueueItemType type, string message)> ConsoleQueue { get; private set; }
            public string UserId { get; private set; }


            public Connection(string connectionId, ICodeRunner runner, ICodeRunnerHubClient caller, string userId)
            {
                ConnectionID = connectionId;
                Runner = runner;
                Caller = caller;
                UserId = userId;
                ConsoleQueue = new BufferBlock<(QueueItemType type, string message)>();
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
                    connection.ConsoleQueue.Post((QueueItemType.ConsoleOut, e.Message));
                }
            }
        }

    }
}