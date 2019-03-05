using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CodingTrainer.CodingTrainerModels
{
    public class AssessmentGroup
    {
        [Key, Required]
        [System.Runtime.Serialization.IgnoreDataMember]
        public int AssessmentGroupId { get; set; }

        [Column(Order = 0), ForeignKey("Exercise"), Required]
        [Index("IX_AssessmentGroup_Sequence", IsUnique = true, Order = 1)]
        public int ChapterNo { get; set; }

        [Column(Order = 1), ForeignKey("Exercise"), Required]
        [Index("IX_AssessmentGroup_Sequence", IsUnique = true, Order = 2)]
        public int ExerciseNo { get; set; }

        [Index("IX_AssessmentGroup_Sequence", IsUnique = true, Order = 3)]
        [Required]
        public int Sequence { get; set; }

        public string Title { get; set; }

        [Required]
        public bool ShowAutoMessageOnStart { get; set; }

        [Required]
        public bool ShowAutoMessageOnPass { get; set; }

        [Required]
        public bool ShowAutoMessageOnFail { get; set; }

        [Required]
        public bool EndAssessmentsOnFail { get; set; }

        [XmlIgnore]
        public Exercise Exercise { get; set; }

        [XmlIgnore]
        public virtual ICollection<AssessmentBase> Assessments { get; set; }

        [NotMapped]
        [XmlIgnore]
        public IEnumerable<AssessmentBase> OrderedAssessments
        {
            get
            {
                return Assessments.OrderBy(a => a.Sequence);
            }
        }
    }
}
