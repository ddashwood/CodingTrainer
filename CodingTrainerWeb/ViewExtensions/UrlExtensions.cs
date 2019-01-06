using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodingTrainer.CodingTrainerWeb.ViewExtensions
{
    public static class UrlExtensions
    {
        public static string IncludeMin(this UrlHelper helper)
        {
#if DEBUG
            return string.Empty;
#else
            return ".min";
#endif
        }
    }
}