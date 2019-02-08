using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerWeb.ActionFilters;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CodingTrainer.CodingTrainerWeb.Controllers
{
    public class SubmissionController : Controller, ISubmissionController
    {
        ICodingTrainerRepository rep;
        IUserServices userServices;

        public SubmissionController(ICodingTrainerRepository repository, IUserServices userServices)
        {
            rep = repository;
            this.userServices = userServices;
        }

        // For injection by custom filters
        public Submission Submission { private get; set; }

        // GET: Submission/Show
        [AuthorizeSubmission]
        public async Task<ActionResult> Show(int id)
        {
            if (Submission == null)
            {
                // Should never happen - should be set by the AuthorizeSubmission filter
                Submission = await rep.GetSubmissionAsync(id);
            }

            if (Submission == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActiveTheme = await userServices.GetCodeMirrorThemeAsync();
            return View(Submission);
        }
    }
}