using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;

namespace Weavers.Core.Models {
  public class ArchiveAssetDto {
    public int Id { get; set; }
    public int ArchiveId { get; set; }
    public int MediaAssetId { get; set; }
    public string RelPath { get; set; } = string.Empty;    
    public DateTime FileMtimeUtc { get; set; }      // path+size+mtime unchanged = skip on rescan
    public DateTime LastSeenUtc { get; set; }
    public DateTime? LastVerifiedUtc { get; set; }
    public byte Status { get; set; }
    public ItemDto AssetArchive { get; set; } = null!;
    public MediaAssetDto? MediaAsset { get; set; } = null;
  }

  public static class ArchiveAssetDtoExtensions {
    public static ArchiveAssetDto ToDto(this ArchiveAsset asset) {
      return new ArchiveAssetDto {
        Id = asset.Id,
        ArchiveId = asset.ArchiveId,
        MediaAssetId = asset.MediaAssetId,
        RelPath = asset.RelPath,
        FileMtimeUtc = asset.FileMtimeUtc,
        LastSeenUtc = asset.LastSeenUtc,
        LastVerifiedUtc = asset.LastVerifiedUtc,
        Status = asset.Status,
        AssetArchive = asset.AssetArchive.ToDto(),
        MediaAsset = asset.Asset?.ToDto() ?? null
      };
    }
  }

}
