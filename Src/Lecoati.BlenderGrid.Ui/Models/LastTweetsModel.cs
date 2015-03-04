using Lecoati.BlenderGrid.Extension.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lecoati.BlenderGrid.Ui.Models
{
    public class LastTweetsModel : BlenderModel
    {

        public IEnumerable<string> Tweets { get; set; }

    }
}