using CodingTrainer.CodingTrainerModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingTrainer.CodingTrainerModels.Repositories
{
    public interface ICodingTrainerRepository
    {
        Exercise GetExercise(int number);
    }
}