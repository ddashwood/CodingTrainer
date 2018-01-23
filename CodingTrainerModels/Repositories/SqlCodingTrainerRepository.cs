using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodingTrainer.CodingTrainerModels.Contexts;
using CodingTrainer.CodingTrainerModels.Models;

namespace CodingTrainer.CodingTrainerModels.Repositories
{
    public class SqlCodingTrainerRepository : ICodingTrainerRepository
    {
        CodingTrainerContext context = new CodingTrainerContext();

        public Exercise GetExercise(int id)
        {
            return context.Exercises.Single(e => e.ExerciseId == id);
        }
    }
}