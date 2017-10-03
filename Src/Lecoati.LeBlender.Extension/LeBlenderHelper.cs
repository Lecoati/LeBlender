using Lecoati.LeBlender.Extension.Controllers;
using Lecoati.LeBlender.Extension.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.Grid;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace Lecoati.LeBlender.Extension
{
    public class Helper
    {
        /// <summary>
        /// Is Front End
        /// </summary>
        /// <returns></returns>
        public static bool IsFrontEnd()
        {
            return UmbracoContext.Current.IsFrontEndUmbracoRequest;
        }

        /// <summary>
        /// Get Current Content, takes into account frontend and backend
        /// </summary>
        /// <returns></returns>
        public static IPublishedContent GetCurrentContent()
        {
            if (UmbracoContext.Current.IsFrontEndUmbracoRequest)
            {
                return GetUmbracoHelper().AssignedContentItem;
            }
            else
            {
                return GetUmbracoHelper().TypedContent(HttpContext.Current.Request["id"].ToString());
            }
        }

        /// <summary>
        /// Deserialize Blender Model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static LeBlenderModel DeserializeBlenderModel(dynamic model)
        {
            return JsonConvert.DeserializeObject<LeBlenderModel>(model.ToString());
        }

        /// <summary>
        /// Get inner message from Exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetInnerMessage(Exception ex)
        {
            if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                return ex.InnerException.Message;

            return ex.Message;
        }

        /// <summary>
        /// Get Cache expiration 
        /// </summary>
        /// <param name="LeBlenderEditorAlias"></param>
        /// <returns></returns>
        public static int GetCacheExpiration(String LeBlenderEditorAlias)
        {
            // Set default expiration.
            var expiration = 0;

            // Try to get expiration if specified.
            try
            {
                // Load editor for this type.
                var editor = GetLeBlenderGridEditors(true).FirstOrDefault(r => r.Alias == LeBlenderEditorAlias);

                // Attempt to load expiration data for this type.
                if (editor.Config.ContainsKey("expiration") && editor.Config["expiration"] != null)
                {
                    int.TryParse(editor.Config["expiration"].ToString(), out expiration);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<Helper>("Could not read editor expiration data from configuration.", ex);
            }

            return expiration;
        }

        #region internal

        /// <summary>
        /// Get and cache LeBlender Grid Editor 
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<IGridEditorConfig> GetLeBlenderGridEditors(bool onlyLeBlenderEditor)
        {
            Func<IEnumerable<IGridEditorConfig>> getResult = () =>
            {
                // Get Umbraco grid editor config including manifests
                var gridConfig = UmbracoConfig.For.GridConfig(
                    UmbracoContext.Current.Application.ProfilingLogger.Logger,
                    UmbracoContext.Current.Application.ApplicationCache.RuntimeCache,
                    new DirectoryInfo(UmbracoContext.Current.HttpContext.Server.MapPath(SystemDirectories.AppPlugins)),
                    new DirectoryInfo(UmbracoContext.Current.HttpContext.Server.MapPath(SystemDirectories.Config)),
                    UmbracoContext.Current.HttpContext.IsDebuggingEnabled);

                // Get grid editors
                var gridEditors = gridConfig.EditorsConfig.Editors;

                // Filter to only display LeBlender editors if needed
                if (onlyLeBlenderEditor)
                {
                    gridEditors = gridEditors.Where(x => x.View.Equals("/App_Plugins/LeBlender/core/LeBlendereditor.html", StringComparison.InvariantCultureIgnoreCase) ||
                                                         x.View.Equals("/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html", StringComparison.InvariantCultureIgnoreCase));
                }

                return gridEditors.ToList();
            };

            var result = (IEnumerable<IGridEditorConfig>)HttpContext.Current.Cache["LeBlenderGridEditorsList"];
            if (result == null || !onlyLeBlenderEditor)
            {
                result = getResult();
                HttpContext.Current.Cache.Add("LeBlenderGridEditorsList", result, null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
            }

            return result;

        }

        /// <summary>
        /// Get and cache LeBlender Controllers
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<Type> GetLeBlenderControllers()
        {

            Func<List<Type>> getResult = () =>
            {
                // https://our.umbraco.org/documentation/Reference/Plugins/finding-types
                var controllerTypes = PluginManager.Current.ResolveTypes<LeBlenderController>();
                return controllerTypes.ToList();
            };

            var result = (List<Type>)HttpContext.Current.Cache["LeBlenderControllers"];
            if (result == null)
            {
                result = getResult();
                HttpContext.Current.Cache.Add("LeBlenderControllers", result, null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
            }

            return (IEnumerable<Type>)result;

        }

        /// <summary>
        /// Get Leblender type by editorAlias
        /// </summary>
        /// <param name="editorAlias"></param>
        /// <returns></returns>
        internal static Type GetLeBlenderController(string editorAlias)
        {
            Type result = null;
            var controllers = GetLeBlenderControllers();

            if (controllers.Any())
            {
                var controllersFilter = controllers.Where(t => t.Name.Equals(editorAlias + "Controller", StringComparison.InvariantCultureIgnoreCase));
                result = controllersFilter.Any() ? controllersFilter.First() : null;
            }
            return result;
        }

        /// <summary>
        /// Build Cache Key
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal static string BuildCacheKey(string guid)
        {
            var cacheKey = new StringBuilder();
            cacheKey.Append("LEBLENDEREDITOR");
            cacheKey.Append(guid);
            cacheKey.Append(HttpContext.Current.Request.Url.PathAndQuery);
            return cacheKey.ToString().ToLower();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myId"></param>
        /// <returns></returns>
        internal static PublishedContentType GetTargetContentType()
        {
            if (UmbracoContext.Current.IsFrontEndUmbracoRequest)
            {
                return GetUmbracoHelper().AssignedContentItem.ContentType;
            }
            else if (!string.IsNullOrEmpty(HttpContext.Current.Request["doctype"]))
            {
                return PublishedContentType.Get(PublishedItemType.Content, HttpContext.Current.Request["doctype"]);
            }
            else
            {
                int contenId = int.Parse(HttpContext.Current.Request["id"]);
                return (PublishedContentType)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                    "LeBlender_GetTargetContentType_" + contenId,
                    () =>
                    {
                        var services = ApplicationContext.Current.Services;
                        var contentType = PublishedContentType.Get(PublishedItemType.Content, services.ContentService.GetById(contenId).ContentType.Alias);
                        return contentType;
                    });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myId"></param>
        /// <returns></returns>
        internal static IDataTypeDefinition GetTargetDataTypeDefinition(Guid myId)
        {
            return (IDataTypeDefinition)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                "LeBlender_GetTargetDataTypeDefinition_" + myId,
                () =>
                {
                    var services = ApplicationContext.Current.Services;
                    var dtd = services.DataTypeService.GetDataTypeDefinitionById(myId);
                    return dtd;
                });
        }

        #endregion

        #region private

        private static UmbracoHelper GetUmbracoHelper()
        {
            return new UmbracoHelper(UmbracoContext.Current);
        }

        #endregion

    }
}
