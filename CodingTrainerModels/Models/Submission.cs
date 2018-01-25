using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTrainer.CodingTrainerModels.Models.Security;

namespace CodingTrainer.CodingTrainerModels.Models
{
    public class Submission
    {
        [Required, Key]
        public int SubmissionId { get; set; }
        [Required]
        public string SubmittedCode { get; set; }
        [Required]
        public virtual Exercise Exercise { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
