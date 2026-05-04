using System.Text;
using MediatR;
using Weavers.Core.Extensions;
using Weavers.Core.Enums;
using Weavers.Core.Constants;

namespace Weavers.Core.Handlers.Templates {
    public record GetDbContextTemplateCommand(int DbContextItemId) : IRequest<string?>;

  public class GetDbContextTemplateCommandHandler(FabricDbContext context) : IRequestHandler<GetDbContextTemplateCommand, string?> {
    private readonly FabricDbContext _context = context;
    
    public async Task<string?> Handle(GetDbContextTemplateCommand request, CancellationToken cancellationToken) {
      
      var dbContextItem = await _context.GetItemDtoById(request.DbContextItemId, cancellationToken);
            
      StringBuilder sbMain = new StringBuilder();
      StringBuilder sbUse = new StringBuilder();

      HashSet<string> sbUses = new HashSet<string>();
      sbUses.Add("using Microsoft.EntityFrameworkCore;");      

      var importItemIds = dbContextItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.DbContextEntityImportModel)
        .Select(r => r.RelatedItemId).Where(r => r.HasValue).Select(v => v!.Value).ToList();
      string namespaceValue = dbContextItem.Properties.Where(p => p.Name == Cx.ItNamespace).FirstOrDefault()?.Value ?? "BadNamespace";      
      sbUses.Add(namespaceValue);

      sbMain.AppendLine($"namespace {namespaceValue} {{");
      sbMain.AppendLine($"  public class {dbContextItem.Name} : DbContext {{");
      sbMain.AppendLine( "    public " + dbContextItem.Name + "(DbContextOptions<" + dbContextItem.Name + "> options) : base(options) { }");
      sbMain.AppendLine($"    protected {dbContextItem.Name}(DbContextOptions options) : base(options) {{}}");

      foreach (var importId in importItemIds) { 
        var import = await _context.GetItemDtoById(importId, cancellationToken);      
        var classId = int.TryParse(import.Properties.Where(p => p.Name == Cx.ItRegisterObject).FirstOrDefault()?.Value, out var result) ? result : 0;
        if (classId > 0) { 
          var classItem = await _context.GetItemDtoById(classId, cancellationToken);
          var classNs = classItem.Properties.Where(p => p.Name == Cx.ItNamespace).FirstOrDefault()?.Value;
          if (!string.IsNullOrEmpty(classNs) && !sbUses.Contains(classNs)) {
            sbUse.AppendLine($"using {classNs};");
            sbUses.Add(classNs);
          }      
          sbMain.AppendLine($"      public DbSet<{classItem.Name}> {classItem.Name}Set => Set<{classItem.Name}>();");
        }
      }
      sbMain.AppendLine("\r\n"+
        "    protected override void OnModelCreating(ModelBuilder modelBuilder) {\r\n"+
        "      base.OnModelCreating(modelBuilder);\r\n"+
        "      modelBuilder.ApplyConfigurationsFromAssembly(typeof("+ dbContextItem.Name + ").Assembly);\r\n"+
        "    }");
      sbMain.AppendLine($"  }}");
      sbMain.AppendLine($"}}");                  

      return sbUse.ToString() 
        + sbMain.ToString();
    }

  }
}
