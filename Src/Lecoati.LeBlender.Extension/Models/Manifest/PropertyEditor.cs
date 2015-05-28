using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecoati.LeBlender.Extension.Models.Manifest
{
    class PropertyEditor
    {

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("alias", Required = Required.Always)]
        public string Alias  { get; set; }

        [JsonProperty("isGridEditor")]
        public bool IsGridEditor { get; set; }

        [JsonProperty("editor", Required = Required.Always)]
        public Editor Editor { get; set; }

        [JsonProperty("prevalues")]
        public Prevalue Prevalues { get; set; }

    }
}
