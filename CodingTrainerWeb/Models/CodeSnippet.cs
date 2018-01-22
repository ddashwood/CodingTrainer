using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingTrainer.CodingTrainerWeb.Models
{
    public class CodeSnippet
    {
        public string DefaultCode { get; private set; }

        public CodeSnippet(string defaultCode)
        {
            DefaultCode = defaultCode;
        }
    }
}