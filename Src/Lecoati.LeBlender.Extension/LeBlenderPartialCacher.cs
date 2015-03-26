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

        public static IHtmlString LeBlenderCachedPartial(
                        this HtmlHelper htmlHelper,
                        string partialViewName,
                        object model,
                        int cachedSeconds,
                        string customKey,
                        ViewDataDictionary viewData = null
            )
        {

            var cacheKey = new StringBuilder();
            cacheKey.Append(PARTIAL_CACHE_PREFIX);

            cacheKey.Append(HttpContext.Current.Request.Url);

            if (customKey != "")
                cacheKey.Append("-" + customKey);

            var finalCacheKey = cacheKey.ToString().ToLower();

            return ApplicationContext.Current.ApplicationCache.GetCacheItem(
                    finalCacheKey,
                    CacheItemPriority.NotRemovable, //not removable, the same as macros (apparently issue #27610)
                    null,
                    new TimeSpan(0, 0, 0, cachedSeconds),
                    () => htmlHelper.Partial(partialViewName, model, viewData));
        }

    }
}