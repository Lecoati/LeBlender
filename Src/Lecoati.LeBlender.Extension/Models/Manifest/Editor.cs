using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecoati.LeBlender.Extension.Models.Manifest
{
    class Editor
    {

        [JsonProperty("view", Required = Required.Always)]
        public string View { get; set; }

    }
}
