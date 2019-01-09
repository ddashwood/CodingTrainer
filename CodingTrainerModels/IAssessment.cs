using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerModels
{
    public interface IAssessment
    {
        int AssessmentId { get; set; }

        [Column(Order = 0), ForeignKey("Exercise")]
        int ChapterNo { get; set; }

        [Column(Order = 1), ForeignKey("Exercise")]
        int ExerciseNo { get; set; }

        string Title { get; set; }

        Exercise Exercise { get; set; }


        Task<bool> AssessAsync();

    }
}
