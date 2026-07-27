using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Items {
  public record GetRepoFolderItemsQuery(int RepoItemId, int ReposParentId) : IRequest<List<DbGitEntryItem>?>;
    
  public class GetRepoFolderItemsQueryHandler : IRequestHandler<GetRepoFolderItemsQuery, List<DbGitEntryItem>?> {
    private readonly FabricDbContext _context;
    public GetRepoFolderItemsQueryHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<List<DbGitEntryItem>?> Handle(GetRepoFolderItemsQuery request, CancellationToken cancellationToken) {

      if (request == null || request.RepoItemId <= 0 || request.ReposParentId <= 0) { throw new Exception("Request was null or invalid."); }

      var sql = $@"
WITH Descendants AS (
  SELECT  
    i.Id,
    i.Name,
    CAST(NULL AS int) AS ParentId,
    i.ItemTypeId,        
    CAST('/' + CAST(i.Id AS varchar(10)) + '/' AS varchar(900)) AS PathKey    
  FROM dbo.Items i
  WHERE i.Id = {request.ReposParentId} 
    AND i.IsActive = 1

UNION ALL

  SELECT  
    i.Id,
    i.Name,             
    r.ItemId,
    i.ItemTypeId,                    
    CAST(d.PathKey + CAST(i.Id AS varchar(10)) + '/' AS varchar(900))

  FROM Descendants d
    join dbo.Relations r ON r.ItemId = d.Id AND r.RelationTypeId = {(int)WeRelationTypes.Contains}
    join dbo.Items i ON i.Id = r.RelatedItemId       

  WHERE i.IsActive = 1
    and i.ItemTypeId in ({(int)WeItemType.GitFolderModel}, {(int)WeItemType.GitFileModel})
    AND d.PathKey NOT LIKE '%/' + CAST(i.Id AS varchar(10)) + '/%'
)

SELECT i.Id, i.Name, i.ParentId, i.ItemTypeId,  
  case when i.ItemTypeId in ({(int)WeItemType.ProjectFolderModel}, {(int)WeItemType.RelativeFolderModel}, {(int)WeItemType.GitFolderModel}) then '1' else '0' end IsDirStr,
  ISNULL(ipGP.Value, '') AS GitPath,
  ISNULL(ipFP.Value, '') AS FilePath,
  ISNULL(ipES.Value, '') AS Sha,
  cast(ISNULL(ipSize.Value,'0') AS bigint) AS Size,
  ISNULL(ipIsBinary.Value,'0') AS IsBinaryStr

FROM Descendants i
  Left outer join dbo.ItemProperties ipGP on i.Id = ipGP.ItemId and ipGP.Name = '{Cx.ItGitPath}'
  Left outer join dbo.ItemProperties ipFP on i.Id = ipFP.ItemId and ipFP.Name in ( '{Cx.ItRelativeFolder}', '{Cx.ItFilePath}')
  Left outer join dbo.ItemProperties ipES on i.Id = ipES.ItemId and ipES.Name = '{Cx.ItEntrySha}'
  Left outer join dbo.ItemProperties ipSize on i.Id = ipSize.ItemId and ipSize.Name = '{Cx.ItFileSize}'
  Left outer join dbo.ItemProperties ipIsBinary on i.Id = ipIsBinary.ItemId and ipIsBinary.Name = '{Cx.ItIsBinary}'

WHERE i.ItemTypeId in ({(int)WeItemType.GitFolderModel}, {(int)WeItemType.GitFileModel})
OPTION (MAXRECURSION 256); ";

      var rows = await _context.Set<DbGitEntryItem>().FromSqlRaw(sql)
        .AsNoTracking().ToListAsync(cancellationToken);

      return rows;
    }
  }
}
