using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using CodingTrainer.CSharpRunner.CodeHost;
using System.Collections.Concurrent;
using Microsoft.CodeAnalysis.Scripting;
using CodingTrainer.CodingTrainerWeb.Hubs.Helpers;
using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerWeb.Dependencies;
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
        ICodeRunner runner;
        IUserServices userServices;
        ICodingTrainerRepository sqlRep;

        // Constructors
        public CodeRunnerHub(ICodeRunner runner, IUserServices userServices, ICodingTrainerRepository dbRepository)
        {
            this.userServices = userServices;
            sqlRep = dbRepository;
            this.runner = runner;
        }

        public async Task Run(string code)
        {
            try
            {
                string userId = userServices.GetCurrentUserId();

                var connectionTask = new ConnectionCodeRunner(runner);
                var connection = new Connection(Context.ConnectionId, connectionTask, Clients.Caller, userId);
                var runnerHandler = new ConsoleOut(connection);
                runner.ConsoleWrite += runnerHandler.OnConsoleWrite;
                connections[Context.ConnectionId] = connection;

                Task queueProcess = ProcessQueueAsync(connection);

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
                await LogErrorAsync(e, code);
                throw new HubException("An error occured when running your code", e);
            }
        }

        private async Task LogErrorAsync(Exception e, string code)
        {
            ExceptionLog log = new ExceptionLog
            {
                ExceptionText = e.ToString(),
                ExceptionDateTime = DateTimeOffset.Now,
                UserCode = $"<Not from user code>{Environment.NewLine}{code}"
            };

            log.UserId = userServices.GetCurrentUserId();
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

        private async Task ProcessQueueAsync(Connection connection)
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
                            Task t = RunCodeAsync(connection, message);
                            break;
                    }
                }
            });
        }

        private async Task RunCodeAsync(Connection connection, string message)
        {
            try
            {
                await connection.Runner.Run(message);
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
                if (connections[Context.ConnectionId].Runner is IConnectionTaskWithInput runner)
                {
                    runner.Input(message);
                }
            }
        }

        private interface IConnectionTask
        {
            Task Run(string message);
        }
        private interface IConnectionTaskWithInput : IConnectionTask
        {
            void Input(string message);
        }

        private class ConnectionCodeRunner : IConnectionTaskWithInput
        {
            private ICodeRunner runner;
            public ConnectionCodeRunner(ICodeRunner runner)
            {
                this.runner = runner;
            }

            public async Task Run(string message)
            {
                await runner.CompileAndRunAsync(message);
            }
            public void Input(string message)
            {
                runner.ConsoleIn(message);
            }
        }

        //private class ConnectionAssessmentRunner : IConnectionTask
        //{
        //    private AssessmentManager runner;
        //    int chapter;
        //    int exercise;
        //    public ConnectionAssessmentRunner(AssessmentManager runner, int chapter, int exercise)
        //    {
        //        this.runner = runner;
        //        this.chapter = chapter;
        //        this.exercise = exercise;
        //    }
        //    public async Task Run(string message)
        //    {
        //        await runner.RunAssessmentsForExercise(message, chapter, exercise);
        //    }
        //}

        private class Connection
        {
            public string ConnectionID { get; private set; }
            public IConnectionTask Runner { get; private set; }
            public ICodeRunnerHubClient Caller { get; private set; }
            public BufferBlock<(QueueItemType type, string message)> ConsoleQueue { get; private set; }
            public string UserId { get; private set; }


            public Connection(string connectionId, IConnectionTask runner, ICodeRunnerHubClient caller, string userId)
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