using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerWeb.ApiControllers;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using CodingTrainer.CodingTrainerWeb.Models;
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

        public async Task<ActionResult> Exercise(int chapter, int exercise)
        {
            return View(await rep.GetExerciseAsync(chapter, exercise));
        }

        [ChildActionOnly]
        public ActionResult ExerciseSidebar()
        {
            async Task<ActionResult> ExerciseSidebarAsync()
            {
                return PartialView(await rep.GetAllChaptersAsync());
            }

            return RunWithoutSyncContext(() => ExerciseSidebarAsync());
        }

        [Authorize]
        [ChildActionOnly]
        public ActionResult RunCode(Exercise exercise)
        {
            async Task<ActionResult> RunCodeAsync(Exercise _exercise)
            {
                string activeTheme = await themeController.Get();

                ViewBag.Theme = CodeMirrorThemes.Themes.ConvertAll(t => new SelectListItem()
                    { Text = char.ToUpper(t[0]) + t.Substring(1), Value = t, Selected = t == activeTheme });

                return PartialView(_exercise);
            }

            return RunWithoutSyncContext(() => RunCodeAsync(exercise));
        }

        [Authorize]
        [ChildActionOnly]
        public ActionResult RunCodeById(int chapter, int exercise)
        {
            async Task<ActionResult> RunCodeAsync(int _chapter, int _exercise)
            {
                Task<string> themeTask = themeController.Get();
                Task<Exercise> exerciseTask = rep.GetExerciseAsync(_chapter, _exercise);

                await Task.WhenAll(themeTask, exerciseTask);

                string activeTheme = themeTask.Result;
                Exercise model = exerciseTask.Result;

                ViewBag.Theme = CodeMirrorThemes.Themes.ConvertAll(t => new SelectListItem()
                    { Text = char.ToUpper(t[0]) + t.Substring(1), Value = t, Selected = t == activeTheme });

                return PartialView("RunCode", model);
            }

            return RunWithoutSyncContext(() => RunCodeAsync(chapter, exercise));
        }

        // Helpers

        private ActionResult RunWithoutSyncContext(Func<Task<ActionResult>> task)
        {
            var syncContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null); // Can't run async from partial views without this

            var result = task().Result;

            SynchronizationContext.SetSynchronizationContext(syncContext);
            return result;
        }
    }
}