using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.Assessment
{
    [Table("AssessmentsByInspection")]
    public abstract class AssessmentByInspectionBase : AssessmentMethodBase
    {
        // Not mapped onto Entity Framework
        private bool compilationsSet = false;
        private CompilationWithSource compilation;
        [NotMapped]
        [IgnoreDataMember]
        public CompilationWithSource Compilation
        {
            get
            {
                return compilation;
            }
            set
            {
                compilationsSet = true;
                compilation = value;
            }
        }

        // Entity Framework properties
        [Required]
        public string InspectionInstructions { get; set; }
    }
}
