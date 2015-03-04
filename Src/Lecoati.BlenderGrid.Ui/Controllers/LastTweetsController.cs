using Lecoati.BlenderGrid.Extension.Controllers;
using Lecoati.BlenderGrid.Extension.Models;
using Lecoati.BlenderGrid.Ui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lecoati.BlenderGrid.Ui.Controllers
{
    public class LastTweetsController : BlenderController
    {
        public ActionResult LastTweets(LastTweetsModel model) {

            model.Tweets = new List<string> {
                "Tweet 1",
                "Tweet 2",
                "Tweet 3"
            };

            return View(model);
        }
    }
}