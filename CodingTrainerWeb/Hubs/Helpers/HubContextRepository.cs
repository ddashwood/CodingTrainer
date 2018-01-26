using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodingTrainer.CodingTrainerWeb.Hubs.Helpers
{
    class HubContextRepository : IHubContextRepository
    {

        public string GetUserIdFromContext(HubCallerContext context)
        {
            return context.User.Identity.GetUserId();
        }

    }
}