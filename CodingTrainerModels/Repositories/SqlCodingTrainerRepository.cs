using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CodingTrainer.CodingTrainerModels.Contexts;
using CodingTrainer.CodingTrainerModels.Models;

namespace CodingTrainer.CodingTrainerModels.Repositories
{
    public class SqlCodingTrainerRepository : ICodingTrainerRepository
    {
        ApplicationDbContext context = new ApplicationDbContext();

        // Chapters

        public async Task<IEnumerable<Chapter>> GetAllChaptersAsync()
        {
            var chapters = await context.Chapters.Include("Exercises").OrderBy(c=>c.ChapterNumber).ToListAsync();
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
                           join c in context.Chapters on e.ChapterId equals c.ChapterId
                           where c.ChapterNumber == chapterNo && e.ExerciseNo == exercisesNo
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