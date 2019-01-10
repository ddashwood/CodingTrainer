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
            private readonly ConnectionConsoleOut consoleOut;

            public Connection(string connectionId, IConnectionTask connectionTask, ICodeRunnerHubClient caller, string userId)
            {
                this.connectionId = connectionId;
                this.connectionTask = connectionTask;
                this.caller = caller;
                this.userId = userId;

                connectionQueue = new BufferBlock<(QueueItemType type, string message)>();
                consoleOut = new ConnectionConsoleOut(this);
            }

            public async Task RunAsync(string message)
            {
                await connectionTask.RunAsync(consoleOut, message);
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
            }

            public async Task ProcessQueueAsync()
            {
                // Run in a different thread from the thread pool

                // The ProcessQueueAsync method should be started before
                // the task is run. It will return immediately due to
                // being run on a differen thread. Then the task can be 
                // started, and any items posted to the queue will be
                // handled by the following code:

                await Task.Run(async () =>
                {
                    bool complete = false;

                    while (!complete)
                    {
                        var (type, message) = await connectionQueue.ReceiveAsync();

                        switch (type)
                        {
                            case QueueItemType.Complete:
                                caller.Complete();
                                connections.TryRemove(connectionId, out var ignore);
                                complete = true;
                                break;
                            case QueueItemType.ConsoleOut:
                                caller.ConsoleOut(message);
                                break;
                        }
                    }
                });
            }
        }

        private class ConnectionConsoleOut
        {
            Connection connection;

            public ConnectionConsoleOut(Connection connection)
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
        }

    }
}