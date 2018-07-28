using System.Linq;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Lecoati.LeBlender.Extension.Controllers
{
    [PluginController("LeBlender")]
    [Tree("developer", "GridEditorManager", "Grid Editors", iconClosed: "icon-doc")]
    public class LeBlenderEditorManagerTreeController : TreeController
    {

        protected override MenuItemCollection GetMenuForNode(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            var textService = ApplicationContext.Services.TextService;

            var menu = new MenuItemCollection();
            if (IsRoot(id))
            {
                // root actions              
                var createText = textService.Localize($"actions/{ActionNew.Instance.Alias}");
                menu.Items.Add<CreateChildEntity, ActionNew>(createText);

                var sortText = textService.Localize($"actions/{ActionSort.Instance.Alias}");
                menu.Items.Add<ActionSort>(sortText);
                
                var refreshNodeText = textService.Localize($"actions/{ActionRefresh.Instance.Alias}");
                menu.Items.Add<RefreshNode, ActionRefresh>(refreshNodeText, true);
                return menu;
            }
            
            var deleteText = textService.Localize($"actions/{ActionDelete.Instance.Alias}");
            menu.Items.Add<ActionDelete>(deleteText);

            return menu;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            if (!IsRoot(id))
                 return nodes;

            var editors = Helper.GetLeBlenderGridEditors(false).ToList();
            foreach (var editor in editors)
            {
                nodes.Add(base.CreateTreeNode(editor.Alias, id, queryStrings, editor.Name, editor.Icon, false));
            }

            return nodes;
            
        }

        private static bool IsRoot(string id)
        {
            return id == Constants.System.Root.ToInvariantString();
        }
    }
}
