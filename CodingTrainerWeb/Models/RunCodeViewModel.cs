using CodingTrainer.CodingTrainerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingTrainer.CodingTrainerWeb.Models
{
    public class RunCodeViewModel
    {
        public Exercise Exercise { get; set; }
        public string SavedCode { get; set; }
    }
}