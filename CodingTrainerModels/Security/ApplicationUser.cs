using CodingTrainer.CodingTrainerModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerModels.Security
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

    public class ApplicationUser : IdentityUser
    {
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [ForeignKey("CurrentExercise"), Column(Order = 0)]
        [DisplayName("Current Chapter")]
        public int CurrentChapterNo { get; set; }

        [Required]
        [ForeignKey("CurrentExercise"), Column(Order = 1)]
        [DisplayName("Current Exercise")]
        public int CurrentExerciseNo { get; set; }

        [Required]
        [DisplayName("Dark Theme")]
        public bool Dark { get; set; }

        [DisplayName("Email Confirmed")]
        public override bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }

        // For university study - some users will only see assessment of the running code, not static assessments
        [DisplayName("Hide Detailed Assessment")]
        public bool AssessByRunningOnly { get; set; }

        public bool Processed { get; set; }

        public virtual Exercise CurrentExercise { get; set; }

        public virtual ICollection<Submission> Submissions { get; set; }
        public virtual ICollection<ExceptionRunningUsersCode> ExceptionLogs { get; set; }
        public virtual ICollection<SavedWork> SaveWork { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }


        public bool ExercisePermitted(int chapter, int exercise)
        {
            if (CurrentChapterNo == chapter)
            {
                return CurrentExerciseNo >= exercise;
            }
            else
            {
                return CurrentChapterNo > chapter;
            }
        }

        public bool ExercisePermitted(Exercise exercise)
        {
            return ExercisePermitted(exercise.ChapterNo, exercise.ExerciseNo);
        }

        public void AdvanceToExercise(Exercise newExercise)
        {
            if (newExercise == null)
            {
                return;
            }

            if (CurrentChapterNo > newExercise.ChapterNo)
            {
                return;
            }

            if (CurrentChapterNo == newExercise.ChapterNo && CurrentExerciseNo > newExercise.ExerciseNo)
            {
                return;
            }

            CurrentChapterNo = newExercise.ChapterNo;
            CurrentExerciseNo = newExercise.ExerciseNo;
        }
    }
}
