using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecoati.LeBlender.Extension.Models.Manifest
{
    class Prevalue
    {

        [JsonProperty("fields")]
        public IEnumerable<Field> Fields { get; set; }

    }
}
