using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Cache;
using Umbraco.Web;
using Umbraco.Web.Editors;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

using System.Text.RegularExpressions;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Lecoati.LeBlender.Extension.Models.Manifest;
using Umbraco.Core.Models;
using Umbraco.Web.PropertyEditors;
using Umbraco.Web.Models.ContentEditing;
using IO = System.IO;

namespace Lecoati.LeBlender.Extension.Controllers
{

    [PluginController("LeBlenderApi")]
    public class PropertyGridEditorController : UmbracoAuthorizedJsonController
    {
		public PropertyGridEditorController( ILogger logger, PropertyEditorCollection propertyEditors )
		{
			this.logger = logger;
            this.propertyEditors = propertyEditors;
        }

		//used to strip comments
		private static readonly Regex CommentsSurround = new Regex(@"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/", RegexOptions.Compiled);
        private static readonly Regex CommentsLine = new Regex(@"^\s*//.*?$", RegexOptions.Compiled | RegexOptions.Multiline);
		private readonly ILogger logger;
        private readonly PropertyEditorCollection propertyEditors;

        // Get all datatypes
        public object GetAll()
        {
            var editors = new List<Lecoati.LeBlender.Extension.Models.Manifest.PropertyEditor>();
            foreach (var manifest in GetManifests())
            {
                if (manifest.PropertyEditors != null)
                {
                    editors.AddRange(GetPropertyGridEditor(manifest.PropertyEditors).Where(r => r.IsGridEditor));
                }
            }
            return editors;
        }


        internal static IEnumerable<Lecoati.LeBlender.Extension.Models.Manifest.PropertyEditor> GetPropertyGridEditor(JArray jsonEditors)
        {
            return JsonConvert.DeserializeObject<IEnumerable<Lecoati.LeBlender.Extension.Models.Manifest.PropertyEditor>>(jsonEditors.ToString());
        }

        internal IEnumerable<PackageManifest> GetManifests()
        {
            var plugins = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/App_Plugins"));

            var manifestFileContents = GetAllManifestFileContents(plugins);
            return CreateManifests(manifestFileContents.ToArray());

        }

        object GetDataTypeConfig( Guid dataTypeKey )
        {

            var dataType = Services.DataTypeService.GetDataType(dataTypeKey);
            if (dataType == null)
            {
                return new object[0];
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
                    jArr.Add( JObject.FromObject( new DataTypeConfigurationFieldDisplay
                    {
                        Key = "multiPicker",
                        Value = true
                    } ) );
                }

                configuration = jArr;
            }

            return new { view = propertyEditor.GetValueEditor().View, config = configuration };

        }

        [System.Web.Http.HttpGet]
        public object GetConfigForElementType(string key)
        {
            Guid guidKey;
            if (!Guid.TryParse( key, out guidKey ))
                throw new Exception( $"Guid expected: {key}" );
            List<object> result = new List<object>();
            var contentTypeService = Services.ContentTypeService;
            var dataTypeService = Services.DataTypeService;

            var contentType = contentTypeService.Get(guidKey);
            if (contentType == null)
            {
                logger.Error( GetType(), null, $"Content type {guidKey} not found" );
                return result;
            }

            foreach (var prop in contentType.CompositionPropertyTypes)
            {
                /*
                {
	                "config": {
		                "showOpenButton": false,
		                "startNodeId": "umb://document/4589c61e907e4203b83b0de1beff1a08",
		                "ignoreUserStartNodes": false
	                },
	                "view": "views/propertyeditors/contentpicker/contentpicker.html"
                }
                 */
                
                object propretyType = GetDataTypeConfig( prop.DataTypeKey );

                result.Add(
                new
                {
                    name = prop.Name,
                    alias = prop.Alias,
                    dataType = prop.DataTypeKey,
                    description = prop.Description,
                    propretyType
                } );
            }

            return result;
        }

        private IEnumerable<string> GetAllManifestFileContents(DirectoryInfo currDir)
        {

            DirectoryInfo _pluginsDir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/App_Plugins"));

            var depth = FolderDepth(_pluginsDir, currDir);

            if (depth < 1)
            {
                var dirs = currDir.GetDirectories();
                var result = new List<string>();
                foreach (var d in dirs)
                {
                    result.AddRange(GetAllManifestFileContents(d));
                }
                return result;
            }

            FileInfo[] packages = currDir.GetFiles("package.manifest");
            return packages.Concat(currDir.GetFiles("leblender.manifest")).Select(f => IO.File.ReadAllText(f.FullName)).ToList();
        }

        internal static int FolderDepth(DirectoryInfo baseDir, DirectoryInfo currDir)
        {
            var removed = currDir.FullName.Remove(0, baseDir.FullName.Length).TrimStart('\\').TrimEnd('\\');
            return removed.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        internal IEnumerable<PackageManifest> CreateManifests(params string[] manifestFileContents)
        {
            var result = new List<PackageManifest>();
            foreach (var m in manifestFileContents)
            {

                if (string.IsNullOrEmpty(m)) continue;

                //remove any comments first
                var replaced = CommentsSurround.Replace(m, match => " ");
                replaced = CommentsLine.Replace(replaced, match => "");

                JObject deserialized;
                try
                {
                    deserialized = JsonConvert.DeserializeObject<JObject>(replaced);
                }
                catch (Exception ex)
                {
                    this.logger.Error<Lecoati.LeBlender.Extension.Models.Manifest.PropertyEditor>("An error occurred parsing manifest with contents: " + m, ex);
                    continue;
                }

                // validate the grid editor configs section
                if (deserialized != null)
                {
                    var propEditors = deserialized.Properties().Where(x => x.Name == "propertyEditors").ToArray();
                    if (propEditors.Length > 1)
                    {
                        throw new FormatException("The manifest is not formatted correctly contains more than one 'gridEditorConfigs' element");
                    }

                    var manifest = new PackageManifest()
                    {
                        PropertyEditors = propEditors.Any() ? (JArray)deserialized["propertyEditors"] : new JArray(),
                    };
                    result.Add(manifest);
                }

            }
            return result;
        }

    }

}