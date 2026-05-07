using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Entities {

  public class BuildFile {
    public int Id { get; set; }
    public int BuildId { get; set; }
    public int ItemId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public bool WasWritten { get; set; }
    public bool WasDeleted { get; set; }

    // Nav properties
    public Build? Build { get; set; } = null;
    public Item? Item { get; set; } = null;

  }

  public class BuildFileConfiguration : IEntityTypeConfiguration<BuildFile> {
    public void Configure(EntityTypeBuilder<BuildFile> builder) {
      builder.ToTable("BuildFiles");

      builder.HasKey(x => x.Id);

      builder.Property(x => x.Id).ValueGeneratedOnAdd();
      builder.Property(x => x.BuildId).HasColumnType("int").IsRequired();
      builder.Property(x => x.ItemId).HasColumnType("int").IsRequired();
      builder.Property(x => x.FilePath).HasColumnType("nvarchar(500)").IsRequired();
      builder.Property(x => x.WasWritten).HasColumnType("bit").IsRequired();
      builder.Property(x => x.WasDeleted).HasColumnType("bit").IsRequired();



    }
  }
}
