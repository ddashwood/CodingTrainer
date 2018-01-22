using CodingTrainer.CodingTrainerWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CodingTrainer.CodingTrainerWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RunCode()
        {
            var exercise = new CodeSnippet(
// Demo code only for now...
@"using static System.Console;

WriteLine(""Enter some text"");
string s = ReadLine();
WriteLine(""You entered: "" + s); ");

            return View(exercise);
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}