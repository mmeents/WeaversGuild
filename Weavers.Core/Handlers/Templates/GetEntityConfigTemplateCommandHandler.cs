using MediatR;
using System.Text;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace Weavers.Core.Handlers.Templates {
  public record GetEntityConfigTemplateCommand(int EntityCfgItemId) : IRequest<string>;

  public class GetEntityConfigTemplateCommandHandler : IRequestHandler<GetEntityConfigTemplateCommand, string> {
    private readonly FabricDbContext _context;
    public GetEntityConfigTemplateCommandHandler(FabricDbContext context) { 
      _context = context;
    }

    public async Task<string> Handle(GetEntityConfigTemplateCommand request, CancellationToken ct) {
      
      var cfgItem = await _context.GetItemDtoById(request.EntityCfgItemId, ct);
      if (cfgItem == null) return "entity config template not found.";
      
      var itemId = cfgItem.IncomingRelations.FirstOrDefault(r => r.ItemId != cfgItem.Id)?.ItemId;
      if (itemId == null || itemId <= 0)  return "entity not found."; 

      var item = await _context.GetItemDtoById(itemId.Value, ct);
      if (item == null) return "entity not found.";
      string dbTableName = item.Properties.FirstOrDefault(p => p.Name == Cx.ItDbTableName)?.Value?.ToString() ?? $"{item.Name}Set";
      string schemaName = item.Properties.FirstOrDefault(p => p.Name == Cx.ItDbSchema)?.Value?.ToString() ?? "dbo";
      string itemClassName = item.Name;

      var cSb = new StringBuilder();      
      var sbProperties = new StringBuilder();
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

        if (!printedComment) {
          sbNavs.AppendLine("    // Inbound nav properties");
          printedComment = true;
        }
                
        var inverseNavName = navItem.Properties.FirstOrDefault(p => p.Name == Cx.ItInverseNavigation)?.Value?.ToString() ?? "MissingInverse";
        var thisNavName = navItem.Name;

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
        var fkProp = navItem.Properties.FirstOrDefault(p => p.Name == Cx.ItForeignKey);
        var fkId = fkProp != null && fkProp.Value != null && int.TryParse(fkProp.Value, out var tempFkId) ? tempFkId : 0;
        var fkItem = fkId > 0 ? await _context.GetItemDtoById(fkId, ct) : null;
        var fkName = fkItem?.Name ?? $"{thisNavName}Id";        

        string navConfig = (WeItemType)navTypeId switch {
          WeItemType.NavHasOneToOne =>
              $"builder.HasOne(x => x.{propClassStr}).WithOne(y => y.{itemClassName}).HasForeignKey<{propClassStr}>(y => y.{fkName});",

          WeItemType.NavHasOneToMany =>
              $"builder.HasOne(x => x.{propClassStr}).WithMany(y => y.{propClassDbTableName}).HasForeignKey(y => y.{fkName});",

          WeItemType.NavHasManyToOne =>   // Most important one for inbound references
              $"builder.HasMany(x => x.{propClassDbTableName}).WithOne(y => y.{itemClassName}).HasForeignKey(y => y.{fkName});",

          WeItemType.NavHasManyToMany =>
              $"builder.HasMany(x => x.{propClassDbTableName}).WithMany(y => y.{propClassDbTableName});",

          _ => $"// Unknown navigation: {thisNavName}"
        };

        sbNavs.AppendLine($"    {navConfig}");        
      } 

      cSb.AppendLine("using Microsoft.EntityFrameworkCore;");
      cSb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
      cSb.AppendLine("");
      cSb.AppendLine($"namespace {namespaceName} {{");
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
      cSb.AppendLine($"}}");
      
      return cSb.ToString();
    }

  }
}
