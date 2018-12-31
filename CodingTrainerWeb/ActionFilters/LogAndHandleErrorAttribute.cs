using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using Microsoft.AspNet.Identity;

namespace CodingTrainer.CodingTrainerWeb.ActionFilters
{
    public class LogAndHandleErrorAttribute : HandleErrorAttribute
    {
        public static ICodingTrainerRepository Repository { get; set; }

        public override void OnException(ExceptionContext filterContext)
        {
            Exception loggingException = null;

            try
            {
                var innerText = new StringBuilder();
                string body = null;
                try
                {
                    var e = filterContext.Exception?.InnerException;
                    while (e != null)
                    {
                        innerText.Append(e.Message + Environment.NewLine);
                        e = e.InnerException;
                    }
                }
                catch { } // Ignore errors

                try
                {
                    using (Stream receiveStream = filterContext.HttpContext.Request.InputStream)
                    {
                        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                        {
                            receiveStream.Seek(0, SeekOrigin.Begin);
                            body = readStream.ReadToEnd();
                        }
                    }
                }
                catch { } // Ignore errors

                var exception = new UnhandledControllerException()
                {
                    Message = filterContext.Exception.Message,
                    InnerExceptionMessages = innerText.ToString(),
                    StackTrace = filterContext.Exception.StackTrace,
                    Url = filterContext.HttpContext.Request.Url.ToString(),
                    Body = body,
                    UserId = filterContext.HttpContext.User.Identity.GetUserId(),
                    DateTime = DateTime.Now
                };

                Repository.InsertUnhandledControllerException(exception);
            }
            catch (Exception e)
            {
                loggingException = e;
            }

            base.OnException(filterContext);

            if (filterContext.Result is ViewResult viewResult)
            {
                viewResult.ViewBag.LoggingException = loggingException;
            }
        }
    }
}