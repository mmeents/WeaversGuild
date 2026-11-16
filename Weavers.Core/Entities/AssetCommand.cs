using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Entities {

  public class AssetCommand {
    public int Id { get; set; }
    public int AssetPlanId { get; set; }
    public byte Kind { get; set; }                 // Replicate, Offload
    public byte Status { get; set; }               // Pending, Claimed, Done, Failed, Skipped
    public byte Step { get; set; }                 // Offload: Copied -> Verified -> SourceDeleted

    public int MediaAssetId { get; set; }          // expected hash; unique per plan
    public long Bytes { get; set; }                // plan totals + byte-based progress, no join

    public int SourceHarnessId { get; set; }
    public int SourceArchiveId { get; set; }
    public int SourceArchiveAssetId { get; set; }

    public int TargetHarnessId { get; set; }       // executor (pull model): claim filters on this
    public int TargetArchiveId { get; set; }
    public string TargetRelPath { get; set; } = "";   // intended; actual lands on the new ArchiveAsset
    public int? TargetArchiveAssetId { get; set; }

    public string? ClaimedBy { get; set; }         // INSTANCE id, not harness id
    public DateTime? LeaseUntilUtc { get; set; }
    public DateTime? NotBeforeUtc { get; set; }    // retry backoff
    public int Attempts { get; set; }
    public string? LastError { get; set; }         // nvarchar(1000)
    public DateTime CreatedUtc { get; set; }       // default SYSUTCDATETIME()
    public DateTime? DoneUtc { get; set; }

    public AssetPlan AssetPlan { get; set; } = null!;

  }

  public enum AssetCommandKind : byte { Replicate = 1, Offload = 2 }
  public enum AssetCommandStatus : byte { Pending = 0, Claimed = 1, Done = 2, Failed = 3, Skipped = 4 }
  public enum AssetCommandStep : byte { None = 0, Copied = 1, Verified = 2, SourceDeleted = 3 }

  public class AssetCommandConfig : IEntityTypeConfiguration<AssetCommand> {
    public void Configure(EntityTypeBuilder<AssetCommand> b) {
      b.ToTable("AssetCommands");
      b.HasKey(x => x.Id);
      b.Property(x => x.Id).UseIdentityColumn();

      b.Property(x => x.Kind).IsRequired();
      b.Property(x => x.Status).HasDefaultValue((byte)AssetCommandStatus.Pending);
      b.Property(x => x.Step).HasDefaultValue((byte)AssetCommandStep.None);

      b.Property(x => x.MediaAssetId).IsRequired();
      b.Property(x => x.Bytes).IsRequired();

      b.Property(x => x.SourceHarnessId).IsRequired();
      b.Property(x => x.SourceArchiveId).IsRequired();
      b.Property(x => x.SourceArchiveAssetId).IsRequired();

      b.Property(x => x.TargetHarnessId).IsRequired();
      b.Property(x => x.TargetArchiveId).IsRequired();
      b.Property(x => x.TargetRelPath)
        .IsRequired()
        .HasMaxLength(1024);                       // nvarchar(1024); not indexed, so no key-size issue

      b.Property(x => x.ClaimedBy)
        .HasMaxLength(64)
        .IsUnicode(false);                         // instance/session id (Guid string fits)
      b.Property(x => x.LeaseUntilUtc).HasColumnType("datetime2(0)");
      b.Property(x => x.NotBeforeUtc).HasColumnType("datetime2(0)");
      b.Property(x => x.Attempts).HasDefaultValue(0);
      b.Property(x => x.LastError).HasMaxLength(1000);

      b.Property(x => x.CreatedUtc).HasColumnType("datetime2(0)")
        .HasDefaultValueSql("SYSUTCDATETIME()");
      b.Property(x => x.DoneUtc).HasColumnType("datetime2(0)");

      // AssetPlan relationship is configured from the plan side (cascade).

      // FK constraints, no navigation properties on either side.
      b.HasOne<MediaAsset>().WithMany()
        .HasForeignKey(x => x.MediaAssetId)
        .OnDelete(DeleteBehavior.Restrict);

      b.HasOne<ArchiveAsset>().WithMany()
        .HasForeignKey(x => x.SourceArchiveAssetId)
        .OnDelete(DeleteBehavior.Restrict);

      // Nullable result link. NoAction rather than SetNull: SetNull would add a second
      // cascade-style path through ArchiveAssets and SQL Server rejects it.
      b.HasOne<ArchiveAsset>().WithMany()
        .HasForeignKey(x => x.TargetArchiveAssetId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.NoAction);

      b.HasOne<Item>().WithMany()
        .HasForeignKey(x => x.SourceHarnessId)
        .OnDelete(DeleteBehavior.Restrict);

      b.HasOne<Item>().WithMany()
        .HasForeignKey(x => x.SourceArchiveId)
        .OnDelete(DeleteBehavior.Restrict);

      b.HasOne<Item>().WithMany()
        .HasForeignKey(x => x.TargetHarnessId)
        .OnDelete(DeleteBehavior.Restrict);

      b.HasOne<Item>().WithMany()
        .HasForeignKey(x => x.TargetArchiveId)
        .OnDelete(DeleteBehavior.Restrict);

      // Claim query: WHERE TargetHarnessId = @me AND Status IN (Pending, Claimed)
      //              AND (NotBeforeUtc IS NULL OR NotBeforeUtc <= now)
      b.HasIndex(x => new { x.TargetHarnessId, x.Status, x.NotBeforeUtc })
        .IncludeProperties(x => new { x.LeaseUntilUtc, x.Attempts })
        .HasDatabaseName("IX_AssetCommands_Claim");

      // Plan progress: counts and bytes by status.
      b.HasIndex(x => new { x.AssetPlanId, x.Status })
        .IncludeProperties(x => x.Bytes)
        .HasDatabaseName("IX_AssetCommands_Plan_Status");

      // A plan never holds two commands for the same content.
      b.HasIndex(x => new { x.AssetPlanId, x.MediaAssetId })
        .IsUnique()
        .HasDatabaseName("UX_AssetCommands_Plan_MediaAsset");

      // Note: EF creates a plain index for each FK above. The FK indexes on SourceHarnessId /
      // TargetHarnessId / TargetArchiveId are mostly redundant with the claim index; harmless at
      // this size, drop them from the migration later if insert speed on big plans matters.
    }
  }

}
