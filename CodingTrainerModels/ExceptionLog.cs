using CodingTrainer.CodingTrainerModels.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerModels
{
    public class ExceptionLog
    {
        [Required, Key]
        public int Id { get; set; }

        [Required]
        public string ExceptionText { get; set; }
        [Required]
        public DateTimeOffset ExceptionDateTime { get; set; }

        public string UserCode { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
