using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CodingTrainer.CSharpRunner.CodeHost;
using CodingTrainer.CodingTrainerWeb.Hubs.Helpers;
using System.Collections.Concurrent;
using System.Threading;

namespace CodingTrainer.CodingTrainerWeb.Hubs
{
    public class IdeHub : Hub<IIdeHubClient>, IIdeHubServer
    {
        private IIdeServices ideServices;
        public IdeHub(IIdeServices ideServices)
        {
            this.ideServices = ideServices;
        }
        public IdeHub()
            : this(new IdeServices())
        { }

        private static ConcurrentDictionary<string, CancellationTokenSource> inProgressDiags = new ConcurrentDictionary<string, CancellationTokenSource>();
        private static ConcurrentDictionary<string, CancellationTokenSource> inProgressCompletions = new ConcurrentDictionary<string, CancellationTokenSource>();

        public async Task RequestDiags(string code, int generation)
        {
            await DoCancellableAction(async (token) =>
            {
                var diags = await ideServices.GetDiagnostics(code, token);

                // If cancelled, then don't bother sending details back to the client
                token.ThrowIfCancellationRequested();

                Clients.Caller.DiagsCallback(diags == null ? null : CompilerError.ArrayFromDiagnostics(diags), generation);
            }, Context.ConnectionId, inProgressDiags);
        }

        public async Task RequestCompletions(string code, int cursorPosition, int generation)
        {
            await DoCancellableAction(async (token) =>
            {
                var completions = await ideServices.GetCompletions(code, cursorPosition, token);

                // If cancelled, then don't bother sending details back to the client
                token.ThrowIfCancellationRequested();

                Clients.Caller.CompletionsCallback(completions, generation);
            }, Context.ConnectionId, inProgressCompletions);
        }

        private static async Task DoCancellableAction(Func<CancellationToken, Task> action, string connectionId, ConcurrentDictionary<string, CancellationTokenSource> inProgressTokenSources)
        {
            if (inProgressTokenSources.TryRemove(connectionId, out var previousTokenSource))
            {
                previousTokenSource.Cancel();
            }

            CancellationTokenSource cts = new CancellationTokenSource();
            inProgressTokenSources[connectionId] = cts;
            await action(cts.Token);
        }
    }
}