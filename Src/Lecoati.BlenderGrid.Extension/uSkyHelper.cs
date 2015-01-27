using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Lecoati.BlenderGrid.Extension
{
    public class Helper
    {

        public static IPublishedContent GetFirstContent(dynamic contentPicker)
        {
            if (contentPicker != null
                && contentPicker.value != null
                && System.Linq.Enumerable.Any(contentPicker.value)
                && System.Linq.Enumerable.First(contentPicker.value).id != null)
            {
                var id = System.Linq.Enumerable.First(contentPicker.value).id.Value;
                return (new UmbracoHelper(UmbracoContext.Current)).TypedContent(id.ToString());
            }
            else
            {
                return null;
            }
        }

        public static IPublishedContent GetFirstMedia(dynamic mediaPicker)
        {
            if (mediaPicker != null
                && mediaPicker.value != null
                && System.Linq.Enumerable.Any(mediaPicker.value)
                && System.Linq.Enumerable.First(mediaPicker.value).id != null)
            {
                var id = System.Linq.Enumerable.First(mediaPicker.value).id.Value;
                return Helper.GetFirstMedia(id.ToString());
            }
            else
            {
                return null;
            }
        }

        public static IPublishedContent GetFirstMedia(string mediaId)
        {
            return (new UmbracoHelper(UmbracoContext.Current)).TypedMedia(mediaId);
        }
    }
}