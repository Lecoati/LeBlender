using Lecoati.LeBlender.Extension.Controllers;
using Lecoati.LeBlender.Extension.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Editors;
using Umbraco.Web.Composing;

namespace Lecoati.LeBlender.Extension
{
    public class Helper
    {
		private readonly ILogger logger;
		private readonly AppCaches appCaches;
		private readonly UmbracoContext umbracoContext;

		public Helper()
		{
			this.logger = Current.Logger;
			this.appCaches = Current.AppCaches;
			this.umbracoContext = Current.UmbracoContext;
		}

        /// <summary>
        /// Is Front End
        /// </summary>
        /// <returns></returns>
        public bool IsFrontEnd()
        {
            return umbracoContext.IsFrontEndUmbracoRequest;
        }

        /// <summary>
        /// Get Current Content, takes into account frontend and backend
        /// </summary>
        /// <returns></returns>
        public IPublishedContent GetCurrentContent()
        {
            if (umbracoContext.IsFrontEndUmbracoRequest)
            {
                return umbracoContext.PublishedRequest.PublishedContent;
            }
            else
            {
				var si = (string)HttpContext.Current.Request["id"];
				int id = 0;
				int.TryParse( si, out id );
				return Current.UmbracoHelper.Content( id );
            }
        }

        /// <summary>
        /// Deserialize Blender Model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public LeBlenderModel DeserializeBlenderModel(dynamic model)
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
		public int GetCacheExpiration( String LeBlenderEditorAlias )
		{

			var result = 0;

			try
			{
				var editor = GetLeBlenderGridEditors( true ).FirstOrDefault( r => r.Alias == LeBlenderEditorAlias );
				if (editor.Config.ContainsKey( "expiration" ) && editor.Config["expiration"] != null)
				{
					int.TryParse( editor.Config["expiration"].ToString(), out result );
				}
			}
			catch (Exception ex)
			{
				this.logger.Error<Helper>( "Could not read expiration datas", ex );
			}

			return result;

		}

		#region internal

		/// <summary>
		/// Get and cache LeBlender Grid Editor 
		/// </summary>
		/// <returns></returns>
		internal IEnumerable<GridEditor> GetLeBlenderGridEditors(bool onlyLeBlenderEditor) 
        {            
            Func<List<GridEditor>> getResult = () =>
            {
                var editors = new List<GridEditor>();
                var gridConfig = HttpContext.Current.Server.MapPath("~/Config/grid.editors.config.js");
                if (System.IO.File.Exists(gridConfig))
                {
					try
					{
						var arr = JArray.Parse( System.IO.File.ReadAllText( gridConfig ) );
						var parsed = JsonConvert.DeserializeObject<IEnumerable<GridEditor>>( arr.ToString() ); ;
						editors.AddRange( parsed );

						if (onlyLeBlenderEditor)
						{
							editors = editors.Where( r => r.View.Equals( "/App_Plugins/LeBlender/core/LeBlendereditor.html", StringComparison.InvariantCultureIgnoreCase ) ||
								 r.View.Equals( "/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html", StringComparison.InvariantCultureIgnoreCase ) ).ToList();
						}
					}
					catch (Exception ex)
					{
						this.logger.Error<Helper>( "Could not parse the contents of grid.editors.config.js into a JSON array", ex );
					}
				}

                return editors;
            };

			var result = appCaches.RequestCache.GetCacheItem<List<GridEditor>>( "LeBlenderGridEditorsList", getResult );

            return result;

        }

        /// <summary>
        /// Get and cache LeBlender Controllers
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<Type> GetLeBlenderControllers()
		{

			var result = appCaches.RequestCache.GetCacheItem<IEnumerable<Type>>( "LeBlenderControllers",
			() =>
				{
					var controllerTypes = Umbraco.Core.Composing.TypeFinder.FindClassesOfType<LeBlenderController>();
					return controllerTypes.ToList();
				}
			);

            return result;

        }

        /// <summary>
        /// Get Leblender type by editorAlias
        /// </summary>
        /// <param name="editorAlias"></param>
        /// <returns></returns>
        internal Type GetLeBlenderController(string editorAlias)
        {
            Type result = null;
            var controllers = GetLeBlenderControllers();

            if (controllers.Any()) {
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
        internal static string BuildCacheKey(string guid) {
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
        internal IPublishedContentType GetTargetContentType()
        {
			return GetCurrentContent()?.ContentType;

			// This is the old code. We don't understand the case with "doctype".

            //if (umbracoContext.IsFrontEndUmbracoRequest)
            //{
            //    return this.umbracoHelper.AssignedContentItem.ContentType;
            //}
            //else if (!string.IsNullOrEmpty(HttpContext.Current.Request["doctype"]))
            //{
            //    return PublishedContentType.Get(PublishedItemType.Content, HttpContext.Current.Request["doctype"]);
            //}
            //else
            //{
            //    int contenId = int.Parse(HttpContext.Current.Request["id"]);
            //    return (PublishedContentType)appCaches.RuntimeCache.GetCacheItem(
            //        "LeBlender_GetTargetContentType_" + contenId,
            //        () =>
            //        {
            //            var services = ApplicationContext.Current.Services;
            //            var contentType = PublishedContentType.Get(PublishedItemType.Content, services.ContentService.GetById(contenId).ContentType.Alias);
            //            return contentType;
            //        });
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myId"></param>
        /// <returns></returns>
        internal IDataType GetTargetDataTypeDefinition(Guid myId)
        {
            return (IDataType)this.appCaches.RuntimeCache.GetCacheItem(
                "LeBlender_GetTargetDataTypeDefinition_" + myId,
                () =>
                {
					var services = Current.Services;
                    var dtd = services.DataTypeService.GetDataType(myId);
                    return dtd;
                });
        }

        #endregion

    }
}
