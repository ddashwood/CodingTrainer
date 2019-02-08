using CodingTrainer.CodingTrainerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingTrainer.CodingTrainerWeb.ActionFilters
{
    interface ISubmissionController
    {
        Submission Submission { set; }
    }
}