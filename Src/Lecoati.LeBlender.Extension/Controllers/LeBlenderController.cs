using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

using Lecoati.LeBlender.Extension.Models;
using Newtonsoft.Json;
using Umbraco.Web.Editors;
using Umbraco.Core.Logging;

namespace Lecoati.LeBlender.Extension.Controllers
{
    public class LeBlenderController : SurfaceController
    {
        [ChildActionOnly]
        public ActionResult RenderEditor(string editorAlias, string frontView, LeBlenderModel model)
        {
            // Check if the frontView is a custom path
            if (string.IsNullOrEmpty(frontView))
            {
                frontView = String.Format("~/views/partials/grid/editors/{0}.cshtml", Helper.FirstCharToUpper(editorAlias));
            }
            else if (frontView.IndexOf("/") < 0)
            {
                frontView = string.Format("~/Views/Partials/Grid/Editors/{0}.cshtml", frontView);
            }

            // Look for a custom controller
            var controllerType = Helper.GetLeBlenderController(editorAlias);
            if (controllerType != null)
            {
                try
                {
                    // Load a controller instance
                    var controllerInstance = (LeBlenderController)Activator.CreateInstance(controllerType);
                    controllerInstance.ControllerContext = this.ControllerContext;

                    // Take the view name as default method
                    var parts = frontView.Split(new char[] { '/', '\\' });
                    var method = parts.Last().Split('.').First();
                    var actionMethod = controllerType.GetMethod(method);

                    if (actionMethod == null)
                    {
                        method = "Index";
                        actionMethod = controllerType.GetMethod("Index");
                    }

                    if (actionMethod == null)
                        throw new Exception("No default method '" + method + "' was found");

                    // Set the specific model
                    var parameter = actionMethod.GetParameters().First();
                    var type = parameter.ParameterType.UnderlyingSystemType;
                    var typeInstance = (LeBlenderModel)Activator.CreateInstance(type);
                    typeInstance.Items = model.Items;

                    // Invoke the custom controller
                    var actionResult = (ViewResult)controllerType.GetMethod(method).Invoke(controllerInstance, new[] { typeInstance });

                    // Return the action result 
                    if (string.IsNullOrWhiteSpace(actionResult.ViewName))
                    {
                    	actionResult.ViewName = frontView;
                    }
                    return actionResult;
                }
                catch (Exception ex)
                {
                    LogHelper.Error<LeBlenderController>("Could not load LeBlender invoke the custom controller", ex);
                }

            }

            return PartialView(frontView, model);
            
        }
    }
}