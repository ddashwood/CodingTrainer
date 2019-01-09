using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment
{
    [Table("AssessmentByInspection")]
    public abstract class AssessmentByInspectionBase : AssessmentMethodBase
    {
        // Entity Framework properties
        [Required]
        public string InspectionInstructions { get; set; }
    }
}
