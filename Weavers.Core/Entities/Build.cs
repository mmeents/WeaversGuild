using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Entities {
  public class Build {
    public int Id { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int Status { get; set; }
    public string? BuildOutput { get; set; }
    public string? CompilerOutput { get; set; }
    public int LibraryItemId { get; set; }

    // Nav properties
    public Item? LibraryItem { get; set; } = null;
    // Inbound nav properties
    public ICollection<BuildFile> BuildFiles { get; set; } = [];

  }

  public class BuildConfiguration : IEntityTypeConfiguration<Build> {
    public void Configure(EntityTypeBuilder<Build> builder) {
      builder.ToTable("Builds");

      builder.HasKey(x => x.Id);

      builder.Property(x => x.Id).ValueGeneratedOnAdd();
      builder.Property(x => x.StartedAt).HasColumnType("datetime").IsRequired();
      builder.Property(x => x.CompletedAt).HasColumnType("datetime");
      builder.Property(x => x.Status).HasColumnType("int").IsRequired();
      builder.Property(x => x.BuildOutput).HasColumnType("nvarchar(max)");
      builder.Property(x => x.CompilerOutput).HasColumnType("nvarchar(max)");
      builder.Property(x => x.LibraryItemId).HasColumnType("int").IsRequired();


      // Inbound nav properties
      builder.HasMany(x => x.BuildFiles).WithOne(y => y.Build).HasForeignKey(y => y.BuildId).OnDelete(DeleteBehavior.Cascade);

    }
  }
}
