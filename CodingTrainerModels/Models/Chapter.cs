using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerModels.Models
{
    public class Chapter
    {
        [Required, Key]
        public int ChapterId { get; set; }

        [Required]
        public int ChapterNumber { get; set; }

        [Required]
        public string ChapterName { get; set; }

        public string Description { get; set; }
    }
}
