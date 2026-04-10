using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Weavers.Core.Entities {
  public class Item {
    public int Id { get; set; } = 0;
    public int ItemTypeId { get; set; } = 0;
    public string Name { get; set; }  = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Data { get; set; } = "{}";
    public DateTime Established { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;


    // Nav properties
    public ItemType ItemType { get; set; } = null!;
    public ICollection<Relation> Relations { get; set; } = [];
    public ICollection<Relation> IncomingRelations { get; set; } = [];
    public ICollection<ItemProperty> Properties { get; set; } = [];


    public Item() { }

  }

  public class ItemConfiguration : IEntityTypeConfiguration<Item> {
    public void Configure(EntityTypeBuilder<Item> builder) {
      builder.ToTable("Items");
      builder.HasKey(x => x.Id);

      builder.Property(x => x.Id).ValueGeneratedOnAdd();
      builder.Property(x => x.Name).IsRequired().HasMaxLength(500);
      builder.Property(x => x.Description).HasMaxLength(-1);
      builder.Property(x => x.Data).HasColumnType("nvarchar(max)").HasDefaultValue("{}");
      builder.Property(x => x.IsActive).HasDefaultValue(true);
      builder.Property(x => x.Established).IsRequired().HasDefaultValueSql("GETUTCDATE()");

      builder.HasOne(x => x.ItemType).WithMany(x => x.Items).HasForeignKey(x => x.ItemTypeId).OnDelete(DeleteBehavior.Restrict);
      builder.HasMany(m => m.Properties).WithOne(p => p.Item).HasForeignKey(p => p.ItemId).OnDelete(DeleteBehavior.Cascade);
      }
  }


}
