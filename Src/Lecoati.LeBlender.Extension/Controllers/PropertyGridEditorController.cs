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
using Umbraco.Web.UI.Pages;

using System.Text.RegularExpressions;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Lecoati.LeBlender.Extension.Models.Manifest;

namespace Lecoati.LeBlender.Extension.Controllers
{

    [PluginController("LeBlenderApi")]
    public class PropertyGridEditorController : UmbracoAuthorizedJsonController
    {

        //private readonly DirectoryInfo _pluginsDir;
        private readonly IRuntimeCacheProvider _cache;

        //used to strip comments
        private static readonly Regex CommentsSurround = new Regex(@"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/", RegexOptions.Compiled);
        private static readonly Regex CommentsLine = new Regex(@"^\s*//.*?$", RegexOptions.Compiled | RegexOptions.Multiline);

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
            return packages.Concat(currDir.GetFiles("leblender.manifest")).Select(f => File.ReadAllText(f.FullName)).ToList();
        }

        internal static int FolderDepth(DirectoryInfo baseDir, DirectoryInfo currDir)
        {
            var removed = currDir.FullName.Remove(0, baseDir.FullName.Length).TrimStart('\\').TrimEnd('\\');
            return removed.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        internal static IEnumerable<PackageManifest> CreateManifests(params string[] manifestFileContents)
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
                    LogHelper.Error<Lecoati.LeBlender.Extension.Models.Manifest.PropertyEditor>("An error occurred parsing manifest with contents: " + m, ex);
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