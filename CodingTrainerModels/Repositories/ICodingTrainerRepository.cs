using CodingTrainer.CodingTrainerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerModels.Repositories
{
    public interface ICodingTrainerRepository
    {
        // Exercises
        Task <Exercise> GetExerciseAsync(int number);

        // Exception logs
        void InsertExceptionLog(ExceptionLog log);
    }
}