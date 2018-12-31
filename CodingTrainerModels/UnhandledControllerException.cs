using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerModels
{
    public class UnhandledControllerException
    {
        [Key]
        public int ExceptionId { get; set; }

        public string Message { get; set; }
        public string InnerExceptionMessages { get; set; }
        public string StackTrace { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }
        public string UserId { get; set; }

        public DateTime DateTime { get; set; }
    }
}
