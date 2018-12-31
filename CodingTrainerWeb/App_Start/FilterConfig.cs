﻿using CodingTrainer.CodingTrainerWeb.ActionFilters;
using System.Web;
using System.Web.Mvc;

namespace CodingTrainer.CodingTrainerWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogAndHandleErrorAttribute());
        }
    }
}
