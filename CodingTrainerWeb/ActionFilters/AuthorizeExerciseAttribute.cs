﻿using CodingTrainer.CodingTrainerModels;
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
    public class AuthorizeExerciseAttribute : AuthorizeAttribute
    {
        public static ICodingTrainerRepository DbRepository { get; set; }
        private int chapter;
        private int exercise;

        public AuthorizeExerciseAttribute()
        {
            if (DbRepository == null)
                throw new InvalidOperationException("The Database Repository has not been set");

            //userServices = new UserServices();
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

            ValueProviderResult oChapter = filterContext.Controller.ValueProvider.GetValue("chapter");
            ValueProviderResult oExercise = filterContext.Controller.ValueProvider.GetValue("exercise");

            if (oChapter == null || oExercise == null)
            {
                throw new InvalidOperationException("No Exercise details found in route parameters");
            }
            chapter = Convert.ToInt32(oChapter.AttemptedValue);
            exercise = Convert.ToInt32(oExercise.AttemptedValue);

            ApplicationUser user = userServices.GetCurrentUser();
            if (!user.ExercisePermitted(chapter, exercise))
            {
                HandleUnauthorizedRequest(filterContext);
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