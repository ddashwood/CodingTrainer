using CodingTrainer.CodingTrainerModels.Models;
using CodingTrainer.CodingTrainerModels.Repositories;
using CodingTrainer.CSharpRunner.CodeHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CodingTrainer.CodingTrainerWeb.Hubs.Helpers
{
    public class CodeRunnerLogger : IExceptionLogger
    {
        ICodingTrainerRepository rep;
        string userId;

        public CodeRunnerLogger(string userId, ICodingTrainerRepository repository)
        {
            this.userId = userId;
            rep = repository;
        }
        public CodeRunnerLogger(string userId)
            :this(userId, new SqlCodingTrainerRepository())
        { }


        public async Task LogException(Exception e, string code)
        {
            ExceptionLog log = new ExceptionLog
            {
                ExceptionText = e.ToString(),
                ExceptionDateTime = DateTimeOffset.Now,
                UserCode = code,
                UserId = userId
            };
            try
            {
                await rep.InsertExceptionLogAsync(log);
            }
            catch
            {
                // Any exceptions, try again without the user id
                // Probably could narrow this down to only foreign key violations...
                log.UserId = null;
                await rep.InsertExceptionLogAsync(log);
            }
        }
    }
}