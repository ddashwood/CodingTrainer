using RazorEngine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodingTrainer.CodingTrainerWeb.ViewExtensions
{
    public static class MvcHtmlStringExtensions
    {
        public static RawString Raw(this MvcHtmlString htmlString)
        {
            return new RawString(htmlString.ToString());
        }
    }
}