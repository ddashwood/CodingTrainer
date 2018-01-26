using CodingTrainer.CSharpRunner.CodeHost;
using CodingTrainer.CSharpRunner.CodeHost.Factories;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingTrainer.CodingTrainerWeb.Hubs.Helpers
{
    public class CodeRunnerWithLoggerFactory : ICodeRunnerFactory, IRequiresContext
    {
        IHubContextRepository rep;

        public CodeRunnerWithLoggerFactory(IHubContextRepository rep)
        {
            this.rep = rep;
        }

        public HubCallerContext Context { get; set; }

        public ICodeRunner GetCodeRunner()
        {
            if (Context == null)
                return new CodeRunner();

            return new CodeRunner(new CodeRunnerLogger(rep.GetUserIdFromContext(Context)));
        }
    }
}