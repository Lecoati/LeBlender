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
using Umbraco.Web.WebApi;

namespace Lecoati.LeBlender.Extension.Controllers
{
    [PluginController("LeBlenderApi")]
    public class DataTypeController : UmbracoAuthorizedJsonController
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
                .Select(t => new { guid = t.Key, name = t.Name, DataTypeName = t.Name });
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

        public object GetPropertyEditors(string alias, Guid? guid)
        {
            if (string.IsNullOrEmpty(alias) && guid.HasValue)
                return this.GetPropertyEditors(guid.Value);

            var idataTypeDefinition = Services.DataTypeService.GetDataTypeDefinitionByPropertyEditorAlias(alias).FirstOrDefault();
            if (idataTypeDefinition == null && guid.HasValue)
            {
                return this.GetPropertyEditors(guid.Value);
            }
            var dataTypeDisplay = AutoMapper.Mapper.Map<IDataTypeDefinition, Umbraco.Web.Models.ContentEditing.DataTypeDisplay>(idataTypeDefinition);
            var propertyEditor = global::Umbraco.Core.PropertyEditors.PropertyEditorResolver.Current.PropertyEditors.Where(r => r.Alias == dataTypeDisplay.SelectedEditor).First();

            return new { defaultPreValues = propertyEditor.DefaultPreValues, alias = propertyEditor.Alias, view = propertyEditor.ValueEditor.View, preValues = dataTypeDisplay.PreValues };
        }

        public object GetPropertyEditors(string DataTypeName, string alias, Guid? guid)
        {
            if (string.IsNullOrEmpty(DataTypeName))
                return this.GetPropertyEditors(alias, guid);

            var definitionByName = Services.DataTypeService.GetDataTypeDefinitionByName(DataTypeName);
            if (definitionByName == null && alias != null)
            {
                return this.GetPropertyEditors(alias, guid);
            }
            var dataTypeDisplay = AutoMapper.Mapper.Map<IDataTypeDefinition, Umbraco.Web.Models.ContentEditing.DataTypeDisplay>(definitionByName);
            var propertyEditor = global::Umbraco.Core.PropertyEditors.PropertyEditorResolver.Current.PropertyEditors.Where(r => r.Alias == dataTypeDisplay.SelectedEditor).First();

            return new { defaultPreValues = propertyEditor.DefaultPreValues, alias = propertyEditor.Alias, view = propertyEditor.ValueEditor.View, preValues = dataTypeDisplay.PreValues };
        }

    }
}