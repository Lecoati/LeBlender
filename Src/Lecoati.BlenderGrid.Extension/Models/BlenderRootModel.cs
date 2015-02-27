using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lecoati.BlenderGrid.Extension.Models
{

    [JsonObject]
    public class BlenderRootModel : IEnumerable<BlenderModel>
    {

        [JsonProperty("value")]
        public IEnumerable<BlenderModel> Items { get; set; }

        public IEnumerator<BlenderModel> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    
    }   

}