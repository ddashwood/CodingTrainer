using CodingTrainer.CodingTrainerModels.Repositories;
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
        IHubContextRepository hubRep;
        ICodingTrainerRepository dbRep;

        public CodeRunnerWithLoggerFactory(IHubContextRepository hubRep, ICodingTrainerRepository dbRep)
        {
            this.hubRep = hubRep;
            this.dbRep = dbRep;
        }

        public HubCallerContext Context { get; set; }

        public ICodeRunner GetCodeRunner()
        {
            if (Context == null)
                return new CodeRunner();

            return new CodeRunner(new CodeRunnerLogger(hubRep.GetUserIdFromContext(Context), dbRep));
        }
    }
}