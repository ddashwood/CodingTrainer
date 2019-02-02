using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CodingTrainer.CodingTrainerWeb.ApiControllers
{
    public class SavedWorkController : ApiController
    {
        IUserServices userService;
        ICodingTrainerRepository repository;

        public SavedWorkController(IUserServices userService, ICodingTrainerRepository repository)
        {
            this.userService = userService;
            this.repository = repository;
        }

        // PUT api/SavedWork/chapter/exercise
        public async Task Put(int chapter, int exercise, [FromBody]string savedWork)
        {
            var dataToSave = new SavedWork
            {
                UserId = userService.GetCurrentUserId(),
                ChapterNo = chapter,
                ExerciseNo = exercise,
                SavedCode = savedWork
            };

            await repository.SaveWorkAsync(dataToSave);
        }
    }
}