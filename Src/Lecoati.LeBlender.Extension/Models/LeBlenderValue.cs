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

        public T GetValue<T>(string propertyAlias)
        {
            var property = GetProperty(propertyAlias);

            if (IsEmptyProperty(property))
            {
                return default(T);
            }

            return property.GetValue<T>();
        }

        private bool IsEmptyProperty(LeBlenderPropertyModel property)
        {
            return (property == null || property.Value == null || string.IsNullOrEmpty(property.Value.ToString()));
        }

        private LeBlenderPropertyModel GetProperty(string propertyAlias)
        {
            return Properties.FirstOrDefault(p => p.Alias.ToLower() == propertyAlias.ToLower());
        }

        #endregion

    }

}