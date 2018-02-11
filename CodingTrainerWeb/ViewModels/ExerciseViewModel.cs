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
        public int HiddenHeaderLength { get; set; }

        public ExerciseViewModel(Exercise exercise)
        {
            ChapterId = exercise.ChapterId;
            ExerciseNo = exercise.ExerciseNo;
            // Change new lines to spaces in the header, otherwise line numbers go funny when we hide it
            DefaultCode = exercise.HiddenCodeHeader == null? exercise.DefaultCode: $"{exercise.HiddenCodeHeader.Replace("\r\n", " ").Replace("\n", " ")} {exercise.DefaultCode}";
            HiddenHeaderLength = exercise.HiddenCodeHeader == null? 0: exercise.HiddenCodeHeader.Length + 1; // Plus 1 due to space
        }
    }
}