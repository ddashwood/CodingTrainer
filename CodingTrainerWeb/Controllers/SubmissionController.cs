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
    public class SubmissionController : Controller
    {
        ICodingTrainerRepository rep;
        IUserServices userServices;

        public SubmissionController(ICodingTrainerRepository repository, IUserServices userServices)
        {
            rep = repository;
            this.userServices = userServices;
        }

        // GET: Submission/Show
        [AuthorizeSubmission]
        public async Task<ActionResult> Show(int id)
        {
            var submission = await rep.GetSubmissionAsync(id);
            if (submission == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActiveTheme = await userServices.GetCodeMirrorThemeAsync();
            return View(submission);
        }
    }
}