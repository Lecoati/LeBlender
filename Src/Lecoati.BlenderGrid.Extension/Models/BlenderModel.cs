using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace Lecoati.BlenderGrid.Extension.Models
{

    [JsonConverter(typeof(BlenderModelMatchingConverter))]
    public class BlenderModel
    {

        internal IEnumerable<BlenderPropertyModel> Properties { get; set; }

        #region Helper Methods

        public string GetValue(string propertyAlias)
        {
            return GetValue<string>(propertyAlias);
        }

        public T GetValue<T>(string propertyAlias)
        {
            var property = GetProperty(propertyAlias);

            if (IsEmptyProperty(property))
            {
                return default(T);
            }

            return property.GetValue<T>();
        }

        private bool IsEmptyProperty(BlenderPropertyModel property)
        {
            return (property == null || property.Value == null || string.IsNullOrEmpty(property.Value.ToString()));
        }

        private BlenderPropertyModel GetProperty(string propertyAlias)
        {
            return Properties.FirstOrDefault(p => p.Alias.ToLower() == propertyAlias.ToLower());
        }

        #endregion

    }

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
                if (property.Any()) {
                    bpml.Add(JsonConvert.DeserializeObject<BlenderPropertyModel>(property.First().ToString()));
                }
            }

            return new BlenderModel
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