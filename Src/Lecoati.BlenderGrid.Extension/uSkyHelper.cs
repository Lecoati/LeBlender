using Lecoati.BlenderGrid.Extension.Models;
using Newtonsoft.Json;
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

        public static UmbracoHelper GetUmbracoHelper() {
            return new UmbracoHelper(UmbracoContext.Current);
        }

        public static dynamic GetFirstValue(dynamic values) {
            if (values != null && System.Linq.Enumerable.Any(values)) {
                return System.Linq.Enumerable.First(values);
            }
            else {
                return null;
            }
        }

        public static bool IsFrontEnd() {
            return UmbracoContext.Current.IsFrontEndUmbracoRequest;
        }

        public static IPublishedContent GetCurrentContent()
        {
            if (UmbracoContext.Current.IsFrontEndUmbracoRequest) {
                return GetUmbracoHelper().AssignedContentItem;
            }
            else {
                return GetUmbracoHelper().TypedContent(HttpContext.Current.Request["id"].ToString());
            }
        }

        public static IPublishedContent GetFirstContent(dynamic contentPicker)
        {
            if (contentPicker != null
                && contentPicker.value != null
                && System.Linq.Enumerable.Any(contentPicker.value)
                && System.Linq.Enumerable.First(contentPicker.value).id != null)
            {
                var id = System.Linq.Enumerable.First(contentPicker.value).id.Value;
                return GetUmbracoHelper().TypedContent(id.ToString());
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
            return GetUmbracoHelper().TypedMedia(mediaId);
        }
    }
}