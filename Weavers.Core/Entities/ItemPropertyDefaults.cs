using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weavers.Core.Extensions;

namespace Weavers.Core.Entities {
  public class ItemPropertyDefault {
    public int Id { get; set; } = 0;
    public int ItemTypeId { get; set; } = 0;
    public int Rank { get; set; } = 1;
    public string Key { get; set; } = string.Empty;
    public string DefaultValue { get; set; } = string.Empty;
    public int? ValueDataTypeId { get; set; } = null;   // FK → DataType (WeDataType enum)
    public int? ReferenceItemTypeId { get; set; } = null;
    public int? EditorTypeId { get; set; } = null;  
    public bool IsRequired { get; set; } = false;
    public bool IsVisible { get; set; } = true;
    public bool IsReadOnly { get; set; } = false;

    // Nav
    public ItemType ItemType { get; set; } = null!;
    public DataType? ValueDataType { get; set; }
    public EditorType? Editor { get; set; }
    public ItemType? ReferenceItemType { get; set; }
  }


  public class ItemPropertyDefaultConfiguration : IEntityTypeConfiguration<ItemPropertyDefault> {
    public void Configure(EntityTypeBuilder<ItemPropertyDefault> builder) {
      builder.ToTable("ItemPropertyDefaults");
      builder.HasKey(x => x.Id);
      builder.Property(x => x.Id).ValueGeneratedOnAdd();
      builder.Property(x => x.ItemTypeId).IsRequired();
      builder.Property(x => x.Rank).HasDefaultValue(1);
      builder.Property(x => x.Key).IsRequired().HasMaxLength(200);
      builder.Property(x => x.DefaultValue).HasMaxLength(-1).HasDefaultValue("");
      builder.Property(x => x.ValueDataTypeId).IsRequired(false);
      builder.Property(x => x.ReferenceItemTypeId).IsRequired(false);
      builder.Property(x => x.EditorTypeId).IsRequired(false);

      builder.Property(x => x.IsRequired).HasDefaultValue(false);
      builder.Property(x => x.IsVisible).HasDefaultValue(true);
      builder.Property(x => x.IsReadOnly).HasDefaultValue(false);

      builder.HasIndex(x => new { x.ItemTypeId, x.Key }).IsUnique();

      builder.HasOne(x => x.ItemType)
          .WithMany(x => x.PropertyDefaults)   // add collection to ItemType
          .HasForeignKey(x => x.ItemTypeId)
          .OnDelete(DeleteBehavior.Cascade);  // if type goes, schema goes

      builder.HasOne(x => x.ValueDataType)
          .WithMany()
          .HasForeignKey(x => x.ValueDataTypeId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);

      builder.HasOne(x => x.Editor)
          .WithMany()
          .HasForeignKey(x => x.EditorTypeId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);

      builder.HasOne(x => x.ReferenceItemType)
          .WithMany()
          .HasForeignKey(x => x.ReferenceItemTypeId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);

      var defaultData = ItemPropertyDefaultsExt.DefaultProps;
      var seedData = new List<ItemPropertyDefault>();
      var idCounter = 0;
      foreach (var itemType in defaultData.Keys) { 
        var item = defaultData[itemType];
        foreach(var prop in item) { 
          idCounter++;
          seedData.Add(new ItemPropertyDefault {
            Id = idCounter,
            ItemTypeId = (int)itemType,
            Rank = prop.Rank,
            Key = prop.Key,
            DefaultValue = prop.DefaultValue,
            ValueDataTypeId = prop.ValueDataTypeId,
            ReferenceItemTypeId = prop.ReferenceItemTypeId,
            EditorTypeId = prop.EditorTypeId,
            IsRequired = prop.IsRequired,
            IsVisible = prop.IsVisible,
            IsReadOnly = prop.IsReadOnly
          });
        }
      }
      
      builder.HasData( seedData );
    }
  }


}
