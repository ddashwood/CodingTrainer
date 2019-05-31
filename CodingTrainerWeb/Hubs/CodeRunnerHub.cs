using CodingTrainer.CSharpRunner.CodeHost;
using CodingTrainer.CodingTrainerWeb.Hubs.Helpers;
using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using Microsoft.AspNet.SignalR;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace CodingTrainer.CodingTrainerWeb.Hubs
{
    public partial class CodeRunnerHub : Hub<ICodeRunnerHubClient>, ICodeRunnerHubServer
    {
        // Static fields

        // All current connections
        // The key is the Hub Context's connection id
        private static ConcurrentDictionary<string, Connection> connections = new ConcurrentDictionary<string, Connection>();

        // Instance fields - dependencies
        private readonly ICodeRunner runner;
        private readonly IUserServices userServices;
        private readonly ICodingTrainerRepository sqlRep;

        // Constructors
        public CodeRunnerHub(ICodeRunner runner, IUserServices userServices, ICodingTrainerRepository dbRepository)
        {
            this.userServices = userServices;
            sqlRep = dbRepository;
            this.runner = runner;
        }

        // Enums

        public enum QueueItemType
        {
            ConsoleOut,
            ConsoleOutHighlight,
            Complete,
            AssessmentComplete
        }

        // Methods

        public async Task Run(string code)
        {
            var task = new ConnectionCodeRunner(runner);
            await StartTaskIfLoggedOn(task, code);
        }

        public async Task Assess(string code, int chapter, int exercise)
        {
            var task = new ConnectionAssessmentRunner(runner, userServices, sqlRep, chapter, exercise);
            await StartTaskIfLoggedOn(task, code);
        }

        public void ConsoleIn(string message)
        {
            if (connections.ContainsKey(Context.ConnectionId))
            {
                connections[Context.ConnectionId].ConsoleIn(message);
            }
        }

        private async Task StartTaskIfLoggedOn(IConnectionTask task, string code)
        {
            try
            {
                string userId = userServices.GetCurrentUserId();

                var connection = new Connection(Context.ConnectionId, task, Clients.Caller, userId);
                connections[Context.ConnectionId] = connection;

                Task queueProcess = connection.ProcessQueueAsync();

                if (userId == null)
                {
                    connection.PostToQueue((QueueItemType.ConsoleOut, "Can't run code because you are not logged in"));
                    connection.PostToQueue((QueueItemType.Complete, null));
                }
                else
                {
                    await RunTaskAsync(connection, code);
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
            ExceptionRunningUsersCode log = new ExceptionRunningUsersCode
            {
                ExceptionText = e.ToString(),
                ExceptionDateTime = DateTimeOffset.UtcNow,
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


        private async Task RunTaskAsync(Connection connection, string message)
        {
            try
            {
                await connection.RunAsync(message);
            }
            catch (CompilationErrorException ex)
            {
                Clients.Caller.CompilerError(CompilerError.ArrayFromException(ex));
            }
            catch (ExceptionRunningUserCodeException e)
            {
                Clients.Caller.ConsoleOut("Error: " + e.Message + Environment.NewLine);
                Clients.Caller.ConsoleOut(e.StackTrace);
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
                connection.PostToQueue((QueueItemType.Complete, null));
            }
        }
    }
}