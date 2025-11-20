using System;
using System.Configuration;
using System.Web.Mvc;

namespace WinstantPay.Common.Extension
{
    public static class UrlExtensions
    {
        static string _path;
        static UrlExtensions()
        {
            _path = ConfigurationManager.AppSettings["uplodaPath"];
        }

        public static string AbsoluteImgContent(this UrlHelper urlHelper
            , string contentPath)
        {
            Uri requestUrl = urlHelper.RequestContext.HttpContext.Request.Url;

            string absolutePath = string.Format("{0}{1}",
                requestUrl.GetLeftPart(UriPartial.Authority),
                urlHelper.Content(contentPath.Replace(_path, "\\Uploads\\")));
            return absolutePath;

        }

    }
}