using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCPSharp;
using Weavers.Core.Service;
using Weavers.Core.Constants;

namespace Weavers.Core.Tools {
  public class AppGraphClassTools {
    private static IAppGraphClassToolsHandler GetTools() => DiBridgeService.GetService<IAppGraphClassToolsHandler>();


    [McpTool(Cx.CmdAddClass, "Adds a new class model, with options to generate interface and register DI.")]
    public static Task<string> AddClassModel(
      int parentItemId, 
      string className, 
      bool generateInterface, 
      bool registerDI
    ) {
        return GetTools().AddClassModel(parentItemId, className, generateInterface, registerDI);
    }


    [McpTool(Cx.CmdAddClassImport, "Adds a new class import model to an existing class. makes private _var and sets via constructor and di.")]
    public static Task<string> AddClassImportModel(
      int classItemId, 
      int importClassId
    ) {
      return GetTools().AddClassImportModel(classItemId, importClassId);
    }


    [McpTool(Cx.CmdAddEntityProperty, "Adds a new entity class property model to an existing class.")]
    public static Task<string> AddClassPropModel(
      int classItemId, 
      string propertyName
    ) {
      return GetTools().AddClassPropModel(classItemId, propertyName);
    }


    [McpTool(Cx.CmdAddClassMethod, "Adds a new class method model to an existing class.")]
    public static Task<string> AddClassMethodModel(
      int classItemId, 
      string methodName
    ) {
      return GetTools().AddClassMethodModel(classItemId, methodName);
    }


    [McpTool(Cx.CmdAddMethodParam, "Adds a new class method param model to an existing method.")]
    public static Task<string> AddClassMethodParamModel(
      int methodItemId,
      string paramName
    ) { 
      return GetTools().AddClassMethodParam(methodItemId, paramName);
    }

  }
}
