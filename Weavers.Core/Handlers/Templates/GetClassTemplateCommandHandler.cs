using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Extensions;
using Weavers.Core.Models;
using Weavers.Core.Enums;


namespace Weavers.Core.Handlers.Templates {

  public record GetClassTemplateCommand(int ClassItemId) : IRequest<string?>;

  public class GetClassTemplateCommandHandler : IRequestHandler<GetClassTemplateCommand, string?> {
    private readonly FabricDbContext _context;
    public GetClassTemplateCommandHandler(FabricDbContext context) { 
      _context = context;
    }

    public async Task<string?> Handle(GetClassTemplateCommand request, CancellationToken ct) { 

      var item = await _context.GetItemDtoById(request.ClassItemId, ct);
      if (item == null) { 
        return "";
      }

      var cSb = new StringBuilder();
      var sbUses = new StringBuilder();
      var sbLocals = new StringBuilder();
      var sbConstParams = new StringBuilder();
      var sbConstructor = new StringBuilder();
      var sbInterface = new StringBuilder();
      var sbProperties = new StringBuilder();
      var sbMethod = new StringBuilder();

      var namespaceName = item.ResolveParentNamespace(item.Name);
      var usesHashSet = new HashSet<string>();
      usesHashSet.Add(namespaceName);
      var className = item.Name;
      var accessibilityClause = GenerateClassAccessibility(item);
      var baseType  = "";     

      var propGenerateInterface = item.Properties.FirstOrDefault(p => p.Name == Cx.ItGenerateInterface);
      bool generateInterface = false;
      if (propGenerateInterface != null) {
        generateInterface = propGenerateInterface.Value.AsBoolean();
      }

      var useIntf = false;
      var propBaseType = item.Properties.FirstOrDefault(p => p.Name == Cx.ItBaseType);
      if (propBaseType != null && propBaseType.Value != null) { 
        if (int.TryParse( propBaseType.Value, out int baseItemId)){ 
          var baseClass = await _context.GetItemDtoById(baseItemId);           
          var propBcGenIntf = baseClass.Properties.FirstOrDefault(p => p.Name == Cx.ItGenerateInterface);
          if (propBcGenIntf != null && propBcGenIntf.Value != null) { 
            useIntf = propBcGenIntf.Value.AsBoolean();
          }
          if (useIntf) { 
            baseType = "I"+baseClass.Name;
          } else { 
            baseType = baseClass.Name;
          }
        }
      }
      if (baseType != "") { 
        baseType = $" : {baseType}";
      }
      
      var interfaceContent = "";
      if (generateInterface) {                 
        var interfaceName = "I" + item.Name;
        sbInterface.AppendLine($"  {accessibilityClause}interface {interfaceName}{baseType} {{");
        baseType = " : I"+item.Name;        
      }
      

      var hasConstructorParams = false;
      var imports = item.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.ClassImportModel)
        .Select(r => r.RelatedItemId).Where(id => id.HasValue).Select(id => id!.Value);
      
      
      foreach (var importId in imports) {
        hasConstructorParams = true;
        var importItem = await _context.GetItemDtoById(importId, ct);
        if (importItem != null) { 
          var importObjId = importItem.Properties.FirstOrDefault(p => p.Name == Cx.ItImportObject)?.Value;
          var importUseInterface = importItem.Properties.FirstOrDefault(p => p.Name == Cx.ItImportUseInterface)?.Value.AsBoolean();
          string intfTypeClause = importUseInterface.HasValue && importUseInterface.Value ? "I":"";
          if (importObjId != null) { 
            var importObj = await _context.GetItemDtoById(int.Parse(importObjId), ct);
            if (importObj != null) { 
              var importNamespace = importObj.ResolveParentNamespace(importObj.Name);
              var importObjName = importObj.Name.AsUpperCaseFirstLetter();
              var varName = $"{importObj.Name.AsLowerCaseFirstLetter()}";
              if (!usesHashSet.Contains(importNamespace)) { 
                sbUses.AppendLine($"using {importNamespace};");              
                usesHashSet.Add(importNamespace);
              }
              sbLocals.AppendLine($"    private readonly {intfTypeClause}{importObjName} _{varName};");
              sbConstParams.AppendLine($"      {intfTypeClause}{importObjName} {varName},");
              sbConstructor.AppendLine($"      _{varName} = {varName};");
            }
          }
        }
      }

    

      var classProps = item.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.ClassPropertyModel)
        .Select(r => r.RelatedItemId).Where(id => id.HasValue).Select(id => id!.Value);
      foreach (var propId in classProps) { 
        var propItem = await _context.GetItemDtoById(propId, ct);
        if (propItem != null) {
          var propName = propItem.Name;
          var dataTypeProp = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyDataType);
          var classTypeProp = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType);
          var isNullableProp = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable);
          var hasSetterProp = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasSetter);
          bool isNullable = isNullableProp != null && isNullableProp.Value.AsBoolean();
          bool hasSetter = hasSetterProp != null && hasSetterProp.Value.AsBoolean();
          string setterClause = hasSetter ? " { get; set; }" : " { get; }";
          string nullableClause = isNullable ? "?" : "";
          int dataTypeId = dataTypeProp != null && dataTypeProp.Value != null ? int.Parse(dataTypeProp.Value) : 0;
          int classTypeId =0;
          string propTypeName = "";
          if (dataTypeId == (int)WeItemType.CSharpClassType) { 
            classTypeId = classTypeProp != null && classTypeProp.Value != null ? int.Parse(classTypeProp.Value) : 0;
            if (classTypeId != 0) { 
              var classTypeItem = await _context.GetItemDtoById(classTypeId, ct);
              if (classTypeItem != null) { 
                var propUseInterface = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItImportUseInterface)?.Value.AsBoolean() ?? false;
                propTypeName = propUseInterface ? "I":"" + classTypeItem.Name.AsUpperCaseFirstLetter();
              } else { 
                propTypeName = "object";
              }
            } else { 
              propTypeName = "object";
            }
          } else { 
            propTypeName = dataTypeId != 0 ? ((WeItemType)dataTypeId).AsCsCode() : "object";
          }                        
          sbProperties.AppendLine($"    public {propTypeName}{nullableClause} {propName} {setterClause}");
          if (generateInterface) { 
            sbInterface.AppendLine($"    {propTypeName}{nullableClause} {propName} {setterClause}");
          }
        }
      }

      var methodProps = item.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.ClassMethodModel)
        .Select(r => r.RelatedItemId).Where(id => id.HasValue).Select(id => id!.Value);
      foreach(var methodProp in methodProps) {
        var methodItem = await _context.GetItemDtoById(methodProp, ct);
        if (methodItem != null) {

          var msgParams = new StringBuilder();
          var methodParamProps = methodItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.ClassMethodParameterModel)
            .Select(r => r.RelatedItemId).Where(id => id.HasValue).Select(id => id!.Value);
          foreach(var methodParam in methodParamProps) { 
            var methodParamItem = await _context.GetItemDtoById(methodParam, ct);
            if (methodParamItem != null) {
              var useThis = methodParamItem.Properties.FirstOrDefault(p => p.Name == Cx.ItUseThis)?.Value.AsBoolean() ?? false;
              string thisClause = useThis ? "this " : "";
              var paramName = methodParamItem.Name;
              var paramType = await GenerateParameterType(methodParamItem);
              msgParams.AppendLine($"      {thisClause}{paramType} {paramName},");
            }
          }

          var methodName = methodItem.Name.AsUpperCaseFirstLetter();
          var accessModifier = GenerateMethodAccessibility( methodItem);
          var returnType = await GenerateReturnType(methodItem);
          
          var msgContent = (methodItem.Description ?? "").TrimEnd('\r', '\n');
          string mParms = msgParams.ToString().TrimEnd(',', '\r', '\n').TrimStart(' ');
          if (mParms == "") {
            sbMethod.AppendLine($"    {accessModifier}{returnType} {methodName}() {{");
          } else {
            sbMethod.AppendLine($"    {accessModifier}{returnType} {methodName}({mParms}) {{");
          }          
          sbMethod.AppendLine($"{msgContent}");
          sbMethod.AppendLine($"    }}");

          if (generateInterface) {
            sbInterface.AppendLine($"    {returnType} {methodName}({mParms});");
          }
        }
      }

      if (generateInterface) {
        sbInterface.AppendLine($"  }}");
        interfaceContent = sbInterface.ToString();
      }

      cSb.AppendLine($"namespace {namespaceName} {{");      
      cSb.AppendLine("");
      if (generateInterface) {
        cSb.AppendLine(interfaceContent);
        cSb.AppendLine("");
      }

      cSb.AppendLine($"  {accessibilityClause}class {className}{baseType} {{");
      if (hasConstructorParams) {
        cSb.AppendLine(sbLocals.ToString());
        cSb.AppendLine($"    public {className}(");
        cSb.AppendLine(sbConstParams.ToString().TrimEnd(',', '\r', '\n'));
        cSb.AppendLine($"    ) {{");
        cSb.Append(sbConstructor.ToString());
        cSb.AppendLine($"    }}");
      }

      cSb.AppendLine(sbProperties.ToString());

      cSb.AppendLine(sbMethod.ToString());

      cSb.AppendLine($"  }}");
      cSb.AppendLine($"}}");

      return sbUses.ToString() +Environment.NewLine
        + cSb.ToString();
    }

    private string GenerateClassAccessibility(ItemDto classItem) {
      string accessibility = "public";
      var propAm = classItem.Properties.FirstOrDefault(p => p.Name == Cx.ItAccessModifier);
      bool isStatic = classItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsStatic)?.Value.AsBoolean() ?? false;
      if (propAm != null && propAm.Value != null) {
        if (int.TryParse(propAm.Value, out int accessModifierId)) { 
          var amType = (WeItemType)accessModifierId;
          accessibility = amType.Description();
        }        
      }      
      string AccessibilityClause = $"{accessibility} " + (isStatic ? "static " : "");
      return AccessibilityClause;
    }

    private string GenerateMethodAccessibility(ItemDto methodItem) {
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
      
      string AccessibilityClause = $"{accessibility} {(isAsync ? "async " : "")}{(isVirtual ? "virtual " : "")}{(isStatic ? "static " : "")}{(isAbstract ? "abstract " : "")}{(isSealed ? "sealed " : "")}";
      return AccessibilityClause;     
    }

    private async Task<string> GenerateReturnType(ItemDto methodItem) {
      string returnType = "void";
      var returnTypeProp = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReturnDataType);
      if (returnTypeProp != null && returnTypeProp.Value != null) {
        bool isAsync = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsAsync)?.Value.AsBoolean() ?? false;
        if (int.TryParse(returnTypeProp.Value, out int dataTypeId)) { 
          var returnDataType = ((WeItemType)dataTypeId);
          if (returnDataType == WeItemType.CSharpClassType) { 
            var returnClassProp = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReturnClassType);
            if (returnClassProp != null && returnClassProp.Value != null) { 
              if (int.TryParse(returnClassProp.Value, out int returnClassId)) { 
                var returnClassItem = await _context.GetItemDtoById(returnClassId);
                if (returnClassItem != null) { 
                  var propUseInterface = methodItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGenerateInterface)?.Value.AsBoolean() ?? false;
                  returnType = (propUseInterface ? "I":"") + returnClassItem.Name.AsUpperCaseFirstLetter();                  
                } else { 
                  returnType = "object";                  
                }
              } else { 
                returnType = "object";                
              }
            } else { 
              returnType = returnDataType.AsCsCode();         
            }
          } else { 
            returnType = returnDataType.AsCsCode();         
          }
        }
        if (isAsync) {
          returnType = $"Task<{returnType}>";
        }
      }

      return returnType;
    }

    private async Task<string> GenerateParameterType(ItemDto methodParameterItem) {
      string returnType = "void";
      var dataTypeProp = methodParameterItem.Properties.FirstOrDefault(p => p.Name == Cx.ItParameterDataType);
      if (dataTypeProp != null && dataTypeProp.Value != null) {
        if (int.TryParse(dataTypeProp.Value, out int dataTypeId)) {
          var returnDataType = ((WeItemType)dataTypeId);
          if (returnDataType == WeItemType.CSharpClassType) {
            var returnClassProp = methodParameterItem.Properties.FirstOrDefault(p => p.Name == Cx.ItParameterClassType);
            if (returnClassProp != null && returnClassProp.Value != null) {
              if (int.TryParse(returnClassProp.Value, out int returnClassId)) {
                var returnClassItem = await _context.GetItemDtoById(returnClassId);
                if (returnClassItem != null) {
                  var propUseInterface = methodParameterItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGenerateInterface)?.Value.AsBoolean() ?? false;
                  returnType = (propUseInterface ? "I" : "") + returnClassItem.Name.AsUpperCaseFirstLetter();
                } else {
                  returnType = "object";
                }
              } else {
                returnType = "object";
              }
            } else {
              returnType = returnDataType.AsCsCode();
            }
          } else { 
            returnType = returnDataType.AsCsCode();
          }
        }
      }
      return returnType;
    }



  }
}
