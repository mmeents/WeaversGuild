using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;


namespace Weavers.Core.Handlers.Templates {

  public record GetDependencyInjectionTemplateCommand(int DependencyInjectionItemId) : IRequest<string?>;
  public class GetDependencyInjectionTemplateCommandHandler(FabricDbContext context) : IRequestHandler<GetDependencyInjectionTemplateCommand, string?> {
    private readonly FabricDbContext _context = context;

    public async Task<string?> Handle(GetDependencyInjectionTemplateCommand request, CancellationToken cancellationToken) {
      
      var item = await _context.GetItemDtoById(request.DependencyInjectionItemId, cancellationToken);
      if (item == null) { return ""; }

      var namespaceName = item.ResolveParentNamespace(item.Name);
      var libName = namespaceName.UrlSafe();
      var HasDbProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItHasDbContext);
      bool hasDbContext = false;
      if (HasDbProp != null) { 
        hasDbContext = HasDbProp.Value.AsBoolean();
      }
      var hasMediatrProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItHasMediator);
      bool hasMediatR = false;
      if (hasMediatrProp != null) { 
        hasMediatR =  hasMediatrProp.Value.AsBoolean();
      }

      StringBuilder ic = new StringBuilder();
      StringBuilder sb = new StringBuilder();
      var namespaces = new HashSet<string>();
      if (hasDbContext){
        namespaces.Add("Microsoft.EntityFrameworkCore");
        namespaces.Add("Microsoft.Extensions.Configuration");
      }
      namespaces.Add("Microsoft.Extensions.DependencyInjection");

      sb.AppendLine($"namespace {namespaceName} {{" + Environment.NewLine);

      sb.AppendLine("  public static class DependencyInjection {");
      if (hasDbContext) {
        sb.AppendLine($"    public static IServiceCollection Add{libName}<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext {{");
        sb.AppendLine("      services.AddDbContext<TContext>(options => options.UseSqlServer(configuration.GetConnectionString(\"DefaultConnection\")));");
      } else {
        sb.AppendLine($"    public static IServiceCollection Add{libName}(this IServiceCollection services) {{");
      }
      if (hasMediatR) {
        sb.AppendLine("      services.AddMediatR(cfg => {\r\n          cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);\r\n      });");
      }
      var importList = item.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.DiImportModel);
      foreach (var Rel in importList) {
        if (Rel.RelatedItemId != null) { 
          var importItemId = Rel.RelatedItemId.Value;
          var importItem = await _context.GetItemDtoById(importItemId, cancellationToken);
          if (importItem != null) {

            var className = importItem.Name;
            var regInterfaceProp = importItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRegisterInterface);
            bool registerInterface = false;
            if (regInterfaceProp != null) {
              registerInterface = regInterfaceProp.Value.AsBoolean();
            }

            var regItemProp = importItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRegisterObject);
            int regItemId = 0;
            if (regItemProp != null) { 
              if (int.TryParse(regItemProp.Value,out regItemId)) { 
                var regItem = await _context.GetItemDtoById(regItemId, cancellationToken);
                if (regItem != null) { 
                  className = regItem.Name;
                  var classNamespaceProp = regItem.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespace);
                  if (classNamespaceProp != null&& !string.IsNullOrEmpty(classNamespaceProp.Value)) {
                    namespaces.Add(classNamespaceProp.Value);
                  }
                }
              }
            }

            var lifetimeProp = importItem.Properties.FirstOrDefault(p => p.Name == Cx.ItLifetimeScope);
            if (lifetimeProp != null) { 
              if (registerInterface) { 
                className = $"I{className}, {className}";
              }
              if (int.TryParse(lifetimeProp.Value,out var itemTypeId)) { 
                switch (itemTypeId) { 
                  case (int)WeItemType.CSLifetimeSingleton :
                    sb.AppendLine($"      services.AddSingleton<{className}>();");
                    break;
                  case (int)WeItemType.CSLifetimeScoped :
                    sb.AppendLine($"      services.AddScoped<{className}>();"); 
                    break;
                  case (int)WeItemType.CSLifetimeTransient :
                    sb.AppendLine($"      services.AddTransient<{className}>();");
                    break;
                  default:
                    break;
                }                 
              }
            }
          }
        }
      }

      sb.AppendLine("      return services;");
      sb.AppendLine("    }");
      sb.AppendLine("  }");
      sb.AppendLine("}");
      string namespaceImports = string.Join(Environment.NewLine, namespaces.Select(n => $"using {n};"));
      return namespaceImports + Environment.NewLine + sb.ToString();

    }
  }
}
