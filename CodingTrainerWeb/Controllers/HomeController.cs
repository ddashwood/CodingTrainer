﻿using System;
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

        public ActionResult About()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult TermsAndConditions()
        {
            return View();
        }
    }
}