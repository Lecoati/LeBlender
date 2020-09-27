//clear cache on publish
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using System.Web;
using System.IO;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Services;
using Umbraco.Core.Models;
using Newtonsoft.Json;

namespace Lecoati.LeBlender.Extension.Events
{
    public class ApplicationInitializer : IComponent
    {
		private readonly ILogger logger;
		private readonly IContentTypeService contentTypeService;
		private readonly Guid containerId = new Guid("30404f24-ff1c-47ea-ab3e-ecb9b72d2602");
		private static readonly string configName = "~/Config/grid.editors.config.js";
		List<IDataType> dataTypeDefinitions;

		public ApplicationInitializer(ILogger logger, IContentTypeService contentTypeService, IDataTypeService dataTypeService)
		{
			this.logger = logger;
			this.contentTypeService = contentTypeService;
			this.dataTypeDefinitions = dataTypeService.GetAll().ToList();
		}

		public void Initialize()
		{
			JToken editors;
			editors = ReadEditors();
			List<string> errors = new List<string>();

			foreach (var editor in editors)
			{
				try
				{
					var config = editor["config"];
					var alias = (string)editor["alias"];
					var name = (string)editor["name"];
					if (config != null && editor["render"] != null && ( (string) editor["render"] ).IndexOf( "Base.cshtml" ) > -1)
					{
						if (config["documentType"] != null || config["editors"] == null)
							continue;
					}
					else
					{
						continue;
					}
					var container = GetLeblenderContainer();
					var ctalias = "leblender" + alias;
					var contentType = contentTypeService.Get(ctalias);
					if (contentType == null)
					{
						contentType = new ContentType( container.Id );
						contentType.Alias = ctalias;
					}
					contentType.Name = name;
					contentType.Icon = (string) editor["icon"];
					contentType.IsElement = true;

					int sortOrder = 1;
					foreach (var propertyEditor in config["editors"])
					{
						var pnName = (string)propertyEditor["name"];
						var pnAlias = (string)propertyEditor["alias"];
						var dataTypeKey = new Guid((string)propertyEditor["dataType"]);
						var description = (string)propertyEditor["description"];
						CreateProperty( contentType, pnName, pnAlias, dataTypeKey, description, sortOrder++ );
					}

					contentTypeService.Save( contentType );

					config["documentType"] = contentType.Key;
					config["editors"] = null;
				}
				catch (Exception ex)
				{
					errors.Add( ex.ToString() );
				}
			}

			WriteEditors( editors );

			if (errors.Count > 0)
			{
				var msg = String.Join("\r\n", errors);
				logger.Error( GetType(), msg, null );
			}

		}

		public void CreateProperty( IContentType contentType, string propertyName, string alias, Guid dataTypeKey, string description, int sortOrder)//, bool mandatory, string validationRegExp, string dataTypeName, string tabName, ICreateContext createContext, int? sortOrder, bool saveImmediately = true )
		{
			IDataType dt = dataTypeDefinitions.Where( dtDef => dtDef.Key == dataTypeKey ).FirstOrDefault();
			bool unknownDatatype = false;
			if (dt == null)
			{
				// Let's add the property as a text field, so that we can alter it later in the content type
				dt = dataTypeDefinitions.Where( dtDef => dtDef.Key == new Guid( "0cc0eba1-9960-42c9-bf9b-60e150b429ae" ) ).FirstOrDefault();
				logger.Error(GetType(), null, $"CreateProperty: DataType {dataTypeKey} doesn't exist" );
				unknownDatatype = true;
			}

			var pt = contentType.CompositionPropertyTypes.FirstOrDefault(p=>p.Alias == alias);
			bool isNew = false;
			if (pt == null)
			{
				pt = new PropertyType( dt );
				isNew = true;
			}

			pt.Name = propertyName;
			pt.Alias = alias;
			pt.Mandatory = false;
			pt.Description = description;
			if (unknownDatatype)
				pt.Description = $"{description} / Unknown datatype: {dataTypeKey}";
			pt.SortOrder = sortOrder;
			if (isNew)
				contentType.AddPropertyType( pt, "Data" );
		}

		private EntityContainer GetLeblenderContainer()
		{
			var container = contentTypeService.GetContainers( "LeBlender", 1 ).FirstOrDefault();
			
			if (container == null)
			{
				container = new EntityContainer( Umbraco.Core.Constants.ObjectTypes.DocumentType );
				container.ParentId = -1;
				container.Name = "LeBlender";
				container.Key = containerId;
				contentTypeService.SaveContainer( container );
			}

			return container;
		}

		private static JToken ReadEditors()
		{
			JToken editors;
			var fileName = HttpContext.Current.Server.MapPath( configName );
			
			if (!System.IO.File.Exists( fileName ))
				return new JArray();

			string json = "";
			using (StreamReader sr = new StreamReader( fileName ))
				json = sr.ReadToEnd();
			editors = JsonConvert.DeserializeObject<JArray>(json);
			return editors;
		}

		private static void WriteEditors(JToken editors)
		{
			var json = editors.ToString();
			var fileName = HttpContext.Current.Server.MapPath( configName );
			using (StreamWriter sw = new StreamWriter( fileName ))
				sw.Write( json );
		}

		public void Terminate()
		{
		}
    }
}