using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Weavers.Core.Entities {
  public  class ItemProperty {
    public int Id { get; set; }
    public int? ItemPropertyDefaultId { get; set; }
    public int ItemId { get; set; }    
    public string Name { get; set; } = string.Empty;    
    public string? Value { get; set; }
    public int? ValueDataTypeId { get; set; }
    public int? ReferenceItemTypeId { get; set; }
    public int? EditorTypeId { get; set; }
    public bool IsRequired { get; set; } = false;
    public bool IsVisible { get; set; } = true;
    public bool IsReadOnly { get; set; } = false;


    // Navigation properties
    public Item Item { get; set; } = null!;
    public DataType? ValueType { get; set; }
    public EditorType? Editor { get; set; }
    public ItemType? ReferenceItemType { get; set; }
    public ItemPropertyDefault? ItemPropertyDefault { get; set; }

    // EF Core constructor
    public ItemProperty() { }

    public ItemProperty(
      int itemId,
      string name,
      string? value = null,
      int? valueDataTypeId = null,
      int? referenceItemTypeId = null,
      int? editorTypeId = null
    ) {
      ItemId = itemId;
      Name = name;
      Value = value;
      ValueDataTypeId = valueDataTypeId;
      ReferenceItemTypeId = referenceItemTypeId;
      EditorTypeId = editorTypeId;
    }

    public void Update(string? value, int? valueDataTypeId = null, int? referenceItemTypeId = null, int? editorTypeId = null) {
      Value = value;
      if (valueDataTypeId.HasValue) {
        ValueDataTypeId = valueDataTypeId;
      }
      if (referenceItemTypeId.HasValue) {
        ReferenceItemTypeId = referenceItemTypeId;
      } 
      if (editorTypeId.HasValue) {
        EditorTypeId = editorTypeId;
      }
    }
  }

  public class ItemPropertyConfiguration : IEntityTypeConfiguration<ItemProperty> {
    public void Configure(EntityTypeBuilder<ItemProperty> builder) {
      builder.HasKey(p => p.Id);

      builder.Property(p => p.Name).IsRequired().HasMaxLength(100);

      builder.Property(p => p.Value).HasMaxLength(-1).IsRequired(false);

      builder.Property(p => p.ValueDataTypeId).IsRequired(false);
      
      builder.Property(p => p.ReferenceItemTypeId).IsRequired(false);
      
      builder.Property(p => p.ItemPropertyDefaultId).IsRequired(false);

      builder.Property(p => p.EditorTypeId).IsRequired(false);

      // Unique constraint on ItemId + Name
      builder.HasIndex(p => new { p.ItemId, p.Name }).IsUnique();
      
      
      builder.HasOne(p => p.ValueType)
          .WithMany()
          .HasForeignKey(p => p.ValueDataTypeId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);

      builder.HasOne(p => p.ReferenceItemType)
          .WithMany()
          .HasForeignKey(p => p.ReferenceItemTypeId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);

      builder.HasOne(p => p.Editor)
          .WithMany()
          .HasForeignKey(p => p.EditorTypeId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);

      builder.HasOne(p => p.ItemPropertyDefault)
          .WithMany()
          .HasForeignKey(p => p.ItemPropertyDefaultId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);

      // Indexes
      builder.HasIndex(p => p.ItemId);
      builder.HasIndex(p => p.Name);
      builder.HasIndex(p => p.ValueDataTypeId);
      builder.HasIndex(p => p.ReferenceItemTypeId);
      builder.HasIndex(p => p.EditorTypeId);
    }
  }



}
