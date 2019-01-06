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
                      "~/Scripts/respond.js",
                      "~/Scripts/app/navbar-fix.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/codemirror").Include(
                        "~/Scripts/codemirror/lib/codemirror.js",
                        "~/Scripts/codemirror/mode/clike/clike.js",
                        "~/Scripts/codemirror/addon/edit/closebrackets.js",
                        "~/Scripts/codemirror/addon/edit/matchbrackets.js",
                        "~/Scripts/codemirror/addon/display/panel.js",
                        "~/Scripts/codemirror/addon/lint/lint.js",
                        "~/Scripts/codemirror/addon/comment/comment.js",
                        "~/Scripts/codemirror/addon/hint/show-hint.js",
                        "~/Scripts/app/codemirror-addon/performed-lint.js",
                        "~/Scripts/app/codemirror-addon/hint-loader.js",
                        "~/Scripts/app/codemirror-addon/console.js",
                        "~/Scripts/codemirror-buttons/buttons.js"));

            bundles.Add(new ScriptBundle("~/bundles/runcode").Include(
                        // RxJS
                        "~/Scripts/Rx.js",

                        // Factories
                        "~/Scripts/app/runcode/signalRFactory.js",
                        "~/Scripts/app/runcode/serviceFactory.js",
                        "~/Scripts/app/runcode/serviceFactoryForHiddenCode.js",

                        // SignalR wrappers
                        "~/Scripts/app/runcode/codeRunner.js",
                        "~/Scripts/app/runcode/ideServices.js",

                        // Code corrections for dealing with hidden code
                        "~/Scripts/app/runcode/codeRunnerWithCorrections.js",
                        "~/Scripts/app/runcode/ideServicesWithCorrections.js",

                        // The editor
                        "~/Scripts/app/runcode/ide.js",
                        "~/Scripts/app/runcode/ideEditor.js",
                        "~/Scripts/app/runcode/ideConsole.js"));

            var codeMirrorStyles = new List<string> {
                        "~/Scripts/codemirror/lib/codemirror.css",
                        "~/Scripts/codemirror/addon/lint/lint.css",
                        "~/Scripts/codemirror/addon/hint/show-hint.css",
                        "~/Scripts/codemirror-buttons/buttons.css"};
            codeMirrorStyles.AddRange(CodeMirrorThemes.ThemeFiles);

            bundles.Add(new StyleBundle("~/Content/codemirror").Include(codeMirrorStyles.ToArray()));
        }
    }
}
