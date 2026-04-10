using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weavers.Core.Enums;

namespace Weavers.Core.Entities {
  public class RelationType {
    public int Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Nav properties
    public ICollection<Relation> Relations { get; set; } = [];
    public RelationType() { }
  }


  public class RelationTypeConfiguration : IEntityTypeConfiguration<RelationType> {
    public void Configure(EntityTypeBuilder<RelationType> builder) {
      builder.ToTable("RelationTypes");
      builder.HasKey(x => x.Id);
      builder.Property(x => x.Id).ValueGeneratedOnAdd();
      builder.Property(x => x.Name).IsRequired().HasMaxLength(500);
      builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
      builder.HasIndex(x => x.Name).IsUnique();

      builder.HasData(
        new RelationType { Id = (int)WeRelationTypes.TypeOf, Name = "TypeOf", Description = "Item belongs to a type category" },
        new RelationType { Id = (int)WeRelationTypes.Contains , Name = "Contains", Description = "Structural parent contains child model" }
      );

    }
  }
}
