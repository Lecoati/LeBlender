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

			//This is a mock, where onle the EditorAlias is used in x.IsConverter(propertyType)
			var propertyType = new PublishedPropertyType( helper.GetTargetContentType(),
				new PropertyType( targetDataType, targetDataType.EditorAlias ), 
				new PropertyValueConverterCollection( new List<IPropertyValueConverter>() ), 
				new PublishedModelFactoryMock(),
				new PublishedContentTypeFactoryMock() );

            // Try Umbraco's PropertyValueConverters
            var converters = Current.Factory.GetInstance<PropertyValueConverterCollection>().ToArray();
            foreach (var converter in converters.Where(x => x.IsConverter(propertyType)))
            {
				// Since the ConvertDataToSource and ConvertSourceToObject methods don't exist anymore,
				// We skip the code and try to convert the Value property directly.

				//// Convert the type using a found value converter
				//var value2 = converter.ConvertDataToSource(propertyType, Value, false);

				//// If the value is of type T, just return it
				//if (value2 is T)
				//    return (T)value2;

				//// If ConvertDataToSource failed try ConvertSourceToObject.
				//var value3 = converter.ConvertSourceToObject(propertyType, value2, false);

				//// If the value is of type T, just return it
				//if (value3 is T)
				//    return (T)value3;

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

		class PublishedContentTypeFactoryMock : IPublishedContentTypeFactory
		{			
			public PublishedContentType CreateContentType( IContentTypeComposition contentType )
			{
				throw new NotImplementedException();
			}

			public PublishedPropertyType CreatePropertyType( PublishedContentType contentType, PropertyType propertyType )
			{
				throw new NotImplementedException();
			}

			public PublishedPropertyType CreatePropertyType( PublishedContentType contentType, string propertyTypeAlias, int dataTypeId, ContentVariation variations )
			{
				throw new NotImplementedException();
			}

			public PublishedDataType GetDataType( int id )
			{
				return Current.Factory.GetInstance<IPublishedContentTypeFactory>().GetDataType( id );
			}

			public void NotifyDataTypeChanges( int[] ids )
			{
				throw new NotImplementedException();
			}
		}

	}
}