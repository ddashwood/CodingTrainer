using CodingTrainer.CodingTrainerModels.Contexts;
using CodingTrainer.CodingTrainerModels.Models.Security;
using CodingTrainer.CodingTrainerModels.Repositories;
using CodingTrainer.CodingTrainerWeb.ApiControllers;
using CodingTrainer.CodingTrainerWeb.Models;
using CodingTrainer.CodingTrainerWeb.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CodingTrainer.CodingTrainerWeb.Controllers
{
    public class ExerciseController : Controller
    {
        ICodingTrainerRepository rep;
        ThemeController themeController;

        public ExerciseController(ICodingTrainerRepository repository, ThemeController themeController)
        {
            rep = repository;
            this.themeController = themeController;
        }

        [Authorize]
        public ActionResult Playground()
        {
            return View();
        }

        [Authorize]
        public ActionResult RunCode(int chapter, int exercise)
        {
            var syncContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null); // Can't run async from partial views without this

            var result = RunCodeAsync(chapter, exercise).Result;

            SynchronizationContext.SetSynchronizationContext(syncContext);
            return result;
        }

        private async Task<ActionResult> RunCodeAsync(int chapter, int exercise)
        {
            Task<string> themeTask = themeController.Get();

            var model = await rep.GetExerciseAsync(1, 1);
            string theme = await themeTask;

            ViewBag.Theme = CodeMirrorThemes.Themes.ConvertAll(t => new SelectListItem()
                    { Text = char.ToUpper(t[0]) + t.Substring(1), Value = t, Selected = t == theme });

            return PartialView(new ExerciseViewModel(model));
        }
    }
}