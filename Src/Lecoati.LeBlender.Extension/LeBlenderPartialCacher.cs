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

        public static IHtmlString LeBlenderCachedPartial(
                        this HtmlHelper htmlHelper,
                        string partialViewName,
                        object model,
                        int cachedSeconds,
                        string guid,
                        ViewDataDictionary viewData = null
            )
        {
            var finalCacheKey = Helper.BuildCacheKey(guid);

            return (IHtmlString)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                finalCacheKey,
                () => htmlHelper.Partial(partialViewName, model, viewData),
                new TimeSpan(0, 0, 0, cachedSeconds),
                false,
                CacheItemPriority.NotRemovable //not removable, the same as macros (apparently issue #27610)
            );
        }

    }
}