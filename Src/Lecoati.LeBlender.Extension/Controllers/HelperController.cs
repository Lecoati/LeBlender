using LightInject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Editors;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Core.Composing;
using Umbraco.Web.WebApi;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Lecoati.LeBlender.Extension.Controllers
{
	[PluginController("LeBlender")]
    public class HelperController : UmbracoAuthorizedApiController
    {
		private readonly ILogger logger;
		private readonly AppCaches appCaches;

		public HelperController(ILogger logger)
		{
			this.appCaches = Current.Factory.GetInstance<AppCaches>();
			this.logger = logger;
		}

        [HttpPost]
        public HttpResponseMessage GetPartialViewResultAsHtmlForEditor([FromBody]JObject jObj)
        {
			try
			{
				var modelStr = (string)jObj["model"];
				var view = (string)jObj["view"];
				var model = JsonConvert.DeserializeObject<JObject>( modelStr );
				model["contentId"] = (int)jObj["id"];
				var result = new HttpResponseMessage( HttpStatusCode.OK );
				string viewResult = new ViewRenderer().RenderPartialViewToString( "/views/Partials/" + view + ".cshtml", model );
				result.Content = new StringContent( viewResult );
				return result;
			}
			catch (Exception ex)
			{
				this.logger.Error<HelperController>( ex );
				throw;  // returns a 500
			}
		}

		[HttpPost]
		public object SaveEditorConfig([FromBody] JObject jObj)
		{
			try
			{
				var config = jObj["config"];
				var configPath = (string)jObj["configPath"];

				// Update
				using (System.IO.StreamWriter file = new System.IO.StreamWriter( System.Web.HttpContext.Current.Server.MapPath( configPath ) ))
				{
					file.Write( config.ToString() );
				}

				// Refrech GridConfig for next use

				appCaches.RequestCache.ClearByKey( "LeBlenderControllers" );
				appCaches.RequestCache.ClearByKey( "LeBlenderGridEditorsList" );

				appCaches.RuntimeCache.ClearByRegex( @"LEBLENDEREDITOR\.*" );

				// See GridEditorsConfig.cs in Umbraco.Core
				// We need to invalidate the cache, in order to load the change configs during next use.
				appCaches.RuntimeCache.ClearByKey( "Umbraco.Core.Configuration.Grid.GridEditorsConfig.Editors" );

				return new { Message = "Saved" };
			}
			catch (Exception ex)
			{
				this.logger.Error<HelperController>( ex );
				throw;  // returns a 500
			}
		}

	}

}