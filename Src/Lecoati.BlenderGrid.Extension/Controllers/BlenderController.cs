using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

using Lecoati.BlenderGrid.Extension.Models;
using Newtonsoft.Json;

namespace Lecoati.BlenderGrid.Extension.Controllers
{
    public class BlenderController : SurfaceController
    {
        [ChildActionOnly]
        public ActionResult RenderEditor(string editorAlias, string frontView, dynamic model)
        {
            BlenderModel blenderModel = JsonConvert.DeserializeObject<BlenderModel>(model.ToString());

            var baseType = typeof(BlenderController);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(a => a.GetTypes().Where(t => t.BaseType == baseType && t.Name.Equals(editorAlias + "Controller", StringComparison.InvariantCultureIgnoreCase)));

            if (types.Any()) {
                var controllerType = types.First();

                var controllerInstance = (BlenderController)Activator.CreateInstance(controllerType);
                controllerInstance.ControllerContext = this.ControllerContext;

                var parts = frontView.Split(new char[] { '/', '\\' });
                var method = parts.Last().Split('.').First();

                var actionMethod = controllerType.GetMethod(method);
                var parameter = actionMethod.GetParameters().First();

                var type = parameter.ParameterType.UnderlyingSystemType;
                var typeInstance = (BlenderModel)Activator.CreateInstance(type);

                typeInstance.Items = blenderModel.Items;

                var actionResult = (ViewResult)controllerType.GetMethod(method).Invoke(controllerInstance, new[] { typeInstance });

                actionResult.ViewName = frontView;
                
                return actionResult;
            }

            return View(frontView, blenderModel);
        }
    }
}