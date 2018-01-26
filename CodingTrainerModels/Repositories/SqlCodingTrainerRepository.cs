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
        // Exercises

        public async Task<Exercise> GetExerciseAsync(int id)
        {
            using (var context = new CodingTrainerContext())
            {
                return await context.Exercises.SingleAsync(e => e.ExerciseId == id);
            }
        }

        // Exception logs

        public void InsertExceptionLog(ExceptionLog log)
        {
            using (var context = new CodingTrainerContext())
            {
                context.ExceptionLogs.Add(log);
                context.SaveChanges();
            }
        }
    }
}