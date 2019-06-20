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
using Unity;

namespace CodingTrainer.CodingTrainerWeb.ActionFilters
{
    public class AuthorizeSubmissionAttribute : AuthorizeAttribute
    {
        public static ICodingTrainerRepository DbRepository { get; set; }
        private Submission submission;

        public AuthorizeSubmissionAttribute()
        {
            if (DbRepository == null)
                throw new InvalidOperationException("The Database Repository has not been set");
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result != null)
            {
                // Not logged in
                return;
            }

            // Filters get reused, but if we reuse the same user service, old data will be cached
            IUserServices userServices = UnityConfig.Container.Resolve<IUserServices>();

            ValueProviderResult oId = filterContext.Controller.ValueProvider.GetValue("id");

            if (oId == null)
            {
                throw new InvalidOperationException("No Submission ID found in route parameters");
            }
            var id = Convert.ToInt32(oId.AttemptedValue);
            submission = DbRepository.GetSubmission(id);

            if (submission == null || submission.UserId != userServices.GetCurrentUserId())
            {
                HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                if (filterContext.Controller is ISubmissionController controller)
                {
                    controller.Submission = submission;
                }
            }
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
                // Logged in, but no permission to access this submission

                // If the submission doesn't even exist, we'd prefer the user to see a 404
                if (submission == null)
                {
                    throw new HttpException(404, "Not found");
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    filterContext.Result = new ViewResult() { ViewName = "~/Views/Submission/AccessDenied.cshtml" };
                }
            }
        }
    }
}