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
        [Required, Key]
        public int ExerciseId { get; set; }

        [ForeignKey("Chapter")]
        [Index("IX_ExerciseChapterSequence", IsUnique = true, Order = 0)]
        public int ChapterId { get; set; }

        [Index("IX_ExerciseChapterSequence", IsUnique = true, Order = 1)]
        public int ExerciseNo { get; set; }

        [Required]
        public string DefaultCode { get; set; }
        public string ModelAnswer { get; set; }



        public virtual Chapter Chapter { get; set; }
    }
}