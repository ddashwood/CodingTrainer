using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerModels
{
    public class Chapter
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ChapterNo { get; set; }

        [Required]
        public string ChapterName { get; set; }

        public string Description { get; set; }

        public virtual List<Exercise> Exercises { get; set; }
    }
}
