using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

using Lecoati.LeBlender.Extension.Models;
using Newtonsoft.Json;

namespace Lecoati.LeBlender.Extension.Controllers
{
    public class BlenderController : SurfaceController
    {
        [ChildActionOnly]
        public ActionResult RenderEditor(string editorAlias, string frontView, BlenderModel model)
        {

            // TODO: This stuff have to done on app start and cached
            var baseType = typeof(BlenderController);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(a => a.GetTypes().Where(t => t.BaseType == baseType && t.Name.Equals(editorAlias + "Controller", StringComparison.InvariantCultureIgnoreCase)));

            // Check if the frontView is a custom path
            if (frontView.IndexOf("/") < 0)
            {
                frontView = string.Format("/Views/Partials/Grid/Editors/{0}.cshtml", frontView);
            }

            // If custom controller was found
            if (types.Any()) {

                // Load a controller instance
                var controllerType = types.First();
                var controllerInstance = (BlenderController)Activator.CreateInstance(controllerType);
                controllerInstance.ControllerContext = this.ControllerContext;

                // Take the view name as default method
                var parts = frontView.Split(new char[] { '/', '\\' });
                var method = parts.Last().Split('.').First();
                var actionMethod = controllerType.GetMethod(method);
                if (actionMethod == null)
                    throw new Exception("No default method '" + method + "' was found");

                // Set the specific model
                var parameter = actionMethod.GetParameters().First();
                var type = parameter.ParameterType.UnderlyingSystemType;
                var typeInstance = (BlenderModel)Activator.CreateInstance(type);
                typeInstance.Items = model.Items;

                // Invoke the custom controller
                var actionResult = (ViewResult)controllerType.GetMethod(method).Invoke(controllerInstance, new[] { typeInstance });

                // Return the action result 
                actionResult.ViewName = frontView;
                return actionResult;

            }

            return View(frontView, model);
            
        }
    }
}