using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

using Lecoati.BlenderGrid.Extension.Models;

namespace Lecoati.BlenderGrid.Extension.Controllers
{
    public class BlenderController : SurfaceController
    {
        [ChildActionOnly]
        public ActionResult RenderEditor(string editorAlias, string frontView)
        {
            var model = new BlenderModel();

            var baseType = typeof(BlenderController);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(a => a.GetTypes().Where(t => t.BaseType == baseType && t.Name.Equals(editorAlias + "Controller", StringComparison.InvariantCultureIgnoreCase)));

            if (types.Any()) {
                var controllerType = types.First();

                var controllerInstance = (BlenderController)Activator.CreateInstance(controllerType);
                controllerInstance.ControllerContext = this.ControllerContext;

                var parts = frontView.Split(new char[] { '/', '\\' });
                var method = parts.Last().Split('.').First();

                var actionResult = (ViewResult)controllerType.GetMethod(method).Invoke(controllerInstance, new[] { model });

                actionResult.ViewName = frontView;
                
                return actionResult;
            }

            return View(frontView, model);
        }
    }
}