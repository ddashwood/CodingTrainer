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
        public async Task<Exercise> GetExerciseAsync(int id)
        {
            using (var context = new CodingTrainerContext())
            {
                return await context.Exercises.SingleAsync(e => e.ExerciseId == id);
            }
        }
    }
}