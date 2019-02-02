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
    [Table("SavedWork")]
    public class SavedWork
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SavedWorkId { get; set; }

        [Column(Order = 0), ForeignKey("Exercise"), Required]
        [Index("IX_SavedWorkUserExercise", IsUnique = true, Order = 2)]
        public int ChapterNo { get; set; }

        [Column(Order = 1), ForeignKey("Exercise"), Required]
        [Index("IX_SavedWorkUserExercise", IsUnique = true, Order = 3)]
        public int ExerciseNo { get; set; }
        public virtual Exercise Exercise { get; set; }

        [ForeignKey("User")]
        [Index("IX_SavedWorkUserExercise", IsUnique = true, Order = 1)]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public string SavedCode { get; set; }
    }
}
