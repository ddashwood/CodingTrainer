using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CodingTrainer.CodingTrainerWeb.Hubs
{
    public interface IHubContextRepository
    {
        string GetUserIdFromContext(HubCallerContext context);
    }
}