using CodingTrainer.CodingTrainerModels.Models;
using CodingTrainer.CodingTrainerModels.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CodingTrainer.CodingTrainerWeb.ApiControllers
{
    public class ChapterController : ApiController
    {
        ICodingTrainerRepository rep;
        public ChapterController(ICodingTrainerRepository repository)
        {
            rep = repository;
        }

        // GET api/<controller>
        public async Task<IEnumerable<Chapter>> Get()
        {
            var data = await rep.GetAllChaptersAsync();
            return data;
        }

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}