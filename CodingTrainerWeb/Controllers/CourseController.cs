using CodingTrainer.CodingTrainerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodingTrainer.CodingTrainerWeb.Controllers
{
    public class CourseController : Controller
    {
        public ActionResult Content(Exercise exercise)
        {
            return Content(exercise.ChapterNo, exercise.ExerciseNo);
        }

        private ActionResult Content(int chapter, int exercise)
        {
            string viewName = $"c{chapter}_e{exercise}";
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, viewName, null);

            if (result.View == null)
            {
                return PartialView("NotFound");
            }

            return PartialView(viewName);
        }
    }
}