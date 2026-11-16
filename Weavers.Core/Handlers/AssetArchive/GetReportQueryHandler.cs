using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace Weavers.Core.Handlers.AssetArchive {

  public record GetReportQuery(int ArchiveId, ArchiveReportOrderBy OrderBy, int PageNo, int PageSize) : IRequest<ArchiveReport>;

  public class ArchiveReport {
    public int ArchiveId { get; set; }
    public string ArchiveName { get; set; } = "";
    public string RootPath { get; set; } = "";
    public int AssetCount { get; set; }    
    public IReadOnlyList<ArchiveAssetRef> Batch { get; set; } = new List<ArchiveAssetRef>();
  }

  public class GetReportQueryHandler : IRequestHandler<GetReportQuery, ArchiveReport> {
    private readonly FabricDbContext _context;
    public GetReportQueryHandler(FabricDbContext context) => _context = context;

    public async Task<ArchiveReport> Handle(GetReportQuery request, CancellationToken cancellationToken) {
      int archiveId = request.ArchiveId;
      var archive = await _context.GetItemDtoById(archiveId, cancellationToken)
        ?? throw new InvalidOperationException($"Archive item {archiveId} not found.");
      if (archive.ItemTypeId != (int)WeItemType.AssetArchiveModel)
        throw new InvalidOperationException($"Item {archiveId} is not an asset archive.");

      var rootPathProp = archive.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder);
      var rootPath = rootPathProp?.Value ?? "";

      int pageNo = request.PageNo < 1 ? 1 : request.PageNo;
      int take = Math.Clamp(request.PageSize, 1, 500);
      int skip = (pageNo - 1) * take;

      var (inner, outer) = request.OrderBy switch {
        ArchiveReportOrderBy.CapturedLocalAsc => ("ma.CapturedLocal ASC", "p.CapturedLocal ASC"),
        ArchiveReportOrderBy.CapturedLocalDesc => ("ma.CapturedLocal DESC", "p.CapturedLocal DESC"),
        ArchiveReportOrderBy.RelPathAsc => ("aa.RelPath ASC", "p.RelPath ASC"),
        ArchiveReportOrderBy.RelPathDesc => ("aa.RelPath DESC", "p.RelPath DESC"),
        ArchiveReportOrderBy.AssetIdAsc => ("aa.Id ASC", "p.Id ASC"),
        ArchiveReportOrderBy.AssetIdDesc => ("aa.Id DESC", "p.Id DESC"),
        _ => throw new ArgumentOutOfRangeException(nameof(request.OrderBy)),
      };

      var sql = $@"
WITH page AS (
  SELECT aa.Id, 
    aa.ArchiveId, 
    aa.MediaAssetId, 
    aa.RelPath, 
    aa.Status,
    ma.FileSize, 
    ma.CapturedLocal, 
    ma.CapturedSource, 
    ma.MediaType
  FROM dbo.ArchiveAssets aa
  JOIN dbo.MediaAssets ma ON ma.Id = aa.MediaAssetId
  WHERE aa.ArchiveId = @aid AND aa.Status = 0
  ORDER BY {inner}, aa.Id
  OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY)
SELECT p.*,
  c.CopiesHere, 
  c.CopiesTotal, 
  c.ArchivesHolding
FROM page p
CROSS APPLY (
  SELECT SUM(CASE WHEN x.ArchiveId = @aid THEN 1 ELSE 0 END) AS CopiesHere,
   COUNT(*) AS CopiesTotal,
   COUNT(DISTINCT x.ArchiveId) AS ArchivesHolding
  FROM dbo.ArchiveAssets x
  WHERE x.MediaAssetId = p.MediaAssetId AND x.Status = 0) c
ORDER BY {outer}, p.Id;";

      var rows = await _context.Database.SqlQueryRaw<ArchiveAssetRef>(sql,
        new SqlParameter("@aid", archiveId),
        new SqlParameter("@skip", skip),
        new SqlParameter("@take", take))
        .ToListAsync(cancellationToken);

      var assetCount = await _context.ArchiveAssets
        .CountAsync(a => a.ArchiveId == archiveId && a.Status == (byte)ArchiveAssetStatus.Present, cancellationToken);

      return new ArchiveReport {
        ArchiveName = archive.Name,
        RootPath = rootPath,
        AssetCount = assetCount,  
        Batch = rows
      };
    }
  }

  public enum ArchiveReportOrderBy {
    CapturedLocalAsc = 1,
    CapturedLocalDesc = 2,
    RelPathAsc = 3,
    RelPathDesc = 4,
    AssetIdAsc = 5,
    AssetIdDesc = 6
  }

  public class ArchiveAssetRef {
    public int Id { get; set; }
    public int ArchiveId { get; set; }
    public int MediaAssetId { get; set; }
    public string RelPath { get; set; } = "";
    public byte Status { get; set; }
    public long FileSize { get; set; }
    public DateTime? CapturedLocal { get; set; }
    public byte CapturedSource { get; set; }
    public string MediaType { get; set; } = "";
    public int CopiesHere { get; set; }
    public int CopiesTotal { get; set; }
    public int ArchivesHolding { get; set; }
  }

}
