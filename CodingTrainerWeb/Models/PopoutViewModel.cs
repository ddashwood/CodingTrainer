using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodingTrainer.CodingTrainerWeb.Models
{
    public class PopoutViewModel
    {
        public int Chapter { get; set; }
        public int Exercise { get; set; }
        [AllowHtml]
        public string Code { get; set; }
    }
}