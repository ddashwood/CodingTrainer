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
        public async Task<ActionResult> RunCode()
        {
            Task<string> themeTask = themeController.Get();

            var exercise = await rep.GetExerciseAsync(1, 1);
            string theme = await themeTask;

            ViewBag.Theme = CodeMirrorThemes.Themes.ConvertAll(t => new SelectListItem()
                    { Text = char.ToUpper(t[0]) + t.Substring(1), Value = t, Selected = t == theme });

            return View(new ExerciseViewModel(exercise));
        }
    }
}