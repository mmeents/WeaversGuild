using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCPSharp;
using Weavers.Core.Service;
using Weavers.Core.Constants;
using System.ComponentModel;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace Weavers.Core.Tools {
  public class AppGraphClassTools {
    private static IAppGraphClassToolsHandler GetTools() => DiBridgeService.GetService<IAppGraphClassToolsHandler>();

    [McpTool(Cx.CmdAddClass, "Adds a new class model, with options to generate interface and register DI.")]
    public static Task<string> AddClassModel(
     [Description("The ID of the parent item. (an item with type either Library 1200 or Namespace 1400 type Models)")] int parentItemId,
     [Description("The name of the new class.")] string className,
     [Description("Generate an interface for the class?")] bool generateInterface,
     [Description("Register the class with dependency injection model?")] bool registerDI
   ) {
      return GetTools().AddClassModel(parentItemId, className, generateInterface, registerDI);
    }


    [McpTool(Cx.CmdAddClassImport, "Adds a new class import model to an existing class. Makes a private _var and sets it via constructor and DI.")]
    public static Task<string> AddClassImportModel(
      [Description("The ID of the class item.")] int classItemId,
      [Description("The ID of the class model to import.")] int importClassId
    ) {
      return GetTools().AddClassImportModel(classItemId, importClassId);
    }


    [McpTool(Cx.CmdAddClassProperty, "Adds a new class property model to an existing class. " +
      $"Note: when propertyTypeId is ClassType 51, RecordType 52, or StructType 53 then propertyClassId is a reference to that item, else ignored. For the full list of types see {Cx.CmdGetTypeDetails} using ItemTypeId 50 for CSharpTypes.")]
    public static Task<string> AddClassPropModel(
      [Description("The ID of the class item.")] int classItemId,
      [Description("The name of the property.")] string propertyName,
      [Description("The ID of the property type. Property types: string 54, int 57, long 58;")] int? propertyTypeId,
      [Description("The Id of an item with type ClassType, RecordType, or StructType.")] int? propertyClassId
    ) {
      return GetTools().AddClassPropModel(classItemId, propertyName, propertyTypeId, propertyClassId);
    }


    [McpTool(Cx.CmdAddClassMethod, "Adds a new class method model to an existing class.")]
    public static Task<string> AddClassMethodModel(
      [Description("The ID of the class item.")] int classItemId,
      [Description("The name of the method.")] string methodName,
      [Description("Is the method asynchronous?")] bool? isAsync,
      [Description("The ID of the return type.")] int? returnTypeId,
      [Description("The ID of the return class.")] int? returnClassId
    ) {
      return GetTools().AddClassMethodModel(classItemId, methodName, isAsync, returnTypeId, returnClassId);
    }


    [McpTool(Cx.CmdAddClassMethodParam, "Adds a new class method param model to an existing method.")]
    public static Task<string> AddClassMethodParamModel(
      [Description("The ID of the method item.")] int methodItemId,
      [Description("The name of the parameter.")] string paramName,
      [Description("The ID of the parameter type.")] int? paramTypeId,
      [Description("The ID of the parameter class.")] int? paramClassId
    ) {
      return GetTools().AddClassMethodParam(methodItemId, paramName, paramTypeId, paramClassId);
    }

  }
}
