using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CodingTrainer.CodingTrainerModels.Models
{
    public class Exercise:IComparable<Exercise>
    {
        [Required, Key]
        public int ExerciseId { get; set; }

        [ForeignKey("Chapter"), Required, Index("IX_ExerciseChapterSequence", IsUnique = true, Order = 0)]
        public int ChapterId { get; set; }

        [Required, Index("IX_ExerciseChapterSequence", IsUnique = true, Order = 1)]
        public int ExerciseNo { get; set; }

        [Required]
        public string ExerciseName { get; set; }

        [Required]
        public string DefaultCode { get; set; }
        [JsonIgnore]
        public string ModelAnswer { get; set; }

        public string HiddenCodeHeader { get; set; }

        [JsonIgnore]
        public virtual Chapter Chapter { get; set; }

        public int CompareTo(Exercise other)
        {
            if (ChapterId!=other.ChapterId)
            {
                throw new InvalidOperationException("Can only compare two exercises with each other if they are both in the same chapter");
            }
            return ExerciseNo.CompareTo(other.ExerciseNo);
        }
    }
}