using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {
  public static class ItemTemplateExts {

    public async static Task<BuildResult?> BuildMethod(this FabricDbContext context, int methodItemId, CancellationToken ct) {

      var methodItem = await context.GetItemDtoById(methodItemId, ct);
      if (methodItem != null) {

        var msgParams = new StringBuilder();
        var methodParamProps = methodItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.ClassMethodParameterModel)
          .Select(r => r.RelatedItemId).Where(id => id.HasValue).Select(id => id!.Value);
        foreach (var methodParam in methodParamProps) {
          var methodParamItem = await context.GetItemDtoById(methodParam, ct);
          if (methodParamItem != null) {
            var useThis = methodParamItem.Properties.FirstOrDefault(p => p.Name == Cx.ItUseThis)?.Value.AsBoolean() ?? false;
            string thisClause = useThis ? "this " : "";
            var paramName = methodParamItem.Name;
            var paramType = await context.GenerateParameterType(methodParamItem);
            msgParams.AppendLine($"      {thisClause}{paramType} {paramName},");
          }
        }

        var methodName = methodItem.Name.AsUpperCaseFirstLetter();
        var accessModifier = methodItem.GenerateMethodAccessibility();
        var returnType = await context.GenerateReturnType(methodItem);

        var msgContent = (methodItem.Description ?? "").TrimEnd('\r', '\n');
        string mParms = msgParams.ToString().TrimEnd(',', '\r', '\n').TrimStart(' ');        
        return new BuildResult() {
          InterfaceSignature =  $"    {returnType} {methodName}({mParms});",
          MethodSignature = $"    {accessModifier}{returnType} {methodName}({mParms})",
          MethodBody = msgContent
        };
      }
      return null;
      
    }

    private static string GenerateMethodAccessibility(this ItemDto methodItem) {
      string accessibility = "public";
      var propAm = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItAccessModifier);
      if (propAm != null && propAm.Value != null) {
        if (int.TryParse(propAm.Value, out int accessModifierId)) {
          var amType = (WeItemType)accessModifierId;
          accessibility = amType.Description();
        }
      }

      bool isAsync = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsAsync)?.Value.AsBoolean() ?? false;
      bool isVirtual = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsVirtual)?.Value.AsBoolean() ?? false;
      bool isStatic = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsStatic)?.Value.AsBoolean() ?? false;
      bool isAbstract = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsAbstract)?.Value.AsBoolean() ?? false;
      bool isSealed = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsSealed)?.Value.AsBoolean() ?? false;

      string AccessibilityClause = $"{accessibility} {(isStatic ? "static " : "")}{(isAsync ? "async " : "")}{(isVirtual ? "virtual " : "")}{(isAbstract ? "abstract " : isSealed ? "sealed " : "")}";
      return AccessibilityClause;
    }


    private async static Task<string> GenerateParameterType(this FabricDbContext context, ItemDto methodParameterItem) {
      string returnType = "void";
      var dataTypeProp = methodParameterItem.Properties.FirstOrDefault(p => p.Name == Cx.ItParameterDataType);
      var nullable = methodParameterItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable)?.Value.AsBoolean() ?? false;
      if (dataTypeProp != null && dataTypeProp.Value != null) {
        if (int.TryParse(dataTypeProp.Value, out int dataTypeId)) {
          var returnDataType = ((WeItemType)dataTypeId);
          var nullableClause = nullable ? "?" : "";
          if (returnDataType == WeItemType.CSharpClassType) {
            var returnClassProp = methodParameterItem.Properties.FirstOrDefault(p => p.Name == Cx.ItParameterClassType);
            if (returnClassProp != null && returnClassProp.Value != null) {
              if (int.TryParse(returnClassProp.Value, out int returnClassId)) {
                var returnClassItem = await context.GetItemDtoById(returnClassId);
                if (returnClassItem != null) {
                  var propUseInterface = methodParameterItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGenerateInterface)?.Value.AsBoolean() ?? false;
                  returnType = (propUseInterface ? "I" : "") + returnClassItem.Name.AsUpperCaseFirstLetter() + nullableClause;
                } else {
                  returnType = "object" + nullableClause;
                }
              } else {
                returnType = "object" + nullableClause;
              }
            } else {
              returnType = returnDataType.AsCsCode() + nullableClause;
            }
          } else {
            returnType = returnDataType.AsCsCode() + nullableClause;
          }
        }
      }
      return returnType;
    }


    private async static Task<string> GenerateReturnType(this FabricDbContext context, ItemDto methodItem) {
      string returnType = "void";
      var returnTypeProp = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReturnDataType);
      var retNullable = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReturnNullable)?.Value.AsBoolean() ?? false;
      string nullableClause = retNullable ? "?" : "";
      if (returnTypeProp != null && returnTypeProp.Value != null) {
        bool isAsync = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsAsync)?.Value.AsBoolean() ?? false;
        if (int.TryParse(returnTypeProp.Value, out int dataTypeId)) {
          var returnDataType = ((WeItemType)dataTypeId);
          if (returnDataType == WeItemType.CSharpClassType) {
            var returnClassProp = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReturnClassType);
            if (returnClassProp != null && returnClassProp.Value != null) {
              if (int.TryParse(returnClassProp.Value, out int returnClassId)) {
                var returnClassItem = await context.GetItemDtoById(returnClassId);
                if (returnClassItem != null) {
                  var propUseInterface = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGenerateInterface)?.Value.AsBoolean() ?? false;
                  returnType = (propUseInterface ? "I" : "") + returnClassItem.Name.AsUpperCaseFirstLetter() + nullableClause;
                } else {
                  returnType = "object" + nullableClause;
                }
              } else {
                returnType = "object" + nullableClause;
              }
            } else {
              returnType = returnDataType.AsCsCode() + nullableClause;
            }
          } else {
            returnType = returnDataType.AsCsCode() + nullableClause;
          }
        }
        if (isAsync) {
          returnType = returnType == "void" ? "void" : $"Task<{returnType}>";
        }
      }

      return returnType;
    }


  }


  public class BuildResult {
    public string? InterfaceSignature { get; set; } = null;
    public string MethodSignature { get; set; } = "";
    public string MethodBody { get; set; } = "";    
  }

}
