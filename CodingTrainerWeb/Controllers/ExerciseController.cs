using CodingTrainer.CodingTrainerModels.Repositories;
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
            var exercise = await rep.GetExerciseAsync(0);

            return View(exercise);
        }
    }
}