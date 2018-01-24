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
using CodingTrainer.CodingTrainerModels.Repositories;
using CodingTrainer.CodingTrainerModels.Models.Security;

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
        ICodingTrainerRepository rep;

        // Constructors
        public CodeRunnerHub(ICodeRunnerFactory runnerFactory, ICodingTrainerRepository repository)
        {
            this.runnerFactory = runnerFactory;
            rep = repository;
        }
        public CodeRunnerHub(ICodeRunnerFactory runnerFactory)
            :this(runnerFactory, new SqlCodingTrainerRepository())
        { }
        public CodeRunnerHub(ICodingTrainerRepository repository)
            :this(new CodeRunnerFactory(), repository)
        { }
        public CodeRunnerHub()
            : this(new CodeRunnerFactory() ,new SqlCodingTrainerRepository())
        { }


        public async Task Run(string code)
        {
            var user = rep.GetUser(Context);

            ICodeRunner runner = runnerFactory.GetCodeRunner();
            var connection = new Connection(Context.ConnectionId, runner, Clients.Caller, user);
            var runnerHandler = new ConsoleOut(connection);
            runner.ConsoleWrite += runnerHandler.OnConsoleWrite;
            connections[Context.ConnectionId] = connection;

            Task queueProcess = Task.Run(() => ProcessQueue(connection));

            if (user == null)
            {
                connection.ConsoleQueue.Add((QueueItemType.ConsoleOut, "Can't run code because you are not logged in"));
                connection.ConsoleQueue.Add((QueueItemType.Complete, null));
                return;
            }

            connection.ConsoleQueue.Add((QueueItemType.Run, code));

            await queueProcess;
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
            public ApplicationUser User { get; private set; }


            public Connection(string connectionId, ICodeRunner runner, ICodeRunnerHubClient caller, ApplicationUser user)
            {
                ConnectionID = connectionId;
                Runner = runner;
                Caller = caller;
                User = user;
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