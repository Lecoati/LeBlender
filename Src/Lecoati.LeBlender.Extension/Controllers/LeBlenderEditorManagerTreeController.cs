using Lecoati.LeBlender.Extension;
using Lecoati.LeBlender.Extension.Models;
using System.Collections.Generic;
using System.Linq;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Lecoati.LeBlender.Extension.Controllers
{

    [PluginController("LeBlender")]
    [Umbraco.Web.Trees.Tree("developer", "GridEditorManager", "Grid Editors", iconClosed: "icon-doc")]
    public class LeBlenderEditorManagerTreeController : TreeController
    {

        protected override Umbraco.Web.Models.Trees.MenuItemCollection GetMenuForNode(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            var textService = ApplicationContext.Services.TextService;
            var createText = textService.Localize($"actions/{ActionNew.Instance.Alias}");
            var sortText = textService.Localize($"actions/{ActionSort.Instance.Alias}");
            var refreshNodeText = textService.Localize($"actions/{ActionRefresh.Instance.Alias}");
            var deleteText = textService.Localize($"actions/{ActionDelete.Instance.Alias}");

            var menu = new MenuItemCollection();
            if (id == Constants.System.Root.ToInvariantString())
            {
                // root actions              
                menu.Items.Add<CreateChildEntity, ActionNew>(createText);
                menu.Items.Add<ActionSort>(sortText);
                menu.Items.Add<RefreshNode, ActionRefresh>(refreshNodeText, true);
                return menu;
            }
            menu.Items.Add<ActionDelete>(deleteText);
            return menu;
        }

        protected override Umbraco.Web.Models.Trees.TreeNodeCollection GetTreeNodes(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {

            var nodes = new TreeNodeCollection();
            if (id == "-1")
            {
                IList<GridEditor> editors = Helper.GetLeBlenderGridEditors(false).ToList();
                foreach (var editor in editors)
                {
                    nodes.Add(this.CreateTreeNode(editor.Alias, id, queryStrings, editor.Name, editor.Icon, false));
                }

                return nodes;
            }

            return nodes;
            
        }
    }
}
