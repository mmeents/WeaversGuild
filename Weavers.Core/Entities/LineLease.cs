using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Entities {
  public class LineLease {
      public string Machine { get; set; } = "";      // PK
      public string Line { get; set; } = "";
      public int HarnessAppSessionId { get; set; }
      public DateTime AcquiredAt { get; set; }
  }

  public class LineLeaseConfiguration : IEntityTypeConfiguration<LineLease> {
    public void Configure(EntityTypeBuilder<LineLease> builder) {
      builder.ToTable("LineLeases");
      builder.HasKey(x => x.Machine);
      builder.Property(x => x.Machine).HasMaxLength(255);
      builder.Property(x => x.Line).HasMaxLength(50);
      builder.Property(x => x.AcquiredAt).HasDefaultValueSql("SYSUTCDATETIME()");
    }
  }

}
