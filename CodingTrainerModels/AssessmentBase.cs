using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerModels
{
    [Table("Assessments")]
    public abstract class AssessmentBase
    {
        [Key, Required]
        public int AssessmentId { get; set; }

        [Column(Order = 0), ForeignKey("Exercise"), Required]
        public int ChapterNo { get; set; }

        [Column(Order = 1), ForeignKey("Exercise"), Required]
        public int ExerciseNo { get; set; }

        [Required]
        public string Title { get; set; }

        [IgnoreDataMember]
        public Exercise Exercise { get; set; }


        public abstract Task<bool> AssessAsync();

    }
}
