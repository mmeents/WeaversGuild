using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Entities {
  public class AssetPlan {
    public int Id { get; set; }
    public int PolicyItemId { get; set; }
    public byte Kind { get; set; }
    public byte Status { get; set; }
    public int CommandCount { get; set; }
    public long Bytes { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? ApprovedUtc { get; set; }
    public DateTime? StartedUtc { get; set; }
    public DateTime? DoneUtc { get; set; }

    public string PlanSummary { get; set; } = string.Empty;
    public string ResultSummary { get; set; } = string.Empty;

    // Nav properties
    public Item PolicyItem { get; set; } = null!;
    public List<AssetCommand> Commands { get; set; } = new List<AssetCommand>();
  }

  public enum AssetPlanKind : byte { Replica = 0, Offload = 1 }
  public enum AssetPlanStatus : byte { Draft = 0, Approved = 1, Running = 2, Paused = 3, Done = 4, Failed = 5, Cancelled = 6 }


  public class AssetPlanConfig : IEntityTypeConfiguration<AssetPlan> {
    public void Configure(EntityTypeBuilder<AssetPlan> b) {
      b.ToTable("AssetPlans");
      b.HasKey(x => x.Id);
      b.Property(x => x.Id).UseIdentityColumn();

      b.Property(x => x.PolicyItemId).IsRequired();
      b.Property(x => x.Kind).IsRequired();
      b.Property(x => x.Status).HasDefaultValue((byte)AssetPlanStatus.Draft);
      b.Property(x => x.CommandCount).HasDefaultValue(0);
      b.Property(x => x.Bytes).HasDefaultValue(0L);

      b.Property(x => x.CreatedUtc)
        .HasColumnType("datetime2(0)")
        .HasDefaultValueSql("SYSUTCDATETIME()");
      b.Property(x => x.ApprovedUtc).HasColumnType("datetime2(0)");
      b.Property(x => x.StartedUtc).HasColumnType("datetime2(0)");
      b.Property(x => x.DoneUtc).HasColumnType("datetime2(0)");
      b.Property(x => x.PlanSummary).HasColumnType("nvarchar(max)").IsRequired(false);
      b.Property(x => x.ResultSummary).HasColumnType("nvarchar(max)").IsRequired(false);

      // Nav on the plan only; no AssetPlans collection added to Item.
      b.HasOne(x => x.PolicyItem)
        .WithMany()
        .HasForeignKey(x => x.PolicyItemId)
        .OnDelete(DeleteBehavior.Restrict);

      b.HasMany(x => x.Commands)
        .WithOne(c => c.AssetPlan)
        .HasForeignKey(c => c.AssetPlanId)
        .OnDelete(DeleteBehavior.Cascade);

      // "Latest plan for this policy" / "is one already running?"
      b.HasIndex(x => new { x.PolicyItemId, x.Status })
        .HasDatabaseName("IX_AssetPlans_Policy_Status");
    }
  }

}
