using Lecoati.LeBlender.Extension.Controllers;
using Lecoati.LeBlender.Extension.Models;
using Lecoati.LeBlender.Ui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lecoati.LeBlender.Ui.Controllers
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