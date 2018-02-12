using CodingTrainer.CodingTrainerModels.Repositories;
using CodingTrainer.CodingTrainerWeb.Models;
using CodingTrainer.CodingTrainerWeb.ViewModels;
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

        public ExerciseController(ICodingTrainerRepository repository)
        {
            rep = repository;
        }
        public ExerciseController()
            : this(new SqlCodingTrainerRepository())
        { }

        [Authorize]
        public async Task<ActionResult> RunCode()
        {
            ViewBag.Theme = CodeMirrorThemes.Themes.ConvertAll(t => new SelectListItem()
                    { Text = char.ToUpper(t[0]) + t.Substring(1), Value = t, Selected = t == "elegant" });

            var exercise = await rep.GetExerciseAsync(1, 1);

            return View(new ExerciseViewModel(exercise));
        }
    }
}