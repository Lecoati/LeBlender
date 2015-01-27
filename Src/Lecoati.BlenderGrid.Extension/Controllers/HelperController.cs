using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.UI.Pages;

namespace Lecoati.BlenderGrid.Extension.Controllers
{

    public class HelperController : SurfaceController
    {

        [ValidateInput(false)]
        public ActionResult GetPartialViewResultAsHtmlForEditor()
        {

            umbraco.BusinessLogic.User u = umbraco.helper.GetCurrentUmbracoUser();

            if (u != null)
            {
                var modelStr = Request["model"];
                var view = Request["view"];
                dynamic model = JsonConvert.DeserializeObject(modelStr);
                return View("/views/Partials/" + view + ".cshtml", model);
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

    }

}