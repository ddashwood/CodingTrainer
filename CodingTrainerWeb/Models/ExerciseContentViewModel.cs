using CodingTrainer.CodingTrainerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodingTrainer.CodingTrainerWeb.Models
{
    public class ExerciseContentViewModel
    {
        public Exercise Exercise { get; set; }
        public UrlHelper Url { get; set; }
        public HtmlHelper Html { get; set; }
    }
}