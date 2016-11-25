using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lecoati.LeBlender.Extension.Models
{
    [JsonObject]
    public class LeBlenderModel :  RenderModel {
        public LeBlenderModel() : this(new UmbracoHelper(UmbracoContext.Current).TypedContent(UmbracoContext.Current.PageId)) { }
        public LeBlenderModel(IPublishedContent content, CultureInfo culture) : base(content, culture) { }
        public LeBlenderModel(IPublishedContent content) : base(content) { }

        [JsonProperty("value")]
        public IEnumerable<LeBlenderValue> Items { get; set; }

        public IPublishedContent Content { get; private set; }
    }
}
