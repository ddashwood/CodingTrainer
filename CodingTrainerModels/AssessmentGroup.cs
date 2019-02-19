using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        [System.Runtime.Serialization.IgnoreDataMember]
        public Exercise Exercise { get; set; }

        public virtual ICollection<AssessmentBase> Assessments { get; set; }
    }
}
