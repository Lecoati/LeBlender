using Lecoati.LeBlender.Extension;
using Lecoati.LeBlender.Extension.Models;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Actions;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Web.WebApi.Filters;

namespace Lecoati.LeBlender.Extension.Controllers
{

    [PluginController("LeBlender")]
	[UmbracoTreeAuthorize( Constants.Trees.DataTypes)]
	//[Tree( Constants.Applications.Settings, Constants.Trees.DocumentTypes, SortOrder = 0, TreeGroup = Constants.Trees.Groups.Settings )]
	[Tree( Constants.Applications.Settings, "GridEditorManager", TreeTitle ="Grid Editors", SortOrder = 4, TreeGroup = Constants.Trees.Groups.Settings )]
	public class LeBlenderEditorManagerTreeController : TreeController
    {
		private readonly ILogger mainLogger;
		private readonly UmbracoHelper umbracoHelper;
		private readonly AppCaches appCaches;

		public LeBlenderEditorManagerTreeController(ILogger logger, UmbracoHelper umbracoHelper, AppCaches appCaches)
		{
			this.mainLogger = logger;
			this.umbracoHelper = umbracoHelper;
			this.appCaches = appCaches;
		}

		protected override Umbraco.Web.Models.Trees.MenuItemCollection GetMenuForNode(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            var textService = Services.TextService;
            var createText = textService.Localize($"actions/{ActionNew.ActionAlias}");
            var sortText = textService.Localize($"actions/{new ActionSort().Alias}");
            //var refreshNodeText = textService.Localize($"actions/{ActionRefresh.Instance.Alias}");
            var deleteText = textService.Localize($"actions/{ActionDelete.ActionAlias}");

            var menu = new MenuItemCollection();
            if (id == Constants.System.Root.ToInvariantString())
            {
				// root actions              
				//menu.Items.Add<ActionNew>(createText, false, true);
				menu.Items.Add( new CreateChildEntity( Services.TextService, true ) );
				menu.Items.Add<ActionSort>(sortText, false, true);
				menu.Items.Add( new RefreshNode( Services.TextService, true ) );
				return menu;
            }
            menu.Items.Add<ActionDelete>(deleteText);
            return menu;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {

            var nodes = new TreeNodeCollection();

			try
			{
				if (id == "-1")
				{
					IList<GridEditor> editors = new Helper().GetLeBlenderGridEditors( false ).ToList();
					foreach (var editor in editors)
					{
						nodes.Add( this.CreateTreeNode( editor.Alias, id, queryStrings, editor.Name, editor.Icon, false ) );
					}

					return nodes;
				}
			}
			catch (System.Exception ex)
			{
				this.mainLogger.Error( GetType(), nameof( GetTreeNodes ), ex );
			}

            return nodes;            
        }
    }
}
