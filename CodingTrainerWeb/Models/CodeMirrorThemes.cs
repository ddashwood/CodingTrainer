using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace CodingTrainer.CodingTrainerWeb.Models
{
    public static class CodeMirrorThemes
    {
        public static List<string> Themes { get; }
        public static List<string> ThemeFiles { get; }

        static CodeMirrorThemes()
        {
            Themes = new List<string>();
            ThemeFiles = new List<string>();

            var di = new DirectoryInfo(HostingEnvironment.MapPath("~/Scripts/codemirror/theme"));
            foreach (var file in di.GetFiles())
            {
                Themes.Add(Path.GetFileNameWithoutExtension(file.Name));
                ThemeFiles.Add("~/Scripts/codemirror/theme/" + file.Name);
            }
        }
    }
}