using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Entities {
  public class ArchiveAsset {
    public int Id { get; set; }
    public int ArchiveId { get; set; }              // FK to Item (the AssetArchive)
    public int MediaAssetId { get; set; }
    public string RelPath { get; set; } = "";       // relative to archive root, not just the file name
    public DateTime FileMtimeUtc { get; set; }      // path+size+mtime unchanged = skip on rescan
    public DateTime LastSeenUtc { get; set; }
    public DateTime? LastVerifiedUtc { get; set; }
    public byte Status { get; set; }                // Present / Missing / Quarantined / Offloaded
    public Item AssetArchive { get; set; } = null!;
    public MediaAsset Asset { get; set; } = null!;
  }

  public class ArchiveAssetConfig : IEntityTypeConfiguration<ArchiveAsset> {
    // Shadow column: hash of the upper-cased path. Keeps the unique key small (nvarchar(1024)
    // would blow the 1700-byte index key limit) and case-insensitive like NTFS.
    public const string RelPathKey = "RelPathKey";

    public void Configure(EntityTypeBuilder<ArchiveAsset> b) {
      b.ToTable("ArchiveAssets");
      b.HasKey(x => x.Id);
      b.Property(x => x.Id).UseIdentityColumn();

      b.Property(x => x.ArchiveId).IsRequired();
      b.Property(x => x.MediaAssetId).IsRequired();

      b.Property(x => x.RelPath)
        .IsRequired()
        .HasMaxLength(800);                                // nvarchar(800), relative to archive root

      // datetime2(7) = full .NET tick precision, so the rescan "unchanged?" compare is exact.
      b.Property(x => x.FileMtimeUtc).HasColumnType("datetime2(7)");

      b.Property(x => x.LastSeenUtc)
        .HasColumnType("datetime2(0)")
        .HasDefaultValueSql("SYSUTCDATETIME()");
      b.Property(x => x.LastVerifiedUtc).HasColumnType("datetime2(0)");

      b.Property(x => x.Status).HasDefaultValue((byte)0);  // 0 = Present

      // Archive config lives in the graph. Restrict so removing/archiving an AssetArchive item
      // never silently wipes its catalog rows (and avoids multiple-cascade-path errors off Items).
      b.HasOne(x => x.AssetArchive)
        .WithMany(i => i.ArchiveAssets)
        .HasForeignKey(x => x.ArchiveId)
        .OnDelete(DeleteBehavior.Restrict);

      b.HasOne(x => x.Asset)
        .WithMany(i => i.Copies)
        .HasForeignKey(x => x.MediaAssetId)
        .OnDelete(DeleteBehavior.Restrict);

      // One row per path per archive.
      b.HasIndex(nameof(ArchiveAsset.ArchiveId), nameof(ArchiveAsset.RelPath))
        .IsUnique()
        .HasDatabaseName("UX_ArchiveAssets_Archive_RelPath");

      // Copies-per-asset, replica counting, "where else does this live?"
      b.HasIndex(x => x.MediaAssetId)
        .IncludeProperties(x => new { x.ArchiveId, x.Status })
        .HasDatabaseName("IX_ArchiveAssets_MediaAsset");

      // Per-archive scans: missing files, unverified, offload candidates.
      b.HasIndex(x => new { x.ArchiveId, x.Status })
        .IncludeProperties(x => new { x.MediaAssetId, x.LastVerifiedUtc })
        .HasDatabaseName("IX_ArchiveAssets_Archive_Status");
    }
  }
}
