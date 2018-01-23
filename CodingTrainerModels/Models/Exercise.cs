using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CodingTrainer.CodingTrainerModels.Models
{
    public class Exercise
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ExerciseId { get; set; }
        [Required]
        public string DefaultCode { get; set; }
        public string ModelAnswer { get; set; }
    }
}