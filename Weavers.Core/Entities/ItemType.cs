using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace Weavers.Core.Entities {
  public class ItemType {
    public int Id { get; set; } = 0;
    public int? ParentTypeId { get; set; }
    public int? EditorTypeId { get; set; }
    public int Rank { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = false;
    public bool IsReadOnly { get; set; } = false;
    public string IconName { get; set; } = string.Empty;  


    // Nav 
    public ItemType? ParentType { get; set; }
    public ICollection<ItemType> ChildTypes { get; set; } = [];    
    public EditorType? Editor { get; set; }
    public ICollection<ItemPropertyDefault> PropertyDefaults { get; set; } = [];
    public ICollection<Item> Items { get; set; } = [];
  }


  public class ItemTypeConfiguration : IEntityTypeConfiguration<ItemType> {
    public void Configure(EntityTypeBuilder<ItemType> builder) {
      builder.ToTable("ItemTypes");
      builder.HasKey(x => x.Id);
      builder.Property(x => x.Id).ValueGeneratedOnAdd();
      builder.Property(x => x.Name).IsRequired().HasMaxLength(500);
      builder.Property(x => x.Description).HasMaxLength(-1);
      builder.Property(x => x.IsVisible).IsRequired().HasDefaultValue(false);
      builder.Property(x => x.IsReadOnly).IsRequired().HasDefaultValue(false);
      builder.Property(x => x.IconName).IsRequired()
        .HasMaxLength(50).HasDefaultValue(string.Empty);

      builder.HasOne(t => t.ParentType)
        .WithMany(t => t.ChildTypes)
        .HasForeignKey(t => t.ParentTypeId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired(false);

      builder.HasOne(t => t.Editor)
        .WithMany()
        .HasForeignKey(t => t.EditorTypeId)
        .OnDelete(DeleteBehavior.SetNull)
        .IsRequired(false);

      builder.HasIndex(x => x.ParentTypeId);
      builder.HasIndex(x => new { x.ParentTypeId, x.Rank });
      builder.HasIndex(x => x.EditorTypeId);
      builder.HasIndex(x => x.Name).IsUnique();


      // Seeding logic: Iterate through the Enum to create the entity data
      var seedData = Enum.GetValues(typeof(WeItemType))
          .Cast<WeItemType>()
          .Select(e => new ItemType {
            Id = (int)e,
            ParentTypeId = (int?)e.ParentType(),
            EditorTypeId = e.DefaultEditorTypeId(),
            Rank = e.DefaultRank(),
            Name = e.ToString(),
            Description = e.Description(),
            IsVisible = true,
            IsReadOnly = false,
            IconName = e.DefaultIconName()
          });

      builder.HasData(seedData);
    }
  }

}
