using Microsoft.EntityFrameworkCore;  
using Weavers.Core.Entities;

namespace Weavers.Core {
  public class FabricDbContext : DbContext {
    protected FabricDbContext(DbContextOptions options) : base(options) {}
    public FabricDbContext(DbContextOptions<FabricDbContext> options) : base(options) { }

    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<DataType> DataTypes => Set<DataType>();
    public DbSet<EditorType> EditorTypes => Set<EditorType>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemProperty> ItemProperties => Set<ItemProperty>();
    public DbSet<ItemPropertyDefault> ItemPropertyDefaults => Set<ItemPropertyDefault>();
    public DbSet<ItemType> ItemTypes => Set<ItemType>();
    public DbSet<Relation> Relations => Set<Relation>();
    public DbSet<RelationType> RelationTypes => Set<RelationType>();


    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      base.OnModelCreating(modelBuilder);
      modelBuilder.ApplyConfigurationsFromAssembly(typeof(FabricDbContext).Assembly);
    }
  }
}
