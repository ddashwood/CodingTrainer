using CodingTrainer.CodingTrainerEntityFrameworks.Contexts;
using CodingTrainer.CodingTrainerModels.Models;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerWeb.Repositories
{
    public class SqlCodingTrainerRepository : ICodingTrainerRepository
    {
        ApplicationDbContext context = new ApplicationDbContext();

        // Chapters

        public async Task<IEnumerable<Chapter>> GetAllChaptersAsync()
        {
            // Ignore chapters <0 - these will be things like the Playground which
            // are not real chapters
            var chapters = await context.Chapters.Where(c => c.ChapterNo >= 0).Include("Exercises").OrderBy(c => c.ChapterNo).ToListAsync();
            foreach (var chapter in chapters)
            {
                chapter.Exercises.Sort();
            }
            return chapters;
        }

        // Exercises

        public async Task<Exercise> GetExerciseAsync(int chapterNo, int exercisesNo)
        {
            var exercise = from e in context.Exercises
                           join c in context.Chapters on e.ChapterNo equals c.ChapterNo
                           where c.ChapterNo == chapterNo && e.ExerciseNo == exercisesNo
                           select e;
            return await exercise.SingleAsync();
        }

        // Exception logs

        public async Task InsertExceptionLogAsync(ExceptionLog log)
        {
            context.ExceptionLogs.Add(log);
            await context.SaveChangesAsync();
        }
    }

}
