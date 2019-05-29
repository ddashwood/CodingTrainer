using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Web;

namespace CodingTrainer.CodingTrainerWeb.Hubs
{
    public partial class CodeRunnerHub
    {
        private class Connection
        {
            private readonly string connectionId;
            private readonly IConnectionTask connectionTask;
            private readonly string userId;
            private readonly ICodeRunnerHubClient caller;

            private readonly BufferBlock<(QueueItemType type, string message)> connectionQueue;
            private readonly ConnectionOutput output;

            public Connection(string connectionId, IConnectionTask connectionTask, ICodeRunnerHubClient caller, string userId)
            {
                this.connectionId = connectionId;
                this.connectionTask = connectionTask;
                this.caller = caller;
                this.userId = userId;

                connectionQueue = new BufferBlock<(QueueItemType type, string message)>();
                output = new ConnectionOutput(this);
            }

            public async Task RunAsync(string message)
            {
                await connectionTask.RunAsync(output, message);
            }

            public void ConsoleIn(string message)
            {
                if (connectionTask is IConnectionTaskWithInput taskWithInput)
                {
                    taskWithInput.Input(message);
                }
            }

            public void PostToQueue((QueueItemType type, string message) data)
            {
                connectionQueue.Post(data);
                if (data.type == QueueItemType.Complete)
                    connectionQueue.Complete();
            }

            public async Task ProcessQueueAsync()
            {
                // Run in a different thread from the thread pool

                // The ProcessQueueAsync method should be started before
                // the task is run. It will return immediately. Then the task
                // can be started, and any items posted to the queue will be
                // handled by the following code:

                while (await connectionQueue.OutputAvailableAsync())
                {
                    var (type, message) = connectionQueue.Receive();

                    switch (type)
                    {
                        case QueueItemType.Complete:
                            caller.Complete();
                            connections.TryRemove(connectionId, out var ignore);
                            break;
                        case QueueItemType.ConsoleOut:
                            caller.ConsoleOut(message);
                            break;
                        case QueueItemType.AssessmentComplete:
                            caller.AssessmentComplete(message == null ? false : true);
                            break;
                    }
                }
            }
        }

        private class ConnectionOutput
        {
            Connection connection;

            public ConnectionOutput(Connection connection)
            {
                this.connection = connection;
            }

            public void OnConsoleWrite(object sender, ConsoleWriteEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.Message))
                {
                    connection.PostToQueue((QueueItemType.ConsoleOut, e.Message));
                }
            }

            public void AssessmentComplete(bool success)
            {
                connection.PostToQueue((QueueItemType.AssessmentComplete, success ? "Success" : null));
            }
        }

    }
}