using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace Lecoati.LeBlender.Extension.Models
{
    [JsonConverter(typeof(LeBlenderModelMatchingConverter))]
    public class LeBlenderValue
    {
        internal IEnumerable<LeBlenderPropertyModel> Properties { get; set; }

        #region Helper Methods

        public string GetValue(string propertyAlias)
        {
            return GetValue<string>(propertyAlias);
        }

        public string GetRawValue(string propertyAlias)
        {
            var property = GetProperty(propertyAlias);

            if (IsEmptyProperty(property))
            {
                return string.Empty;
            }

            return property.Value.ToString();
        }

        public T GetValue<T>(string propertyAlias)
        {
            var property = GetProperty(propertyAlias);

            if (IsEmptyProperty(property))
            {
                return default(T);
            }

            if (property.Value is long)
            {
                int newValue;
                if (Int32.TryParse(property.Value.ToString(), out newValue)) {
                    property.Value = newValue;
                }
            }

            return property.GetValue<T>();
        }

        public bool HasProperty(string propertyAlias)
        {
            return GetProperty(propertyAlias) != null;
        }

        private bool IsEmptyProperty(LeBlenderPropertyModel property)
        {
            return (property == null || property.Value == null || string.IsNullOrEmpty(property.Value.ToString()));
        }

        private LeBlenderPropertyModel GetProperty(string propertyAlias)
        {
            return Properties.FirstOrDefault(p => p.Alias.ToLower().Equals(propertyAlias.ToLower()));
        }

        #endregion

    }

}