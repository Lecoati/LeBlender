using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.ObjectResolution;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace Lecoati.LeBlender.Extension.Models
{
    public class LeBlenderPropertyModel
    {
        [JsonProperty("dataTypeGuid")]
        public String DataTypeGuid { get; set; }

        [JsonProperty("DataTypeName")]
        public String DataTypeName { get; set; }

        [JsonProperty("PropertyEditorAlias")]
        public String PropertyEditorAlias { get; set; }

        [JsonProperty("editorName")]
        public String Name { get; set; }

        [JsonProperty("editorAlias")]
        public String Alias { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        public T GetValue<T>()
        {
            IDataTypeDefinition idataTypeDefinition = null;

            if (!string.IsNullOrEmpty(this.DataTypeName))
            {
                idataTypeDefinition = Helper.GetTargetDataTypeDefinitionByDataTypeName(this.DataTypeName);
            }
            else if (!string.IsNullOrEmpty(this.PropertyEditorAlias))
            {
                idataTypeDefinition = Helper.GetTargetDataTypeDefinitionByPropertyAlias(this.PropertyEditorAlias);
            }
            else if (!string.IsNullOrEmpty(this.DataTypeGuid))
            {
                idataTypeDefinition = Helper.GetTargetDataTypeDefinition(Guid.Parse(this.DataTypeGuid));
            }

            PublishedContentType targetContentType = Helper.GetTargetContentType();
            var dataTypeDefinition = new DataTypeDefinition(idataTypeDefinition.PropertyEditorAlias);

            dataTypeDefinition.Id = idataTypeDefinition.Id;
            var propertyType = new PropertyType(dataTypeDefinition);

            PublishedPropertyType properyType = new PublishedPropertyType(targetContentType, propertyType);

            // Try Umbraco's PropertyValueConverters
            var converters = PropertyValueConvertersResolver.Current.Converters.ToArray();
            foreach (var converter in converters.Where(x => x.IsConverter(properyType)))
            {
                // Convert the type using a found value converter
                var value2 = converter.ConvertDataToSource(properyType, Value, false);

                // If the value is of type T, just return it
                if (value2 is T)
                    return (T)value2;

                // If ConvertDataToSource failed try ConvertSourceToObject.
                var value3 = converter.ConvertSourceToObject(properyType, value2, false);

                // If the value is of type T, just return it
                if (value3 is T)
                    return (T)value3;

                // Value is not final value type, so try a regular type conversion aswell
                var convertAttempt = value2.TryConvertTo<T>();
                if (convertAttempt.Success)
                    return convertAttempt.Result;
            }

            // if already the requested type, return
            if (Value is T) return (T)Value;

            // if can convert to requested type, return
            var convert = Value.TryConvertTo<T>();
            if (convert.Success) return convert.Result;

            return default(T);

        }
    }
}