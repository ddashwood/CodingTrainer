using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerModels
{
    [Table("Assessments")]
    public abstract class AssessmentBase
    {
        [Key, Required]
        [System.Runtime.Serialization.IgnoreDataMember]
        public int AssessmentId { get; set; }

        [ForeignKey("AssessmentGroup"), Required]
        [Index("IX_Assessment_Sequence", IsUnique = true, Order = 1)]
        public int AssessmentGroupId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public bool AbortOnFail { get; set; }

        [Index("IX_Assessment_Sequence", IsUnique = true, Order = 2)]
        [Required]
        public int Sequence { get; set; }

        [System.Runtime.Serialization.IgnoreDataMember]
        public AssessmentGroup AssessmentGroup { get; set; }


        public abstract Task<bool> AssessAsync();

    }
}
