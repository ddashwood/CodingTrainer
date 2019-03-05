using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        public bool ShowAutoMessageOnStart { get; set; }

        [Required]
        public bool ShowAutoMessageOnPass { get; set; }

        [Required]
        public bool ShowAutoMessageOnFail { get; set; }

        [Required]
        public bool EndAssessmentGroupOnFail { get; set; }

        [Index("IX_Assessment_Sequence", IsUnique = true, Order = 2)]
        [Required]
        public int Sequence { get; set; }

        [XmlIgnore]
        public AssessmentGroup AssessmentGroup { get; set; }


        public abstract Task<bool> AssessAsync();

    }
}
