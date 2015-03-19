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

namespace Lecoati.LeBlender.Extension.Controllers
{

    public class HelperController : SurfaceController
    {

        [ValidateInput(false)]
        [HttpPost]
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

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SaveEditorConfig()
        {

            umbraco.BusinessLogic.User u = umbraco.helper.GetCurrentUmbracoUser();

            if (u != null)
            {

                var config = Request["config"];
                var configPath = Request["configPath"];

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath(configPath)))
                {
                    file.Write(config);
                }

                return Json(new { Message = "Saved" }); ;

            }
            else
            {
                return new HttpUnauthorizedResult();
            }

        }

    }

}