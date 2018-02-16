using CodingTrainer.CodingTrainerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingTrainer.CodingTrainerWeb.ViewModels
{
    public class ExerciseViewModel
    {
        public int ChapterId { get; set; }
        public int ExerciseNo { get; set; }
        public string DefaultCode { get; set; }
        public string HiddenCodeHeader { get; set; }

        public ExerciseViewModel(Exercise exercise)
        {
            ChapterId = exercise.ChapterId;
            ExerciseNo = exercise.ExerciseNo;
            DefaultCode = exercise.DefaultCode;
            HiddenCodeHeader = exercise.HiddenCodeHeader;
        }
    }
}