using Lecoati.LeBlender.Extension.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lecoati.LeBlender.Ui.Models
{
    public class LastTweetsModel : BlenderModel
    {

        public IEnumerable<string> Tweets { get; set; }

    }
}