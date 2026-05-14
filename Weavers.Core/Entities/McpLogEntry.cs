

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Weavers.Core.Entities {
  public class McpLogEntry {
    public int Id { get; set; } = 0;
    public DateTime CalledAt { get; set; } = DateTime.MinValue;
    public string OpName { get; set; } = string.Empty;
    public string InputJson { get; set; } = string.Empty;
    public string? OutputJson { get; set; }
    public int DurationMs { get; set; } = 0;
    public bool Success { get; set; } = false;
    public string? ErrorMsg { get; set; }

  }

  public class McpLogEntryConfiguration : IEntityTypeConfiguration<McpLogEntry> {
    public void Configure(EntityTypeBuilder<McpLogEntry> builder) {
      builder.ToTable("McpLogEntries");

      builder.HasKey(x => x.Id);

      builder.Property(x => x.Id).ValueGeneratedOnAdd();
      builder.Property(x => x.CalledAt).HasColumnType("datetime2").IsRequired();
      builder.Property(x => x.OpName).HasColumnType("nvarchar(200)").IsRequired();
      builder.Property(x => x.InputJson).HasColumnType("nvarchar(max)").IsRequired();
      builder.Property(x => x.OutputJson).HasColumnType("nvarchar(max)");
      builder.Property(x => x.DurationMs).HasColumnType("int").IsRequired();
      builder.Property(x => x.Success).HasColumnType("bit").IsRequired();
      builder.Property(x => x.ErrorMsg).HasColumnType("nvarchar(max)");

    }
  }

}

