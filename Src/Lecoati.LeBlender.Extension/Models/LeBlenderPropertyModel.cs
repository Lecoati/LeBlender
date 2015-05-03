using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace Lecoati.LeBlender.Extension.Models
{
    public class LeBlenderPropertyModel
    {

        [JsonProperty("dataTypeGuid")]
        public String DataTypeGuid { get; set; }

        [JsonProperty("editorName")]
        public String Name { get; set; }

        [JsonProperty("editorAlias")]
        public String Alias { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        public T GetValue<T>()
        {

            var currentNode = Helper.GetCurrentContent();
            var targetDataType = GetTargetDataTypeDefinition(Guid.Parse(DataTypeGuid));

            var properyType = new PublishedPropertyType(currentNode.ContentType,
                new PropertyType(new DataTypeDefinition(-1, targetDataType.PropertyEditorAlias)
                {
                    Id = targetDataType.Id
                }));

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

        private static IDataTypeDefinition GetTargetDataTypeDefinition(Guid myId)
        {
            return (IDataTypeDefinition)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                "LeBlender_GetTargetDataTypeDefinition_" + myId,
                () => {
                    var services = ApplicationContext.Current.Services;
                    var dtd = services.DataTypeService.GetDataTypeDefinitionById(myId);
                    return dtd;
                });
        }

    }
}