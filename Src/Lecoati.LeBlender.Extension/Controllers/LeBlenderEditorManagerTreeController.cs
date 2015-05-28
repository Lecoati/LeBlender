using Lecoati.LeBlender.Extension;
using Lecoati.LeBlender.Extension.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
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
            var menu = new MenuItemCollection();
            if (id == Constants.System.Root.ToInvariantString())
            {
                // root actions              
                menu.Items.Add<CreateChildEntity, ActionNew>(ui.Text("actions", ActionNew.Instance.Alias));
                menu.Items.Add<ActionSort>(ui.Text("actions", ActionSort.Instance.Alias));
                menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
                return menu;
            }
            else
            {
                menu.Items.Add<ActionDelete>(ui.Text("actions", ActionDelete.Instance.Alias));
            }
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
