using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;


namespace CodingTrainer.CodingTrainerModels.Models
{
    public class Submission
    {
        [Required, Key]
        public int SubmissionId { get; set; }
        [Required]
        public string SubmittedCode { get; set; }
        [Required]
        public Exercise Exercise { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}
