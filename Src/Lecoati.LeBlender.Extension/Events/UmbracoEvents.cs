//clear cache on publish
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Logging;
using Umbraco.Core.Publishing;
using Umbraco.Web;
using umbraco.interfaces;
using System.Web;
using Lecoati.LeBlender.Extension;

namespace Lecoati.LeBlender.Extension.Events
{
    public class UmbracoEvents : ApplicationEventHandler
    {

        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationInitialized(umbracoApplication, applicationContext);

            RouteTable.Routes.MapRoute(
                "leblender",
                "umbraco/backoffice/leblender/helper/{action}",
                new
                {
                    controller = "Helper",
                }
            );
        }

        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

            // Upgrate default view path for LeBlender 1.0.0
            var gridConfig = HttpContext.Current.Server.MapPath("~/Config/grid.editors.config.js");
            if (System.IO.File.Exists(gridConfig))
            {
                try
                {
                    string readText = System.IO.File.ReadAllText(gridConfig);
                    if (readText.IndexOf("/App_Plugins/Lecoati.LeBlender/core/LeBlendereditor.html") > 0
                        || readText.IndexOf("/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/LeBlendereditor.html") > 0
                        || readText.IndexOf("/App_Plugins/Lecoati.LeBlender/core/views/Base.cshtml") > 0
                        || readText.IndexOf("/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/views/Base.cshtml") > 0
                        )
                    {
                        readText = readText.Replace("/App_Plugins/Lecoati.LeBlender/core/LeBlendereditor.html", "~/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html")
                            .Replace("/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/LeBlendereditor.html", "~/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html")
                            .Replace("/App_Plugins/Lecoati.LeBlender/core/views/Base.cshtml", "~/App_Plugins/LeBlender/editors/leblendereditor/views/Base.cshtml")
                            .Replace("/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/views/Base.cshtml", "~/App_Plugins/LeBlender/editors/leblendereditor/views/Base.cshtml");
                        System.IO.File.WriteAllText(gridConfig, readText);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error<Helper>("Enable to upgrate LeBlender 1.0.0", ex);
                }
            }

            base.ApplicationStarting(umbracoApplication, applicationContext);
            PublishingStrategy.Published += PublishingStrategy_Published;
        }

        private void PublishingStrategy_Published(IPublishingStrategy sender, Umbraco.Core.Events.PublishEventArgs<Umbraco.Core.Models.IContent> e)
        {
            ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheByKeySearch("LEBLENDEREDITOR");
        }

    }
}