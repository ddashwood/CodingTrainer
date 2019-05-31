using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerWeb.ActionFilters;
using CodingTrainer.CodingTrainerWeb.ApiControllers;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using CodingTrainer.CodingTrainerWeb.Models;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CodingTrainer.CodingTrainerWeb.Controllers
{
    public class ExerciseController : Controller
    {
        ICodingTrainerRepository rep;
        IUserServices userServices;

        public ExerciseController(ICodingTrainerRepository repository, IUserServices userServices)
        {
            rep = repository;
            this.userServices = userServices;
        }

        [Authorize]
        public ActionResult Playground()
        {
            return View();
        }

        [AuthorizeExercise]
        public async Task<ActionResult> Exercise(int chapter, int exercise)
        {
            var exerciseDetails = await rep.GetExerciseAsync(chapter, exercise);
            if (exerciseDetails == null) throw new HttpException(404, "Not found");
            ViewBag.ActiveTheme = await userServices.GetCodeMirrorThemeAsync();
            if (exerciseDetails.IsAssessed)
            {
                return View(exerciseDetails);
            }
            else
            {
                return View("ExerciseNotAssessed", exerciseDetails);
            }
        }

        [Authorize]
        public async Task<ActionResult> Next(bool advance = true)
        {
            var fromUri = Request.UrlReferrer;
            int chapter;
            int exercise;
            
            if (fromUri != null &&
                fromUri.Segments.Length == 4 && 
                RemoveTrailingSlash(fromUri.Segments[0]) == "" &&
                RemoveTrailingSlash(fromUri.Segments[1]) == "Exercise" &&
                int.TryParse(RemoveTrailingSlash(fromUri.Segments[2]), out chapter) &&
                int.TryParse(RemoveTrailingSlash(fromUri.Segments[3]), out exercise))
            {
                var thisExercise = await rep.GetExerciseAsync(chapter, exercise);
                var nextExercise = await rep.GetNextExerciseAsync(chapter, exercise);

                if (advance)
                {
                    if (thisExercise.IsAssessed)
                    {
                        // Can't just decide to move on from an assessed exercise
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Can't call the Next action from an assessed exercise");
                    }
                    await userServices.AdvanceToExercise(nextExercise);
                }
                return RedirectToAction("Exercise", new { chapter = nextExercise.ChapterNo, exercise = nextExercise.ExerciseNo });
            }
            else
            {
                // We haven't come from an exercise
                return RedirectToAction("CurrentExercise");
            }
        }

        private string RemoveTrailingSlash(string s)
        {
            return s.EndsWith("/") ? s.Substring(0, s.Length - 1) : s;
        }

        [HttpPost]
        [AuthorizeExercise]
        public async Task<ActionResult> ExercisePopout(PopoutViewModel viewModel)
        {
            var exerciseDetails = await rep.GetExerciseAsync(viewModel.Chapter, viewModel.Exercise);
            if (exerciseDetails == null) throw new HttpException(404, "Not found");
            
            return View(new RunCodeViewModel {
                Exercise = exerciseDetails,
                SavedCode = viewModel.Code
            });
        }

        [Authorize]
        public async Task<ActionResult> CurrentExercise()
        {
            var user = await userServices.GetCurrentUserAsync();
            return RedirectToAction("Exercise", new { chapter = user.CurrentChapterNo, exercise = user.CurrentExerciseNo });
        }

        [ChildActionOnly]
        public ActionResult ExerciseSidebar(Exercise currentExercise)
        {
            async Task<ActionResult> ExerciseSidebarAsync()
            {
                ViewBag.CurrentExercise = currentExercise;

                ViewBag.User = await userServices.GetCurrentUserAsync();
                return PartialView("ExerciseSidebar", await rep.GetAllChaptersAsync());
            }

            return RunWithoutSyncContext(() => ExerciseSidebarAsync());
        }

        public async Task<ActionResult> ExerciseSidebarRefresh(int chapter, int exercise)
        {
            var ex = await rep.GetExerciseAsync(chapter, exercise);
            return ExerciseSidebar(ex);
        }

        [Authorize]
        [ChildActionOnly]
        public ActionResult RunCode(Exercise exercise, bool fullScreen = false, string overrideSavedCode = null)
        {
            async Task<ActionResult> RunCodeAsync()
            {
                string activeTheme = await userServices.GetCodeMirrorThemeAsync();
                return await RunCodeInternal(exercise, activeTheme, fullScreen, overrideSavedCode);
            }

            return RunWithoutSyncContext(() => RunCodeAsync());
        }

        [Authorize]
        [ChildActionOnly]
        public ActionResult RunCodeById(int chapter, int exercise, bool fullScreen = false, string overrideSavedCode = null)
        {
            async Task<ActionResult> RunCodeAsync()
            {
                Task<string> themeTask = userServices.GetCodeMirrorThemeAsync();
                Task<Exercise> exerciseTask = rep.GetExerciseAsync(chapter, exercise);

                await Task.WhenAll(themeTask, exerciseTask);

                string activeTheme = themeTask.Result;
                Exercise model = exerciseTask.Result;

                return await RunCodeInternal(model, activeTheme, fullScreen, overrideSavedCode);
            }

            return RunWithoutSyncContext(() => RunCodeAsync());
        }

        private async Task<ActionResult> RunCodeInternal(Exercise model, string activeTheme, bool fullScreen, string overrideSavedCode)
        {
            // The overrideSavedCode parameter is used when the caller knows that there may be
            // a more up to date version of the code than that which is saved in the database.
            // For example, when the editor is popped out, the current text may be the most up to date
            var savedCode = overrideSavedCode ?? (await rep.GetSavedWorkAsync(model.ChapterNo, model.ExerciseNo, userServices.GetCurrentUserId()))?.SavedCode;

            var viewModel = new RunCodeViewModel
            {
                Exercise = model,
                SavedCode = savedCode,
                FullScreenIde = fullScreen
            };

            ViewBag.Theme = CodeMirrorThemes.Themes.ConvertAll(t => new SelectListItem()
            {
                Text = char.ToUpper(t[0]) + t.Substring(1),
                Value = t,
                Selected = t == activeTheme
            });

            return PartialView("RunCode", viewModel);
        }

        [Authorize]
        [ChildActionOnly]
        public ActionResult Content(ExerciseContentViewModel viewModel)
        {
            // Make a razor engine service with the System.Web.Mvc namespace open
            var config = new TemplateServiceConfiguration();
            var namespaces = config.Namespaces;
            namespaces.Add("System.Web.Mvc");
            var service = RazorEngineService.Create(config);
            
            // Make the UrlHelper and HtmlHelper extension methods available in (close to) the same
            // way as a normal view
            string fullSource = "@using System.Web.Mvc.Html; " +
                                "@using CodingTrainer.CodingTrainerWeb.ViewExtensions; " +
                                "@{var Url = Model.Url; var Html = Model.Html;} "
                    + viewModel.Exercise.Content;

            // Including the hash code in the key allows this to work even if the source changes
            // However, this will result in memory leaks. The fix for this is to write a new
            // caching provider - however, this is considered to be a minor issue, and is
            // acceptable for now - see this post by Matthias Dittrich
            // https://github.com/Antaris/RazorEngine/issues/232#issuecomment-128802285
            var key = $"ContentTemplate{viewModel.Exercise.ChapterNo}-{viewModel.Exercise.ExerciseNo}-{fullSource.GetHashCode()}";
            service.AddTemplate(key, new LoadedTemplateSource(fullSource));


            // Finally, compile and run the content source
            var result = service.RunCompile(key, typeof(ExerciseContentViewModel), viewModel);
            return Content(result);
        }

        [Authorize]
        public ActionResult ModelAnswer(int chapter, int exercise)
        {
            var exerciseDetails = rep.GetExercise(chapter, exercise);
            if (exerciseDetails.ModelAnswer == null)
            {
                return PartialView("ModelAnswerNotFound");
            }

            // Has the user completed this exercise?
            var user = userServices.GetCurrentUser();
            if (user.CurrentChapterNo > chapter || (user.CurrentChapterNo == chapter && user.CurrentExerciseNo > exercise))
            {
                var result = new ContentResult();
                result.Content = exerciseDetails.ModelAnswer;
                result.ContentType = "text/html";
                return result;
            }
            else
            {
                return PartialView(); // Model answer will be shown when the exercise is completed
            }
        }

        // Helpers

        private T RunWithoutSyncContext<T>(Func<Task<T>> task)
        {
            var syncContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null); // Can't run async from partial views without this

            T result = task().Result;

            SynchronizationContext.SetSynchronizationContext(syncContext);
            return result;
        }
    }
}