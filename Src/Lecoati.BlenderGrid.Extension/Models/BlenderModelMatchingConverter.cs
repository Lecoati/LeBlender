using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lecoati.BlenderGrid.Extension.Models
{
    public class BlenderModelMatchingConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsClass;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            JObject jsonObject = JObject.Load(reader);
            var properties = jsonObject.Properties().ToList();

            IList<BlenderPropertyModel> bpml = new List<BlenderPropertyModel>();

            foreach (var property in properties)
            {
                if (property.Any())
                {
                    bpml.Add(JsonConvert.DeserializeObject<BlenderPropertyModel>(property.First().ToString()));
                }
            }

            return new BlenderValue
            {
                Properties = bpml,
            };

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }
}