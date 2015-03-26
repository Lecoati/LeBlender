using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web;

namespace Lecoati.LeBlender.Extension
{
    public static class LeBlenderPartialCacher
    {

        public const string PARTIAL_CACHE_PREFIX = "LeBlender";

        //an extension method is created to be used in the views
        public static IHtmlString LeBlenderCachedPartial(
                        this HtmlHelper htmlHelper,
                        string partialViewName,
                        object model,
                        int cachedSeconds,
                        string customKey,
                        ViewDataDictionary viewData = null
            )
        {

            //the key will determine the uniqueness of the cache
            //the key ends up looking like this {prefix-url-customkey}
            var cacheKey = new StringBuilder();
            cacheKey.Append(PARTIAL_CACHE_PREFIX);

            cacheKey.Append(HttpContext.Current.Request.Url);

            if (customKey != "")
                cacheKey.Append("-" + customKey);

            var finalCacheKey = cacheKey.ToString().ToLower();

            //this code was lifted from the Umbraco Core and does the actual caching/retrieval of html
            return ApplicationContext.Current.ApplicationCache.GetCacheItem(
                    finalCacheKey,
                    CacheItemPriority.NotRemovable, //not removable, the same as macros (apparently issue #27610)
                    null,
                    new TimeSpan(0, 0, 0, cachedSeconds),
                    () => htmlHelper.Partial(partialViewName, model, viewData));
        }

    }
}