using System;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace Lecoati.LeBlender.Extension.Controllers
{
    [PluginController("LeBlenderApi")]
    public class DataTypeController : UmbracoAuthorizedJsonController
    {
		public DataTypeController( PropertyEditorCollection propertyEditors )
		{
			this.propertyEditors = propertyEditors;
		}

        // Not allowed datatype because they don't make sense here
        String[] notAllowed = { "LeBlender", 
                                "Umbraco.ListView", 
                                "Umbraco.Grid", 
                                "Umbraco.FolderBrowser",
                                "Umbraco.UploadField", 
                                "Umbraco.ImageCropper" };
		private readonly PropertyEditorCollection propertyEditors;

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
                throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
            var dataTypeDisplay = AutoMapper.Mapper.Map<IDataType, Umbraco.Web.Models.ContentEditing.DataTypeDisplay>(dataType);
            var propertyEditor = this.propertyEditors[dataTypeDisplay.SelectedEditor];

            return new { defaultPreValues = propertyEditor.DefaultConfiguration, alias = propertyEditor.Alias, view = propertyEditor.GetValueEditor().View, preValues = dataTypeDisplay.PreValues };

        }

    }
}