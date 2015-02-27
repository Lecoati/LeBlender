using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Lecoati.BlenderGrid.Extension.Controllers
{
    public class BlenderController : SurfaceController
    {
        [ChildActionOnly]
        public ActionResult RenderEditor(string editor)
        {
            return View(editor);
        }
    }
}