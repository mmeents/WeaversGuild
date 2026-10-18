using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Weavers.Core.Entities {
  public class AppPeer {  
    public int Id { get; set; } = 0;
    public string Machine { get; set; } = "";
    public int HarnessAppSessionId { get; set; } = 0;    
    public int ProcessNum { get; set; } = 0;
    public string Address { get; set; } = "";
    public int Port { get; set; } = 0;     
    public DateTime StartedAt { get; set; }
    public DateTime HeartBeat { get; set; }
    public byte[]? TokenHash { get; set; }

    // Nav
    public Item HarnessAppSession { get; set; } = null!;

  }

  public class AppPeerConfiguration : IEntityTypeConfiguration<AppPeer> {
    public void Configure(EntityTypeBuilder<AppPeer> builder) {
      builder.ToTable("AppPeers");
      builder.HasKey(ap => ap.Id);
      builder.Property(ap => ap.Machine).HasMaxLength(255);
      builder.Property(ap => ap.Address).HasMaxLength(255);
      builder.Property(ap => ap.StartedAt).HasDefaultValueSql("SYSUTCDATETIME()");
      builder.Property(ap => ap.HeartBeat).HasDefaultValueSql("SYSUTCDATETIME()");
      builder.Property(ap => ap.TokenHash).HasColumnType("varbinary(32)").IsRequired(false); 
      builder.HasOne(ap => ap.HarnessAppSession).WithMany().HasForeignKey(ap => ap.HarnessAppSessionId);
      builder.HasIndex(ap => ap.HarnessAppSessionId).IsUnique();
    }
  }

}
