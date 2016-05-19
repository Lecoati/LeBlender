namespace Lecoati.LeBlender.Extension.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Courier.Core;
    using Umbraco.Courier.Core.Logging;
    using Umbraco.Courier.Core.ProviderModel;
    using Umbraco.Courier.DataResolvers.Helpers;
    using Umbraco.Courier.DataResolvers.PropertyDataResolvers;
    using Umbraco.Courier.ItemProviders;

    /// <summary>
    /// Custom implementation of Courier's GridCellResolverProvider for LeBlender controls in a grid
    /// 2016-05-19 Heather Floyd, Scandia Consulting
    /// </summary>
    public class LeBlenderGridDataResolver : GridCellResolverProvider
    {
        private enum Direction
        {
            Extracting,
            Packaging
        }

        /// <summary>
        /// Gets the editor alias.
        /// </summary>
        /// <value>
        /// The editor alias.
        /// </value>
        public override string EditorAlias
        {
            get { return "Umbraco.Grid"; }
        }

        /// <summary>
        /// Checks to see whether the widget is a LeBlender Widget and should be processed. If TRUE, PackagingCell() & ExtractingCell() will run
        /// </summary>
        /// <param name="view"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        public override bool ShouldRun(string view, GridValueControlModel cell)
        {
            try
            {
                var editorAlias = cell.Editor.Alias;
                //CourierLogHelper.Info<LeBlenderGridDataResolver>("LeBlenderGridDataResolver.ShouldRun is RUNNING! " + view + " : " + editorAlias + "[VIEW = " + view + "]");

                if (view.Contains("leblender"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                // ignored
                CourierLogHelper.Error<LeBlenderGridDataResolver>("NON-CRITICAL: LeBlenderGridDataResolver.ShouldRun had an error", ex);
                return false;
            }

            return false;
            //return true;
        }

        public override void PackagingCell(Item item, ContentProperty propertyData, GridValueControlModel cell)
        {
            //CourierLogHelper.Info<LeBlenderGridDataResolver>("LeBlenderGridDataResolver.PackagingCell is RUNNING! " + item.Name + " : " + cell.Editor.Alias);
            ProcessCell(item, propertyData, cell, Direction.Packaging);

        }

        public override void ExtractingCell(Item item, ContentProperty propertyData, GridValueControlModel cell)
        {
            //CourierLogHelper.Info<LeBlenderGridDataResolver>("LeBlenderGridDataResolver.ExtractingCell is RUNNING! " + item.Name + " : " + cell.Editor.Alias);
            ProcessCell(item, propertyData, cell, Direction.Extracting);
        }

        private void ProcessCell(Item item, ContentProperty propertyData, GridValueControlModel cell, Direction direction)
        {
            //Some services we need to use
            var dataTypeService = ApplicationContext.Current.Services.DataTypeService;
            var propertyItemProvider = ItemProviderCollection.Instance.GetProvider(ItemProviderIds.propertyDataItemProviderGuid, ExecutionContext);
            GridHelper gridHelper = new GridHelper();

            var editorAlias = cell.Editor.Alias;

            var originalCellValue = cell.Value;  //for reference
            var cellValue = cell.Value;

            CourierLogHelper.Info<LeBlenderGridDataResolver>("START ProcessCell (" + direction + ") " + editorAlias);
            CourierLogHelper.Info<LeBlenderGridDataResolver>("*********************** Original cell.Value:");
            var logPropDataStart = GetLogValue(cell.Value);
            CourierLogHelper.Info<LeBlenderGridDataResolver>(logPropDataStart);

            if (cellValue == null)
            {
                return;
            }
            
            //cellValue is a JArray, but it generally only has 1 widget (jItem) included in it - even if the actual row/column in the grid has more than 1 widget.
            JArray newJArray = new JArray();
            foreach (var jItem in cellValue)
            {
                //Get the LeBlender widget with its properties to work with
                var leblenderWidget = new LeBlenderGridWidget(editorAlias, jItem);

                //Something to hold all the final widget properties
                //var finalWidgetProps = new Dictionary<string, object>();

                //We need to update the original jItem JSON with new resolved values
                StringBuilder newJItemSB = new StringBuilder();

                //start with a comma so we can remove it later...
                //newJItemSB.Append(",");

                //Each widget has properties which are Umbraco Datatypes
                foreach (var prop in leblenderWidget.Properties)
                {
                    //If there is no data for the property, skip it
                    var propValue = prop.Value;
                    if (propValue == null)
                        continue;

                    //Get info about the Property's Datatypes
                    var umbracoDatatype = dataTypeService.GetDataTypeDefinitionById(new Guid(prop.DataTypeGuid));
                  
                    //We will pretend that each widget property is it's own Doctype property, so we can run the resolvers which have already been created for each Datatype (Check the CourierLog for info on this process)
                    //(This is how Nested Content's Data Resolver works...)
                    var fakeItem = new ContentPropertyData
                    {
                        ItemId = item.ItemId,
                        Name = string.Format("{0} [{1}: LeBlender Widget {2} (Property: {3})]", item.Name, EditorAlias, editorAlias, prop.Alias),
                        Data = new List<ContentProperty>
                            {
                                new ContentProperty
                                {
                                    Alias = prop.Alias,
                                    DataType = new Guid(prop.DataTypeGuid), //courierDatatype.UniqueID,
                                    PropertyEditorAlias = umbracoDatatype.PropertyEditorAlias, //courierDatatype.PropertyEditorAlias,
                                    Value = propValue
                                }
                            }
                    };

                    if (direction == Direction.Packaging)
                    {
                        try
                        {
                            // run the 'fake' item through Courier's data resolvers
                            ResolutionManager.Instance.PackagingItem(fakeItem, propertyItemProvider);
                        }
                        catch (Exception ex)
                        {
                            CourierLogHelper.Error<LeBlenderGridDataResolver>(string.Concat("Error packaging data value: ", fakeItem.Name), ex);
                        }
                    }
                    else if (direction == Direction.Extracting)
                    {
                        try
                        {
                            // run the 'fake' item through Courier's data resolvers
                            ResolutionManager.Instance.ExtractingItem(fakeItem, propertyItemProvider);
                        }
                        catch (Exception ex)
                        {
                            CourierLogHelper.Error<LeBlenderGridDataResolver>(string.Concat("Error extracting data value: ", fakeItem.Name), ex);
                        }
                    }

                    // Gather the dependencies and resources from the Data Resolvers
                    item.Dependencies.AddRange(fakeItem.Dependencies);
                    item.Resources.AddRange(fakeItem.Resources);
                    
                    //loop through transformed properties and format correct JSON string
                    if (fakeItem.Data != null && fakeItem.Data.Any())
                    {
                        var fakeItemPropData = fakeItem.Data;
                        
                        //string serializedNewPropertyData = JsonConvert.SerializeObject(fakeItemPropData);
                        //CourierLogHelper.Info<LeBlenderGridDataResolver>("SERIALIZING: (" + fakeItem.Name + ") serializedNewPropertyData= " + serializedNewPropertyData);

                        //var newjItem = JsonConvert.SerializeObject(fakeItemPropData);
                        //CourierLogHelper.Info<LeBlenderGridDataResolver>("SERIALIZING: (" + fakeItem.Name + ") newjItem= " + newjItem);

                        string serializedNewValue = fakeItem.Data[0].Value as string ?? JsonConvert.SerializeObject(fakeItem.Data[0].Value);
                        //CourierLogHelper.Info<LeBlenderGridDataResolver>("SERIALIZING: (" + fakeItem.Name + ") serializedNewValue= " + serializedNewValue);
                        
                        //We need to preserve "empty string" values from the later cleanup process... 
                        serializedNewValue = serializedNewValue == "" ? "~~EMPTY~~" : serializedNewValue;

                        //var widgetInfo = fakeItem.Data as LeBlenderGridWidget;
                        newJItemSB.AppendFormat("  \"{0}\": {{", prop.Alias);
                        newJItemSB.AppendFormat("   \"value\": \"{0}\",", serializedNewValue);
                        newJItemSB.AppendFormat("   \"dataTypeGuid\": \"{0}\",", prop.DataTypeGuid);        
                        newJItemSB.AppendFormat("   \"editorAlias\": \"{0}\",", prop.Alias);
                        newJItemSB.AppendFormat("   \"editorName\": \"{0}\"", prop.Name);
                        newJItemSB.Append("},");
                    }                   
                }

                //fix up string format
                var newJItemString = "{" + newJItemSB.ToString() + "}";

                CourierLogHelper.Info<LeBlenderGridDataResolver>("SERIALIZING + STARTING CLEANUP: (" + leblenderWidget.WidgetName + ") newJItemString= " + newJItemString);
                newJItemString = newJItemString.Replace("},}", "}}");
                newJItemString = newJItemString.Replace("\"[", "[");
                newJItemString = newJItemString.Replace("]\"", "]");
                newJItemString = newJItemString.Replace("\"\"", "\"");
                newJItemString = newJItemString.Replace("\"null\"", "null");
                newJItemString = newJItemString.Replace("\"~~EMPTY~~\"", "\"\"");
               

                CourierLogHelper.Info<LeBlenderGridDataResolver>("SERIALIZING + CLEANUP COMPLETE: (" + leblenderWidget.WidgetName + ") newJItemString= " + newJItemString);

                var newJToken = JToken.Parse(newJItemString);
                //add jItem widget back into Array so the data is formatted the same as it came in
                newJArray.Add(newJToken);

                //Update the original cell data so that it gets transferred in its "fixed" state
                //var serializedFinal = JsonConvert.SerializeObject(finalPropValues);
                cell.Value = newJArray;

                CourierLogHelper.Info<LeBlenderGridDataResolver>("FINISH ProcessCell (" + direction + ") " + editorAlias);
                CourierLogHelper.Info<LeBlenderGridDataResolver>("*********************** Final cell.Value:");
                var logPropDataEnd = GetLogValue(cell.Value);
                CourierLogHelper.Info<LeBlenderGridDataResolver>(logPropDataEnd.ToString());
            }
        }

        private string GetLogValue(object ObjectValue)
        {
            string logValue;
            try
            {
                logValue = JsonConvert.SerializeObject(ObjectValue).ToString();
            }
            catch
            {
                logValue = ObjectValue.ToString();
            }

            //logValue = logValue.Replace("\"", "'");
            logValue = logValue.Replace("{", "{{");
            logValue = logValue.Replace("}", "}}");
            return logValue;
        }

    }

    public class LeBlenderGridWidget
    {
        public string WidgetName { get; set; }
        public IEnumerable<Lecoati.LeBlender.Extension.Models.LeBlenderPropertyModel> Properties { get; set; }

        public LeBlenderGridWidget(string WidgetName, dynamic Model)
        {
            this.WidgetName = WidgetName;
            var props = new List<Lecoati.LeBlender.Extension.Models.LeBlenderPropertyModel>();

            try
            {
                var data = JsonConvert.DeserializeObject(Model.ToString());

                foreach (var item in data)
                {
                    var lbPropModel = new Lecoati.LeBlender.Extension.Models.LeBlenderPropertyModel();
                    lbPropModel.Name = item.Name;
                    lbPropModel.Alias = item.Value.editorAlias;
                    lbPropModel.DataTypeGuid = item.Value.dataTypeGuid;
                    lbPropModel.Value = item.Value.value;

                    props.Add(lbPropModel);
                }
            }
            catch (Exception ex)
            {
                CourierLogHelper.Error<LeBlenderGridWidget>(WidgetName + ": Error while converting a dynamic property model to a new LeBlenderGridWidget model.", ex);
                LogHelper.Error<LeBlenderGridWidget>(WidgetName + ": Error while converting a dynamic property model to a new LeBlenderGridWidget model.", ex);
            }

            Properties = props;
        }
    }

}