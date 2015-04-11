using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lecoati.LeBlender.Extension.Models
{
    public class LeBlenderModelMatchingConverter : JsonConverter
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

            IList<LeBlenderPropertyModel> bpml = new List<LeBlenderPropertyModel>();

            foreach (var property in properties)
            {
                if (property.Any())
                {
                    bpml.Add(JsonConvert.DeserializeObject<LeBlenderPropertyModel>(property.First().ToString()));
                }
            }

            return new LeBlenderValue
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