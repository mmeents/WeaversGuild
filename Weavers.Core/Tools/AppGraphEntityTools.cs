
using MCPSharp;
using System.ComponentModel;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Service;
using Weavers.Core.Extensions;

namespace Weavers.Core.Tools {
  public class AppGraphEntityTools {
    private static IAppGraphEntityToolsHandler GetTools() => DiBridgeService.GetService<IAppGraphEntityToolsHandler>();

    [McpTool(Cx.CmdAddEntityClass, Cx.CmdAddEntityClassDesc)]
    public static Task<string> AddEntityClassModel(
      [Description("The Item Id of the parent item (either Library or Namespace type Models) to add the new entity class model.")] int parentItemId,
      [Description("The name of the new entity class model. This is normally a Singular named class.")] string className,
      [Description("The name of the database table for the new entity class model. This is normally a pluralized form of the class name.")] string entityDbTableName
    ) {
      return GetTools().AddEntityClassModel(parentItemId, className, entityDbTableName);
    }


    [McpTool(Cx.CmdAddEntityProperty, Cx.CmdAddEntityPropertyDesc)]
    public static Task<string> AddEntityPropertyModel(
      [Description("The Item Id of the entity class model to add the new property model to.")]
      int entityClassId,
      [Description("The name of the new property to add. Should end in Id if it is a navigation property.")]
      string propertyName,
      [Description($"The type Id of the property. Main ones: string 54, int 57, long 58; for the full list of types see {Cx.CmdGetTypeDetails} using ItemTypeId 50 for CSharpTypes. ")]
      int propertyTypeId,
      [Description("Indicates if the property is an EF Core navigation property. If so, it will add an additional navigation Item off the new property model.")]
      bool isNav,
      [Description("If it is a navigation property, the Item Id of the related entity class model for the new navigation properties, else use 0.")]
      int navEntityClassId) {
      return GetTools().AddEntityPropertyModel(entityClassId, propertyName, propertyTypeId, isNav, navEntityClassId);
    }


  }
}
