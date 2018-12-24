using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CodingTrainer.CodingTrainerModels
{
    public class Exercise : IComparable<Exercise>
    {
        [Key, Column(Order = 0), ForeignKey("Chapter"), Required]
        public int ChapterNo { get; set; }

        [Key, Column(Order = 1), Required]
        public int ExerciseNo { get; set; }

        [Required]
        public string ExerciseName { get; set; }

        [Required]
        public string DefaultCode { get; set; }

        public string HiddenCodeHeader { get; set; }

        [Required]
        public string Content { get; set; }

        [JsonIgnore]
        public virtual Chapter Chapter { get; set; }

        // To sort exercises, e.g. in the exercise list, sort them by exercise number
        public int CompareTo(Exercise other)
        {
            if (ChapterNo != other.ChapterNo)
            {
                throw new InvalidOperationException("Can only compare two exercises with each other if they are both in the same chapter");
            }
            return ExerciseNo.CompareTo(other.ExerciseNo);
        }
    }
}