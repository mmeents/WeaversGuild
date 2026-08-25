using MediatR;
using Weavers.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Weavers.Core.Handlers.ItemSummaries {

  public record GetKidsByTypeRecQuery(int ItemId, int ItemTypeId) : IRequest<List<ItemSummaryDto>> {
  }
  public class GetKidsByTypeRecQueryHandler : IRequestHandler<GetKidsByTypeRecQuery, List<ItemSummaryDto>> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public GetKidsByTypeRecQueryHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<List<ItemSummaryDto>> Handle(GetKidsByTypeRecQuery request, CancellationToken cancellationToken) {
    
      if (request == null) throw new ArgumentNullException(nameof(request));
      if (request.ItemId <= 0) throw new ArgumentOutOfRangeException(nameof(request.ItemId));
      if (request.ItemTypeId <= 0) throw new ArgumentOutOfRangeException(nameof(request.ItemTypeId));

      var sql = @$"       
with Descendants as (
  SELECT      
    i.Id,    
    CAST(NULL AS int) AS ParentId,
    CAST(0 AS int) AS [Rank],
    i.Name,    
    i.ItemTypeId, 
    it.Name TypeName,
    i.Description Content,
    i.Data,
    CAST('/' + CAST(i.Id AS varchar(10)) + '/' AS varchar(900)) AS PathKey,
    CAST('/0/' AS varchar(900)) AS OrderKey    
  FROM dbo.Items i    
    join dbo.ItemTypes it on it.Id = i.ItemTypeId
  WHERE i.Id = {request.ItemId} 
    AND i.IsActive = 1

  UNION ALL

  SELECT      
    i.Id,    
    r.ItemId ParentId,
    r.Rank,
    i.Name,                 
    i.ItemTypeId,
    it.Name TypeName,
    i.Description,
    i.Data,
    CAST(d.PathKey + CAST(i.Id AS varchar(10)) + '/' AS varchar(900)),
    CAST(d.OrderKey + RIGHT('0000000000' +CAST(r.Rank AS varchar(10)), 10) + '/' AS varchar(900))

  FROM Descendants d
    join dbo.Relations r ON r.ItemId = d.Id    
    join dbo.Items i ON i.Id = r.RelatedItemId       
    join dbo.ItemTypes it on it.Id = i.ItemTypeId

  WHERE i.IsActive = 1
    AND d.PathKey NOT LIKE '%/' + CAST(i.Id AS varchar(10)) + '/%'
    and LEN(d.PathKey) < 890
)
select 
  d.Id, 
  ParentId, 
  [Rank], 
  Name, 
  ItemTypeId TypeID, 
  TypeName, 
  cast(0 as bit) NodesUp, 
  d.Content, 
  d.Data 
from Descendants d 
where d.itemTypeID = {request.ItemTypeId}
order by d.OrderKey
OPTION (MAXRECURSION 256);";

      var rows = await _context.Set<ItemSummaryDto>().FromSqlRaw(sql)
        .AsNoTracking().ToListAsync(cancellationToken);

      var selectedIds = rows.Select(r => r.Id).ToList();
      var theirProps = await _mediator.Send(new GetPropertiesByItemIdsQuery(selectedIds), cancellationToken);

      foreach (var summary in rows) {
        if (summary != null && theirProps.ContainsKey(summary.Id)) {          
          summary.Props = theirProps[summary.Id];
        }
      }

      return rows;

    }
  }
}
