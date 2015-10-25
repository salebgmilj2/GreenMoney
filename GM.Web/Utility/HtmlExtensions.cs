using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GM.Utility
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString Multiline(this HtmlHelper helper, string text)
        {
            if (string.IsNullOrEmpty(text))
                return MvcHtmlString.Create("");
            return MvcHtmlString.Create(text.Replace(Environment.NewLine, "<br />"));
        }

        public static string CurrentCouncil(this HtmlHelper helper)
        {
            return ConfigurationManager.AppSettings["Application.CurrentCouncil"].ToString();
        }

        public static string CurrentCouncilPath(this HtmlHelper helper)
        {
            return ConfigurationManager.AppSettings["Application.CurrentCouncilPath"].ToString();
        }

        public static bool Global(this HtmlHelper html)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["Global"] == null || bool.Parse(System.Configuration.ConfigurationManager.AppSettings["Global"]) == false)
                return false;

            return true;
        }

        //public static MvcHtmlString Resource(this HtmlHelper html, string type, int pageId)
        //{
        //    String value = "";
        //    IGreenMoneyContext context = new GreenMoney.Data.GreenMoneyContext("GreenMoney");

        //    IList<Resource> resources = (List<Resource>)context.Resources
        //                                .Include(i => i.ResourceType)
        //                                .Include(p => p.Page)
        //                                .Where(w => (w.ResourceType.Name == type) && (w.Page.Id == pageId))
        //                                .ToList<Resource>();

        //    if (resources.Any(a => a.ResourceType.Name == type))
        //    {
        //        value = resources.SingleOrDefault(a => a.ResourceType.Name == type).Value.ToString();
        //    }

        //    return MvcHtmlString.Create(value);
        //}

        public static string TrimIfLongerThan(this string value, int maxLength)
        {
            if (value.Length > maxLength)
            {
                return value.Substring(0, maxLength) + "...";
            }

            return value;
        }
    }
}