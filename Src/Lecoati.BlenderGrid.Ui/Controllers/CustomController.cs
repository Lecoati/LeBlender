using Lecoati.BlenderGrid.Extension.Controllers;
using Lecoati.BlenderGrid.Extension.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lecoati.BlenderGrid.Ui.Controllers
{
    public class CustomController : BlenderController
    {
        public ActionResult TestBlender(BlenderModel model)
        {
            return View(model);
        }
    }
}