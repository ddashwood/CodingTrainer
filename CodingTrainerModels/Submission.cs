using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTrainer.CodingTrainerModels.Security;
using Newtonsoft.Json;

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
        [JsonIgnore]
        public virtual Exercise Exercise { get; set; }

        [JsonIgnore]
        [ForeignKey("User")]
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public DateTime SubmissionDateTime { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string SubmittedCode { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Output { get; set; }

        [Required]
        public bool Success { get; set; }
    }
}
