
using MCPSharp;
using System.ComponentModel;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Service;
using Weavers.Core.Extensions;

namespace Weavers.Core.Tools {
  public class AppGraphEntityTools {
    private static IAppGraphEntityToolsHandler GetTools() => DiBridgeService.GetService<IAppGraphEntityToolsHandler>();

    [McpTool(Cx.CmdAddEntityClass, "Adds a new entity class model, with a config model and primary Id entity property model. adds import ref to dbContext.")]
    public static Task<string> AddEntityClassModel(
      [Description("The Item Id of the parent item (either Library or Namespace type Models) to add the new entity class model.")] int parentItemId, 
      [Description("The name of the new entity class model. This is normally a Singular named class.")] string className, 
      [Description("The name of the database table for the new entity class model. This is normally a pluralized form of the class name.")] string entityDbTableName
    ) {
      return GetTools().AddEntityClassModel(parentItemId, className, entityDbTableName);
    }


    [McpTool(Cx.CmdAddEntityProperty, "Adds a new entity property model to an existing entity class. if it is a navigation property, additional navigation properties will be added, they will need to be configured.")]
    public static Task<string> AddEntityPropertyModel(
      [Description("The Item Id of entity class model to add the new property model.")] 
      int entityClassId, 
      [Description("The name of the new property to add.")] 
      string propertyName, 
      [Description($"The type Id of the property. Main ones: string 54, int 57, long 58; full list of types see {Cx.CmdGetTypeDetails} using ItemTypeId 50 for CSharpTypes. ")] 
      int? propertyTypeId, 
      [Description("Indicates if the property is a ef core navigation property. If so, it will add an additional navigation Item off the new property model.")] 
      bool isNav,
      [Description("if it is a navigation property, the Item Id of the related entity class model for the new navigation properties.")] 
      int? navEntityClassId) {
      return GetTools().AddEntityPropertyModel(entityClassId, propertyName, propertyTypeId, isNav, navEntityClassId);
    }

  }
}
