using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace CodingTrainer.CodingTrainerWeb.ActionFilters
{
    public class LogAndHandleWebApiError: ExceptionHandler
    {
        public static ICodingTrainerRepository Repository { get; set; }

        public async override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var innerText = new StringBuilder();
            string body = null;
            try
            {
                var e = context.Exception?.InnerException;
                while (e != null)
                {
                    innerText.Append(e.Message + Environment.NewLine);
                    e = e.InnerException;
                }
            }
            catch { } // Ignore errors

            try
            {
                HttpContext.Current.Request.InputStream.Position = 0;
                using (var reader = new StreamReader(HttpContext.Current.Request.InputStream, System.Text.Encoding.UTF8, true, 4096, true))
                {
                    body = await reader.ReadToEndAsync();
                }
                HttpContext.Current.Request.InputStream.Position = 0;
            }
            catch { } // Ignore errors

            var exception = new UnhandledControllerException()
            {
                Message = context.Exception.Message,
                InnerExceptionMessages = innerText.ToString(),
                StackTrace = context.Exception.StackTrace,
                Url = context.Request.RequestUri.ToString(),
                Body = body,
                UserId = context.RequestContext.Principal.Identity.GetUserId(),
                DateTime = DateTime.Now
            };

            Repository.InsertUnhandledControllerException(exception);

            await base.HandleAsync(context, cancellationToken);
        }
    }
}