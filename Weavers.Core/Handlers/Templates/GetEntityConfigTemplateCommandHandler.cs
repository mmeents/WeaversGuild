using MediatR;
using System.Text;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace Weavers.Core.Handlers.Templates {
  public record GetEntityConfigTemplateCommand(int EntityCfgItemId, bool includeNamespace = true) : IRequest<string>;

  public class GetEntityConfigTemplateCommandHandler : IRequestHandler<GetEntityConfigTemplateCommand, string> {
    private readonly FabricDbContext _context;
    public GetEntityConfigTemplateCommandHandler(FabricDbContext context) { 
      _context = context;
    }

    public async Task<string> Handle(GetEntityConfigTemplateCommand request, CancellationToken ct) {

      var cfgItem = await _context.GetItemDtoById(request.EntityCfgItemId, ct);
      if (cfgItem == null) return "entity config template not found.";

      var itemId = cfgItem.IncomingRelations.FirstOrDefault(r => r.ItemId != cfgItem.Id)?.ItemId;
      if (itemId == null || itemId <= 0) return "entity not found.";

      var item = await _context.GetItemDtoById(itemId.Value, ct);
      if (item == null) return "entity not found.";
      string dbTableName = item.Properties.FirstOrDefault(p => p.Name == Cx.ItDbTableName)?.Value?.ToString() ?? $"{item.Name}Set";
      string schemaName = item.Properties.FirstOrDefault(p => p.Name == Cx.ItDbSchema)?.Value?.ToString() ?? "dbo";
      string itemClassName = item.Name;

      var cSb = new StringBuilder();
      var sbProperties = new StringBuilder();
      var sbPropNavs = new StringBuilder();
      var sbNavs = new StringBuilder();

      var namespaceName = item.ResolveParentNamespace("missingNamespace");
      var className = item.Name;

      var classProps = item.Relations
       .Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityPropertyModel)
       .Select(r => r.RelatedItemId)
       .Where(id => id.HasValue)
       .Select(id => id!.Value);

      bool pkPrinted = false;
      string pkPropName = "";

      foreach (var propId in classProps) {
        var propItem = await _context.GetItemDtoById(propId, ct);
        if (propItem == null) continue;

        var propName = propItem.Name;
        bool isPrimaryKey = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsPrimaryKey)?.Value.AsBoolean() ?? false;

        bool isNullable = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable)?.Value.AsBoolean() ?? false;
        string nullableClause = isNullable && !isPrimaryKey ? "?" : "";


        if (isPrimaryKey && !pkPrinted) {
          pkPropName = propName;
          sbProperties.AppendLine($"      builder.HasKey(x => x.{propName});\r\n");
          sbProperties.AppendLine($"      builder.Property(x => x.{propName}).ValueGeneratedOnAdd();");
          pkPrinted = true;
          continue;
        }
        if (isPrimaryKey) continue;

        var isNavProp = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasNavigation);
        bool isNav = isNavProp != null && isNavProp.Value.AsBoolean();
        if (isNav) {
          var navItemId = propItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.EntityNavigationModel)?.RelatedItemId ?? 0;
          if (navItemId >= 0) {
            var navItem = await _context.GetItemDtoById(navItemId);
            if (navItem != null) {
              var relationTypeStr = navItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasNavigation)?.Value;
              var relationTypeId = relationTypeStr != null && int.TryParse(relationTypeStr.ToString(), out var tempRelationTypeId) ? tempRelationTypeId : 0;
              WeItemType relationType = WeItemType.NavigationTypes;
              if (relationTypeId > 0) {
                relationType = (WeItemType)relationTypeId;
              } else {
                relationType = WeItemType.NavHasManyToOne;
              }
              var targetClassItemId = navItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType)?.Value;
              if (targetClassItemId != null && int.TryParse(targetClassItemId.ToString(), out var targetClassId)) {
                var targetClassItem = await _context.GetItemDtoById(targetClassId, ct);
                if (targetClassItem != null) {
                  var targetClassName = targetClassItem.Name;
                }
              }

            }
          }
        }

        var dataTypeProp = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyDataType);
        int dataTypeId = dataTypeProp != null && dataTypeProp.Value != null ? int.Parse(dataTypeProp.Value) : 0;

        var maxSizeProp = propItem.Properties.FirstOrDefault(p => p.Name == Cx.ItMaxSize);
        string maxSize = (maxSizeProp != null && maxSizeProp.Value != null) ? maxSizeProp.Value : "0";

        string isReqClause = "";
        if (!isNullable) {
          isReqClause = ".IsRequired()";
        }

        string propTypeName = "";
        if (dataTypeId == (int)WeItemType.CSharpClassType) {
          propTypeName = "";
        } else {
          propTypeName = ((WeItemType)dataTypeId).GetConfigSqlType(maxSize);
        }

        sbProperties.AppendLine($"      builder.Property(x => x.{propName}){propTypeName}{isReqClause};");

      }


      var inboundNavIds = item.Relations
        .Where(r => r.RelatedItemTypeId == (int)WeItemType.EntityInboundNavigationModel)
        .Select(r => r.RelatedItemId)
        .Where(id => id.HasValue)
        .Select(id => id!.Value);

      var printedComment = false;

      foreach (var navId in inboundNavIds) {

        var navItem = await _context.GetItemDtoById(navId, ct);
        if (navItem == null) continue;
        var isNullable = navItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIsNullable)?.Value.AsBoolean() ?? false;
        string nullableClause = isNullable ? "?" : "";

        if (!printedComment) {
          sbNavs.AppendLine("    // Inbound nav properties");
          printedComment = true;
        }

        var classTypeProp = navItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropertyClassType);
        var classTypeId = classTypeProp != null && classTypeProp.Value != null ? int.Parse(classTypeProp.Value) : 0;
        var propClassDbTableName = "MissingTableName";
        var propClassStr = "missingClass";
        if (classTypeId > 0) {
          var propClass = await _context.GetItemDtoById(classTypeId, ct);
          if (propClass != null) {
            propClassDbTableName = propClass.Properties.FirstOrDefault(p => p.Name == Cx.ItDbTableName)?.Value?.ToString() ?? $"{propClass.Name}Set";
          }
          propClassStr = propClass?.Name ?? "missingClass";
        }

        var navTypeProp = navItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasNavigation);
        var navTypeId = navTypeProp != null && navTypeProp.Value != null && int.TryParse(navTypeProp.Value, out var tempNavTypeId) ? tempNavTypeId : 0;

        var deleteBehavior = navItem.ResolveDeleteBehavior();

        var fkProp = navItem.Properties.FirstOrDefault(p => p.Name == Cx.ItForeignKey);
        var fkId = fkProp != null && fkProp.Value != null && int.TryParse(fkProp.Value, out var tempFkId) ? tempFkId : 0;
        var fkItem = fkId > 0 ? await _context.GetItemDtoById(fkId, ct) : null;  // should be driving property item .
        var fkName = fkItem?.Name ?? $"{navItem.Name}";
        if (fkItem != null) {



        }

        string navConfig = (WeItemType)navTypeId switch {
          WeItemType.NavHasOneToOne =>
              $"builder.HasOne(x => x.{propClassStr}).WithOne(y => y.{navItem.Name}).HasForeignKey<{propClassStr}>(y => y.{fkName}){deleteBehavior};",

          WeItemType.NavHasOneToMany =>
              $"builder.HasOne(x => x.{propClassStr}).WithMany(y => y.{propClassDbTableName}).HasForeignKey(y => y.{fkName}){deleteBehavior};",

          WeItemType.NavHasManyToOne =>   // Most important one for inbound references
              $"builder.HasMany(x => x.{propClassDbTableName}).WithOne(y => y.{navItem.Name}).HasForeignKey(y => y.{fkName}){deleteBehavior};",

          WeItemType.NavHasManyToMany =>
              $"builder.HasMany(x => x.{propClassDbTableName}).WithMany(y => y.{propClassDbTableName}){deleteBehavior};",

          _ => $"// Unknown navigation: {navItem.Name}"
        };

        sbNavs.AppendLine($"      {navConfig}");
      }

      if (request.includeNamespace) { 
        cSb.AppendLine("using Microsoft.EntityFrameworkCore;");
        cSb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
        cSb.AppendLine("");
        cSb.AppendLine($"namespace {namespaceName} {{");        
      }

      cSb.AppendLine("");
      cSb.AppendLine($"  public class {className}Configuration : IEntityTypeConfiguration<{className}> {{");
      cSb.AppendLine($"    public void Configure(EntityTypeBuilder<{className}> builder) {{");   
      if(schemaName == "dbo") {
        cSb.AppendLine($"      builder.ToTable(\"{dbTableName}\");");
      } else {
        cSb.AppendLine($"      builder.ToTable(\"{dbTableName}\", \"{schemaName}\");");
      }
      cSb.AppendLine("");
      cSb.AppendLine(sbProperties.ToString());
      cSb.AppendLine("");
      cSb.AppendLine(sbNavs.ToString());

      cSb.AppendLine($"    }}");
      cSb.AppendLine($"  }}");

      if (request.includeNamespace) {
        cSb.AppendLine($"}}");
      }      
      return cSb.ToString();
    }

  }
}
