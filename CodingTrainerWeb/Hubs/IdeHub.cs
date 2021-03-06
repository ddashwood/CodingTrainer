﻿using Microsoft.AspNet.SignalR;
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

        private static readonly ConcurrentDictionary<string, CancellationTokenSource> inProgressDiags = new ConcurrentDictionary<string, CancellationTokenSource>();
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> inProgressCompletions = new ConcurrentDictionary<string, CancellationTokenSource>();
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> inProgressParams = new ConcurrentDictionary<string, CancellationTokenSource>();

        public async Task RequestDiags(string code, int generation)
        {
            await DoCancellableAction(async (token) =>
            {
                var diags = await ideServices.GetDiagnosticsAsyc(code, token);

                // If cancelled, then don't bother sending details back to the client
                token.ThrowIfCancellationRequested();

                Clients.Caller.DiagsCallback(diags == null ? null : CompilerError.ArrayFromDiagnostics(diags), generation);
            }, Context.ConnectionId, inProgressDiags);
        }

        public async Task RequestCompletions(string code, int cursorPosition, int tokenStart)
        {
            await DoCancellableAction(async (token) =>
            {
                var completions = await ideServices.GetCompletionStringsAsync(code, cursorPosition, token);

                // If cancelled, then don't bother sending details back to the client
                token.ThrowIfCancellationRequested();

                Clients.Caller.CompletionsCallback(completions, tokenStart);
            }, Context.ConnectionId, inProgressCompletions);
        }

        public async Task RequestParameters(string code, int cursorPosition, int tokenStart)
        {
            await DoCancellableAction(async (token) =>
            {
                var paramsRaw = await ideServices.GetOverloadsAndParametersAsync(code, cursorPosition, token);

                // If cancelled, then don't bother sending details back to the client
                token.ThrowIfCancellationRequested();

                if (paramsRaw != null) // Don't bother calling back if there is no parameter list to show
                {
                    var overloads = new Overloads(paramsRaw);
                    Clients.Caller.ParamsCallback(overloads, tokenStart);
                }

            }, Context.ConnectionId, inProgressParams);
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