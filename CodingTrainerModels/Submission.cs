using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTrainer.CodingTrainerModels.Security;

namespace CodingTrainer.CodingTrainerModels
{
    public class Submission
    {
        [Required, Key]
        public int SubmissionId { get; set; }

        [Column(Order = 0), ForeignKey("Exercise"), Required]
        public int ChapterNo { get; set; }
        [Column(Order = 1), ForeignKey("Exercise"), Required]
        public int ExerciseNo { get; set; }
        public virtual Exercise Exercise { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public DateTime SubmissionDateTime { get; set; }

        [Required]
        public string SubmittedCode { get; set; }

        [Required]
        public string Output { get; set; }

        [Required]
        public bool Success { get; set; }
    }
}
