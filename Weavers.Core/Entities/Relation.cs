using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Entities {

  public class Relation {
    public int Id { get; set; } = 0;
    public int ItemId { get; set; } = 0;
    public int RelationTypeId { get; set; } = 0;
    public int? RelatedItemId { get; set; } = null;
    public DateTime Established { get; set; } = DateTime.UtcNow;
    public int? Rank { get; set; }

    // Nav properties
    public Item Item { get; set; } = null!;
    public Item? RelatedItem { get; set; } = null!;
    public RelationType RelationType { get; set; } = null!;

    public Relation() { }

  }


  public class RelationConfiguration : IEntityTypeConfiguration<Relation> {
    public void Configure(EntityTypeBuilder<Relation> builder) {
      builder.ToTable("Relations");
      builder.HasKey(x => x.Id);

      builder.Property(x => x.Id).ValueGeneratedOnAdd();

      builder.Property(x => x.Established).IsRequired().HasDefaultValueSql("GETUTCDATE()");

      builder.Property(x => x.Rank).IsRequired(false);

      builder.HasOne(x => x.Item)
        .WithMany(x => x.Relations)
        .HasForeignKey(x => x.ItemId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(x => x.RelatedItem)
        .WithMany(x => x.IncomingRelations)
        .HasForeignKey(x => x.RelatedItemId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(x => x.RelationType)
        .WithMany(x => x.Relations)
        .HasForeignKey(x => x.RelationTypeId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasIndex(x => new { x.ItemId, x.RelationTypeId, x.RelatedItemId }).IsUnique();
    }
  }


}
