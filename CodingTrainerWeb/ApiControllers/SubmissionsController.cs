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
    public class SubmissionsController : ApiController
    {
        ICodingTrainerRepository rep;
        IUserServices userServices;
        public SubmissionsController(ICodingTrainerRepository repository, IUserServices userServices)
        {
            rep = repository;
            this.userServices = userServices;
        }

        // GET /api/submissions/1/2
        [Authorize]
        public async Task<IEnumerable<Submission>> Get(int chapter, int exercise)
        {
            return await rep.GetSubmissionsAsync(chapter, exercise, userServices.GetCurrentUserId());
        }
    }
}
