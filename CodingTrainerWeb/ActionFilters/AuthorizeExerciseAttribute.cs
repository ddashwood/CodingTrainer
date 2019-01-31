using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerModels.Security;
using CodingTrainer.CodingTrainerWeb.Users;
using CodingTrainer.CodingTrainerWeb.Dependencies;
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
        public static IUserServices UserServices { get; set; }
        public static ICodingTrainerRepository DbRepository { get; set; }
        private int chapter;
        private int exercise;

        public AuthorizeExerciseAttribute()
        {
            if (UserServices == null)
                throw new InvalidOperationException("The User Services have not been set");
            if (DbRepository == null)
                throw new InvalidOperationException("The Database Repository has not been set");
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
            {
                // Not logged on
                return false;
            }

            // Retrieve GET parameters
            object oChapter = httpContext.Request.RequestContext.RouteData.Values["chapter"];
            object oExercise = httpContext.Request.RequestContext.RouteData.Values["exercise"];

            // Retrieve POST parameters
            oChapter = oChapter ?? httpContext.Request.Params["chapter"];
            oExercise = oExercise ?? httpContext.Request.Params["exercise"];

            if (oChapter == null || oExercise == null)
            {
                throw new InvalidOperationException("No Exercise details found in route parameters");
            }
            chapter = Convert.ToInt32(oChapter);
            exercise = Convert.ToInt32(oExercise);

            ApplicationUser user = UserServices.GetCurrentUser();
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

                // If the exercise doesn't even exist, we'd prefer the user to see a 404
                if (DbRepository.GetExercise(chapter, exercise) == null)
                {
                    throw new HttpException(404, "Not found");
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    filterContext.Result = new ViewResult() { ViewName = "~/Views/Exercise/AccessDenied.cshtml" };
                }
            }
        }
    }
}