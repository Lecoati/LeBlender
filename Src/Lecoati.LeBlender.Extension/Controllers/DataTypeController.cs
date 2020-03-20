using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Editors;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Mvc;
using Umbraco.Web.PropertyEditors;

namespace Lecoati.LeBlender.Extension.Controllers
{
    [PluginController("LeBlenderApi")]
    public class DataTypeController : UmbracoAuthorizedJsonController
    {
        public DataTypeController( PropertyEditorCollection propertyEditors )
        {
            this.propertyEditors = propertyEditors;
        }

        private readonly PropertyEditorCollection propertyEditors;

        static readonly string[] editorsWithIdType =
        {
            "Umbraco.MediaPicker" ,
            "Umbraco.ContentPicker",
            "Umbraco.MultiNodeTreePicker",
            "Umbraco.MemberPicker",
            "Umbraco.MultiUrlPicker"
        };


        // Not allowed datatype because they don't make sense here
        static readonly string[] notAllowed = { "LeBlender", 
                                "Umbraco.ListView", 
                                "Umbraco.Grid", 
                                "Umbraco.FolderBrowser",
                                "Umbraco.UploadField", 
                                "Umbraco.ImageCropper" };

		// Get all datatypes
		public object GetAll()
        {
            var dataTypes = Services.DataTypeService.GetAll();
            return dataTypes
                .Where(r => !notAllowed.Contains(r.EditorAlias.ToString()))
                .OrderBy(r => r.Name)
                .Select(t => new { guid = t.Key, name = t.Name });
        }

        // Get property editor properties
        public object GetPropertyEditors(Guid guid)
        {

            var dataType = Services.DataTypeService.GetDataType(guid);
            if (dataType == null)
            {
				HttpResponseMessage response = new HttpResponseMessage( System.Net.HttpStatusCode.NotFound );
				response.Content = new StringContent( $"Datatype with Guid {guid} not found." );
				return response;
            }
            var dataTypeDisplay = Mapper.Map<IDataType, Umbraco.Web.Models.ContentEditing.DataTypeDisplay>(dataType);
            var propertyEditor = this.propertyEditors[dataTypeDisplay.SelectedEditor];

            object configuration = dataTypeDisplay.PreValues;
			// Quick Hack for a Bug in Umbraco, which doesn't deliver the "multiPicker" property in case of MultiNodeTreePicker.
            if (propertyEditor.Alias == "Umbraco.MultiNodeTreePicker")
            {
                var originConfig = dataType.Configuration as MultiNodePickerConfiguration;
                JArray jArr = JArray.FromObject(configuration);

                if (originConfig.MaxNumber != 1)
                {
                    jArr.Add(JObject.FromObject(new DataTypeConfigurationFieldDisplay
                    {
                        Key = "multiPicker",
                        Value = true
                    }));
                }
                              
                configuration = jArr;
            }

            if ( editorsWithIdType.Contains( propertyEditor.Alias ) )
            {
                JArray jArr = JArray.FromObject(configuration);
                jArr.Add( JObject.FromObject( new DataTypeConfigurationFieldDisplay
                {
                    Key = "idType",
                    Value = "udi"
                } ) );

                configuration = jArr;
            }

            return new { defaultPreValues = propertyEditor.DefaultConfiguration, alias = propertyEditor.Alias, view = propertyEditor.GetValueEditor().View, preValues = configuration};

        }

    }
}