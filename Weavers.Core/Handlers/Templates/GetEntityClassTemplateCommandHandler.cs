using Azure.Core;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Templates {

  public record GetEntityClassTemplateCommand(int EntityClassItemId): IRequest<string?>;

  public class GetEntityClassTemplateCommandHandler :IRequestHandler<GetEntityClassTemplateCommand, string?>  {
    private readonly FabricDbContext _context;
    public GetEntityClassTemplateCommandHandler(FabricDbContext context) { 
      _context = context;
    }

    public async Task<string?> Handle(GetEntityClassTemplateCommand request, CancellationToken ct) { 

      var item = await _context.GetItemDtoById(request.EntityClassItemId, ct);
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
      var sbNavs = new StringBuilder();

      var namespaceName = item.ResolveParentNamespace(item.Name);
      var usesHashSet = new HashSet<string>();
      usesHashSet.Add(namespaceName);
      var className = item.Name;     

      var hasConstructorParams = false;
      var imports = item.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityClassImportModel)
        .Select(r => r.RelatedItemId).Where(id => id.HasValue).Select(id => id!.Value);

      foreach (var importId in imports) {
        hasConstructorParams = true;
        var importItem = await _context.GetItemDtoById(importId, ct);
        if (importItem != null) {
          var importObjId = importItem.Properties.FirstOrDefault(p => p.Name == Cx.ItImportObject)?.Value;
          var importUseInterface = importItem.Properties.FirstOrDefault(p => p.Name == Cx.ItImportUseInterface)?.Value.AsBoolean();
          string intfTypeClause = importUseInterface.HasValue && importUseInterface.Value ? "I" : "";
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

      var classProps = item.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityPropertyModel)
        .Select(r => r.RelatedItemId).Where(id => id.HasValue).Select(id => id!.Value);
      foreach (var propId in classProps) {
        var propItem = await _context.GetItemDtoById(propId, ct);
        if (propItem != null) {
          var propName = propItem.Name;
          var dataTypeProp = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyDataType);          
          var isNullableProp = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable);
          var hasSetterProp = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasSetter);
          var isNavProp = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasNavigation);
          var isPrimaryKeyProp = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsPrimaryKey);
          var maxSizeProp = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItMaxSize);
          bool isNullable = isNullableProp != null && isNullableProp.Value.AsBoolean();
          bool hasSetter = hasSetterProp != null && hasSetterProp.Value.AsBoolean();
          string setterClause = hasSetter ? " { get; set; }" : " { get; }";          
          bool isNav = isNavProp != null && isNavProp.Value.AsBoolean();
          bool isPrimaryKey = isPrimaryKeyProp != null && isPrimaryKeyProp.Value.AsBoolean();
          string nullableClause = isNullable && !isPrimaryKey ? "?" : "";
          int dataTypeId = dataTypeProp != null && dataTypeProp.Value != null ? int.Parse(dataTypeProp.Value) : 0;          
          int maxSize = (maxSizeProp != null && int.TryParse( maxSizeProp.Value, out var maxSizeOut)) ? maxSizeOut : 0;
          string propTypeName = "";
          if (dataTypeId == (int)WeItemType.CSharpClassType) {                        
              propTypeName = "object";            
          } else {
            propTypeName = dataTypeId != 0 ? ((WeItemType)dataTypeId).AsCsCode() : "object";
          }
          sbProperties.AppendLine($"    public {propTypeName}{nullableClause} {propName} {setterClause}");
          if (isNav) {
            var navItemId = propItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.EntityNavigationModel)?.RelatedItemId??0;
            if (navItemId > 0) {
              var navItem = await _context.GetItemDtoById(navItemId, ct);
              var isNavNullableProp = navItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable);
              bool isNavNullable = isNullableProp != null && isNullableProp.Value.AsBoolean();
              var classTypeProp = navItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType);
              var classTypeId = classTypeProp != null && classTypeProp.Value != null ? int.Parse(classTypeProp.Value) : 0;
              
              var navTypeProp = navItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasNavigation);
              WeItemType navType = (navTypeProp != null && int.TryParse(navTypeProp.Value, out var navTypeEnumInt)) ? (WeItemType)navTypeEnumInt : WeItemType.NavHasOne;
              if (classTypeId > 0) { 
                var classTypeItem = await _context.GetItemDtoById(classTypeId, ct); 
                if (classTypeItem != null) { 
                  string classTypeName = classTypeItem.Name;
                  string navName = classTypeName+"Set";
                  string navNullableClause = isNavNullable ? "?" : "";
                  string navNulClause2 = isNavNullable ? " = null;" : " = null!;";
                  if (navType == WeItemType.NavHasOne) { 
                    sbNavs.AppendLine($"    public {classTypeName}{navNullableClause} {classTypeName}{setterClause}{navNulClause2}");
                  } else if (navType == WeItemType.NavHasMany) { 
                    sbNavs.AppendLine($"    public ICollection<{classTypeName}> {navName}{setterClause} = [];");
                  }
                }
              }              

            }
          }
          
          
        }
      }     


      cSb.AppendLine($"namespace {namespaceName} {{");
      cSb.AppendLine("");  

      cSb.AppendLine($"  public class {className} {{");
      if (hasConstructorParams) {
        cSb.AppendLine(sbLocals.ToString());
        cSb.AppendLine($"    public {className}(");
        cSb.AppendLine(sbConstParams.ToString().TrimEnd(',', '\r', '\n'));
        cSb.AppendLine($"    ) {{");
        cSb.Append(sbConstructor.ToString());
        cSb.AppendLine($"    }}");
      }

      cSb.AppendLine(sbProperties.ToString());      
      cSb.AppendLine(sbNavs.ToString());

      cSb.AppendLine($"  }}");
      cSb.AppendLine($"}}");

      return sbUses.ToString() + Environment.NewLine + cSb.ToString();
    }    


  }
}
