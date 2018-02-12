using CodingTrainer.CodingTrainerWeb.Models;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace CodingTrainer.CodingTrainerWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/codemirror").Include(
                        "~/Scripts/codemirror/lib/codemirror.js",
                        "~/Scripts/codemirror/mode/clike/clike.js",
                        "~/Scripts/codemirror/addon/edit/closebrackets.js",
                        "~/Scripts/codemirror/addon/edit/matchbrackets.js",
                        "~/Scripts/codemirror/addon/lint/lint.js"));

            var codeMirrorStyles = new List<string> {
                        "~/Scripts/codemirror/lib/codemirror.css",
                        "~/Scripts/codemirror/addon/lint/lint.css"};
            codeMirrorStyles.AddRange(CodeMirrorThemes.ThemeFiles);

            bundles.Add(new StyleBundle("~/Content/codemirror").Include(codeMirrorStyles.ToArray()));
        }
    }
}
