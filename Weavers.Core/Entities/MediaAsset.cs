using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Entities {

  public static class MediaAssetSchema {
    // 16 = xxHash128, 32 = SHA-256. Pick once; changing later means rehashing everything.
    public const int HashLength = 32;

    // UTF-8 varchar(max) halves storage vs nvarchar for the tag JSON. Requires SQL Server 2019+.
    // Swap to nvarchar(max) (drop UseCollation, IsUnicode(true)) if a target server is older.
    public const string Utf8Collation = "Latin1_General_100_CI_AS_SC_UTF8";
  }

  public class MediaAsset {
    public int Id { get; set; }
    public byte[] FileHash { get; set; } = [];      // binary(16) for xxHash128, binary(32) for SHA-256. Fixed width so it can be indexed.
    public long FileSize { get; set; }
    public string MediaType { get; set; } = "";     // "image/jpeg", from FileTypeDetector, nvarchar(50)
    public DateTime? CapturedLocal { get; set; }    // nullable: "unknown" is a real answer
    public short? CapturedOffsetMin { get; set; }   // from OffsetTimeOriginal when present
    public byte CapturedSource { get; set; }        // Exif / Xmp / QuickTime / FileName / Mtime
    public int? Width { get; set; }
    public int? Height { get; set; }
    public byte Orientation { get; set; }           // EXIF 1–8; or store post-rotation display dims
    public int? DurationMs { get; set; }            // null for stills
    public string? CameraModel { get; set; }        // common filter, worth a real column
    public double? Lat { get; set; }
    public double? Lon { get; set; }
    public long? PHash { get; set; }
    public string? GroupKey { get; set; }           // Live Photo HEIC+MOV, RAW+JPG pairing
    public string MediaTags { get; set; } = "";     // JSON, see below

    public ICollection<ArchiveAsset> Copies { get; set; } = [];
  }

  public class MediaAssetConfig : IEntityTypeConfiguration<MediaAsset> {
    public void Configure(EntityTypeBuilder<MediaAsset> b) {
      b.ToTable("MediaAssets");
      b.HasKey(x => x.Id);
      b.Property(x => x.Id).UseIdentityColumn();

      b.Property(x => x.FileHash)
        .IsRequired()
        .HasMaxLength(MediaAssetSchema.HashLength)
        .IsFixedLength();                                   // binary(32), indexable

      b.Property(x => x.FileSize).IsRequired();

      b.Property(x => x.MediaType)
        .IsRequired()
        .HasMaxLength(50)
        .IsUnicode(false);                                  // "image/jpeg", "video/quicktime"

      b.Property(x => x.CapturedLocal).HasColumnType("datetime2(3)");   // EXIF wall-clock, no zone
      b.Property(x => x.CapturedOffsetMin);
      b.Property(x => x.CapturedSource).HasDefaultValue((byte)0);

      b.Property(x => x.Width);
      b.Property(x => x.Height);
      b.Property(x => x.Orientation).HasDefaultValue((byte)1);   // 1 = normal
      b.Property(x => x.DurationMs);

      b.Property(x => x.CameraModel).HasMaxLength(100);
      b.Property(x => x.Lat);                               // float(53)
      b.Property(x => x.Lon);
      b.Property(x => x.PHash);                             // bigint, filled in a later pass

      b.Property(x => x.GroupKey)
        .HasMaxLength(128)
        .IsUnicode(false);

      b.Property(x => x.MediaTags)
        .IsRequired()
        .HasColumnType("varchar(max)")
        .IsUnicode(false)
        .UseCollation(MediaAssetSchema.Utf8Collation);

      // Identity: one row per unique content.
      b.HasIndex(x => x.FileHash)
        .IsUnique()
        .HasDatabaseName("UX_MediaAssets_FileHash");

      // Offload ordering (oldest first) and date browsing.
      b.HasIndex(x => x.CapturedLocal)
        .HasDatabaseName("IX_MediaAssets_CapturedLocal");

      // Live Photo / RAW+JPG pairing lookups.
      b.HasIndex(x => x.GroupKey)
        .HasFilter("[GroupKey] IS NOT NULL")
        .HasDatabaseName("IX_MediaAssets_GroupKey");
    }
  }


}
