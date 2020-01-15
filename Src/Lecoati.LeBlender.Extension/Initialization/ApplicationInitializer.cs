//clear cache on publish
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Web;
using System.Web;
using Lecoati.LeBlender.Extension;

namespace Lecoati.LeBlender.Extension.Events
{
    public class ApplicationInitializer : IComponent
    {
		private readonly ILogger logger;

		public ApplicationInitializer(ILogger logger)
		{
			this.logger = logger;
		}

		public void Initialize()
		{
			//RouteTable.Routes.MapRoute(
			//	"leblender",
			//	"umbraco/backoffice/leblender/helper/{action}",
			//	new
			//	{
			//		controller = "Helper",
			//	}
			//);

			// Upgrate default view path for LeBlender 1.0.0
			var gridConfig = HttpContext.Current.Server.MapPath( "~/Config/grid.editors.config.js" );
			if (System.IO.File.Exists( gridConfig ))
			{
				try
				{
					string readText = System.IO.File.ReadAllText( gridConfig );
					if (readText.IndexOf( "/App_Plugins/Lecoati.LeBlender/core/LeBlendereditor.html" ) > 0
						|| readText.IndexOf( "/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/LeBlendereditor.html" ) > 0
						|| readText.IndexOf( "/App_Plugins/Lecoati.LeBlender/core/views/Base.cshtml" ) > 0
						|| readText.IndexOf( "/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/views/Base.cshtml" ) > 0
						)
					{
						readText = readText.Replace( "/App_Plugins/Lecoati.LeBlender/core/LeBlendereditor.html", "/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html" )
							.Replace( "/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/LeBlendereditor.html", "/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html" )
							.Replace( "/App_Plugins/Lecoati.LeBlender/core/views/Base.cshtml", "/App_Plugins/LeBlender/editors/leblendereditor/views/Base.cshtml" )
							.Replace( "/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/views/Base.cshtml", "/App_Plugins/LeBlender/editors/leblendereditor/views/Base.cshtml" );
						System.IO.File.WriteAllText( gridConfig, readText );
					}
				}
				catch (Exception ex)
				{
					this.logger.Error<Helper>( "Enable to upgrade LeBlender 1.0.0", ex );
				}
			}
			
			// PublishingStrategy.Published += PublishingStrategy_Published;

		}

		public void Terminate()
		{
		}
    }
}