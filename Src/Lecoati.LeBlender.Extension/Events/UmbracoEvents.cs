//clear cache on publish
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Logging;
using Umbraco.Core.Publishing;
using Umbraco.Web;
using umbraco.interfaces;

namespace Lecoati.leblender.Extension.Events
{
    public class UmbracoEvents : ApplicationEventHandler
    {

        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);
            PublishingStrategy.Published += PublishingStrategy_Published;
        }

        private void PublishingStrategy_Published(IPublishingStrategy sender, Umbraco.Core.Events.PublishEventArgs<Umbraco.Core.Models.IContent> e)
        {
            ApplicationContext.Current.ApplicationCache.ClearCacheByKeySearch("LEBLENDEREDITOR");
        }

    }
}