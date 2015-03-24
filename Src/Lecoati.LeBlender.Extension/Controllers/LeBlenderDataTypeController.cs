using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Editors;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.UI.Pages;

namespace Lecoati.leblender.Extension.Controllers
{
    [PluginController("LeBlenderApi")]
    public class LeBlenderDataTypeController : UmbracoAuthorizedJsonController
    {

        // Not allowed datatype because they don't make sense here
        String[] notAllowed = { "LeBlender", 
                                "Umbraco.ListView", 
                                "Umbraco.Grid", 
                                "Umbraco.FolderBrowser",
                                "Umbraco.UploadField", 
                                "Umbraco.ImageCropper" };

        // Get all datatypes
        public object GetAll()
        {
            var dataTypes = Services.DataTypeService.GetAllDataTypeDefinitions();
            return dataTypes
                .Where(r => !notAllowed.Contains(r.PropertyEditorAlias.ToString()))
                .OrderBy(r => r.Name)
                .Select(t => new { guid = t.Key, name = t.Name });
        }

        // Get property editor properties
        public object GetPropertyEditors(Guid guid)
        {

            var dataType = Services.DataTypeService.GetDataTypeDefinitionById(guid);
            if (dataType == null)
            {
                throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
            var dataTypeDisplay = AutoMapper.Mapper.Map<IDataTypeDefinition, Umbraco.Web.Models.ContentEditing.DataTypeDisplay>(dataType);
            var propertyEditor = global::Umbraco.Core.PropertyEditors.PropertyEditorResolver.Current.PropertyEditors.Where(r => r.Alias == dataTypeDisplay.SelectedEditor).First();

            return new { defaultPreValues = propertyEditor.DefaultPreValues, alias = propertyEditor.Alias, view = propertyEditor.ValueEditor.View, preValues = dataTypeDisplay.PreValues };

        }

    }
}