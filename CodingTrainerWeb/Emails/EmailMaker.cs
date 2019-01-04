using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace CodingTrainer.CodingTrainerWeb.Emails
{
    public class EmailMaker
    {
        public string MakeEmail(string filename, string userName, string url)
        {
            var fullFilename = "CodingTrainer.CodingTrainerWeb.Emails.Templates." + filename;
            var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullFilename);
            if (fileStream == null)
            {
                throw new FileNotFoundException("The email template could not be found", filename);
            }

            var fileReader = new StreamReader(fileStream);
            string body = fileReader.ReadToEnd();
            return string.Format(body, userName, url);
        }
    }
}