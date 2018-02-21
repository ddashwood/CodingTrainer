using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingTrainer.CodingTrainerWeb.Hubs.Helpers
{
    public class IdeData
    {
        public CompilerError[] Diagnostics { get; set; }
        public IEnumerable<string> Completions { get; set; }
    }
}