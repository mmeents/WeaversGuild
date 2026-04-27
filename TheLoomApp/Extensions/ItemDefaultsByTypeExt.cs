using System.Text;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;

namespace TheLoomApp.Extensions {
  public static class ItemDefaultsByTypeExt {
    public static string GetTypeTemplate(this ItemDto folerItem, WeItemType targetType) {
      switch ((WeItemType)targetType) {
        case WeItemType.FileModel:
          return "";
        case WeItemType.LibraryModel:
          return GetLibraryModelTemplate(null);
        case WeItemType.DependencyInjectionModel:
          return GetDependencyInjectionTemplate(folerItem, false, false);
        default:
          return "";
      }
    }

    public static string GetLibraryModelTemplate(ItemDto? libraryNode) {
      if (libraryNode == null) { // empty        
        return GetLibraryTemplateEmpty();
      } else {
        if (libraryNode.Relations.Any()) {
        }
        return GetLibraryTemplateEmpty();
      }
    }

    public static string GetLibraryTemplateEmpty() {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">" + Environment.NewLine);
      sb.AppendLine("  <PropertyGroup>");
      sb.AppendLine("    <TargetFramework>net9.0</TargetFramework >");
      sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings >");
      sb.AppendLine("    <Nullable>enable</Nullable>");
      sb.AppendLine("  </PropertyGroup>" + Environment.NewLine);
      sb.AppendLine("</Project>");
      return sb.ToString();
    }

    public static string GetDependencyInjectionTemplate(ItemDto itemParent, bool HasDbContext, bool hasMediatR) {
      if (itemParent == null) { return ""; }
      var namespaceName = itemParent.ResolveParentNamespace(itemParent.Name);      
      var libName = namespaceName.UrlSafe().Trim();
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
      sb.AppendLine($"namespace {namespaceName} {{"+Environment.NewLine);

      sb.AppendLine("  public static class DependencyInjection {");      
      if (HasDbContext) {
      sb.AppendLine($"    public static IServiceCollection Add{libName}<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext {{");
      sb.AppendLine( "      services.AddDbContext<TContext>(options => options.UseSqlServer(configuration.GetConnectionString(\"DefaultConnection\")));");
      } else {
      sb.AppendLine($"    public static IServiceCollection Add{libName}(this IServiceCollection services) {{"); 
      }
      if (hasMediatR) {
      sb.AppendLine("      services.AddMediatR(cfg => {\r\n          cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);\r\n      });");
      }     
     
      sb.AppendLine("      return services;");
      sb.AppendLine("    }");
      sb.AppendLine("  }");
      sb.AppendLine("}");
      return sb.ToString();
    }

    


  }
}
