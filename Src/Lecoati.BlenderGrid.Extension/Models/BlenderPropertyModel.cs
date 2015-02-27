using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace Lecoati.BlenderGrid.Extension.Models
{
    public class BlenderPropertyModel
    {

        [JsonProperty("editorName")]
        public String Name { get; set; }

        [JsonProperty("editorAlias")]
        public String Alias { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        public T GetValue<T>()
        {

            // Try Umbraco's PropertyValueConverters
            //var converters = UmbracoContext.Current != null ? 
            //    PropertyValueConvertersResolver.Current.Converters : Enumerable.Empty<IPropertyValueConverter>();
            //if (converters.Any())
            //{
            //    var convertedAttempt = TryConvertWithPropertyValueConverters<T>(Value, converters);
            //    if (convertedAttempt.Success)
            //        return convertedAttempt.Result;
            //}

            // If the value is of type T, just return it
            if (Value is T)
                return (T)Value;

            // No PropertyValueConverters matched, so try a regular type conversion
            var convertAttempt2 = Value.TryConvertTo<T>();
            if (convertAttempt2.Success)
                return convertAttempt2.Result;

            return default(T);
        }


    }
}