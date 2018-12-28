using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerModels.Security;
using CodingTrainer.CodingTrainerWeb.AspNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CodingTrainer.CodingTrainerWeb.ActionFilters
{
    public class AuthorizeExerciseAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
            {
                // Not logged on
                return false;
            }

            object oChapter = httpContext.Request.RequestContext.RouteData.Values["chapter"];
            object oExercise = httpContext.Request.RequestContext.RouteData.Values["exercise"];

            if (oChapter == null || oExercise == null)
            {
                throw new InvalidOperationException("No Exercise details found in route parameters");
            }

            UserRepository userRepository = new UserRepository();
            ApplicationUser user = userRepository.GetCurrentUser();

            int chapter = Convert.ToInt32(oChapter);
            int exercise = Convert.ToInt32(oExercise);

            return user.ExercisePermitted(chapter, exercise);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // Not logged in, send to normal login page
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                // Logged in, but no permission to access this exercise

                // This line removed because the message body of 401 responses get
                // overwritten by Azure. Will put this line back in once the Azure issue is
                // fixed

                // filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                filterContext.Result = new ViewResult() { ViewName = "~/Views/Exercise/AccessDenied.cshtml" };
            }
        }
    }
}