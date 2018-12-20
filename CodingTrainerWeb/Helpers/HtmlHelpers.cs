using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodingTrainer.CodingTrainerWeb.Helpers
{
    // Credit: Martin Booth (Just Another Dot Net Blog)
    // Original at https://jadnb.wordpress.com/2011/02/16/rendering-scripts-from-partial-views-at-the-end-in-mvc/

    // Modified to allow multiple sections to be added, in a similar
    // way to @section(), e.g. to allow one section for CSS
    // and another for JavaScript

    public static class HtmlHelpers
    {
        private class SectionBlock : IDisposable
        {
            private readonly string key;

            public static List<string> GetSection(string key)
            {
                if (HttpContext.Current.Items[key] == null)
                    HttpContext.Current.Items[key] = new List<string>();
                return (List<string>)HttpContext.Current.Items[key];
            }

            WebViewPage webPageBase;

            public SectionBlock(WebViewPage webPageBase, string key)
            {
                this.key = key;
                this.webPageBase = webPageBase;
                this.webPageBase.OutputStack.Push(new StringWriter());
            }

            public void Dispose()
            {
                GetSection(key).Add(((StringWriter)webPageBase.OutputStack.Pop()).ToString());
            }
        }

        /// <summary>
        /// Create a section of code to be included in another part of the view. Use with a
        /// using() {} statement to wrap the section.
        /// </summary>
        /// <param name="helper">HTML Helper</param>
        /// <param name="key">An identifier for the section</param>
        /// <returns></returns>
        public static IDisposable BeginSection(this HtmlHelper helper, string key)
        {
            return new SectionBlock((WebViewPage)helper.ViewDataContainer, key);
        }

        /// <summary>
        /// Renders a section that was created using Html.BeginSection()
        /// </summary>
        /// <param name="helper">HTML Helper</param>
        /// <param name="key">An identifier for the section</param>
        /// <returns></returns>
        public static MvcHtmlString RenderSection(this HtmlHelper helper, string key)
        {
            return MvcHtmlString.Create(string.Join(Environment.NewLine, SectionBlock.GetSection(key).Select(s => s.ToString())));
        }
    }
}