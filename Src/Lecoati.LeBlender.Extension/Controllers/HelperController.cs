using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Editors;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.UI.Pages;

namespace Lecoati.LeBlender.Extension.Controllers
{

    public class HelperController : UmbracoAuthorizedController
    {
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GetPartialViewResultAsHtmlForEditor()
        {
            var modelStr = Request["model"];
            var view = Request["view"];
            dynamic model = JsonConvert.DeserializeObject(modelStr);
            return View("/views/Partials/" + view + ".cshtml", model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SaveEditorConfig()
        {
            var config = Request["config"];
            var configPath = Request["configPath"];

            // Update
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath(configPath)))
            {
                file.Write(config);
            }

            // Refrech GridConfig for next use
            HttpContext.Cache.Remove("LeBlenderControllers");
            HttpContext.Cache.Remove("LeBlenderGridEditorsList");
            ApplicationContext.ApplicationCache.RuntimeCache.ClearCacheByKeySearch("LEBLENDEREDITOR");
            ApplicationContext.ApplicationCache.RuntimeCache.ClearCacheItem(typeof(BackOfficeController) + "GetGridConfig");

            return Json(new { Message = "Saved" });
        }

    }

}