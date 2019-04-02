using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;
using System.Collections;

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
			var helper = new Helper();
            //var targetContentType = Helper.GetTargetContentType();
            var targetDataType = helper.GetTargetDataTypeDefinition(Guid.Parse(DataTypeGuid));

			// This propertyType is a mock, where only the TargetDatatype.EditorAlias is used in x.IsConverter(propertyType)
			// This constructor is the one with minimal validity checks. See comment in PublishedPropertyType.cs.
			// But this may change in future, so be prepared to look into the Umbraco source code, if this code fails.
			var propertyType = new PublishedPropertyType( "pt-" + targetDataType.Id, targetDataType.Id, true, ContentVariation.Nothing, new PropertyValueConverterCollection( new IPropertyValueConverter[] { } ), new PublishedModelFactoryMock(), Current.PublishedContentTypeFactory );

			var converters = Current.Factory.GetInstance<PropertyValueConverterCollection>().ToArray();
			foreach (var converter in converters.Where( x => x.IsConverter( propertyType ) ))
			{
				// Since the ConvertDataToSource and ConvertSourceToObject methods don't exist anymore,
				// We skip the code and try to convert the Value property directly.

				// Value is not final value type, so try a regular type conversion aswell
				var convertAttempt = Value.TryConvertTo<T>();
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

		class PublishedModelFactoryMock : IPublishedModelFactory
		{
			public IPublishedElement CreateModel( IPublishedElement element )
			{
				throw new NotImplementedException();
			}

			public IList CreateModelList( string alias )
			{
				throw new NotImplementedException();
			}

			public Type MapModelType( Type type )
			{
				throw new NotImplementedException();
			}
		}		

	}
}