using CodingTrainer.CodingTrainerModels.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CodingTrainer.CodingTrainerModels
{
    public class Exercise : IComparable<Exercise>
    {
        [Key, Column(Order = 0), ForeignKey("Chapter"), Required]
        public int ChapterNo { get; set; }

        [Key, Column(Order = 1), Required]
        public int ExerciseNo { get; set; }

        [Required]
        public string ExerciseName { get; set; }

        [Required]
        public string DefaultCode { get; set; }

        public string HiddenCodeHeader { get; set; }

        [Required]
        public string Content { get; set; }

        [JsonIgnore]
        public virtual Chapter Chapter { get; set; }

        // To sort exercises, e.g. in the exercise list, sort them by exercise number
        public int CompareTo(Exercise other)
        {
            if (ChapterNo != other.ChapterNo)
            {
                throw new InvalidOperationException("Can only compare two exercises with each other if they are both in the same chapter");
            }
            return ExerciseNo.CompareTo(other.ExerciseNo);
        }

        public bool Permitted(ApplicationUser user)
        {
            if (user == null) return false; // Not logged on

            return user.ExercisePermitted(this);
        }

        public static bool operator==(Exercise a, Exercise b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return (a.ChapterNo == b.ChapterNo) && (a.ExerciseNo == b.ExerciseNo);
        }
        public static bool operator!=(Exercise a, Exercise b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return this == (Exercise)obj;
        }
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + ChapterNo.GetHashCode();
                hash = hash * 23 + ExerciseNo.GetHashCode();
                return hash;
            }
        }
    }
}