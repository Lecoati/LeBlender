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
    public class BlenderValue
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

}