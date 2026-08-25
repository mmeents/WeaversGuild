using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Enums;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.ItemSummaries {

  public record GetPropertiesByItemIdsQuery(IReadOnlyCollection<int> ItemIds)
    : IRequest<Dictionary<int, List<PropSummaryDto>>>;

  public class GetPropertiesByItemIdsQueryHandler
    : IRequestHandler<GetPropertiesByItemIdsQuery, Dictionary<int, List<PropSummaryDto>>> {

    private readonly FabricDbContext _Context;
    public GetPropertiesByItemIdsQueryHandler(FabricDbContext context) {
      _Context = context;
    }

    public async Task<Dictionary<int, List<PropSummaryDto>>> Handle(
        GetPropertiesByItemIdsQuery request, CancellationToken cancellationToken) {

      if (request == null) throw new ArgumentNullException(nameof(request));
      if (request.ItemIds == null) throw new ArgumentNullException(nameof(request.ItemIds));

      var ids = request.ItemIds.Where(x => x > 0).Distinct().ToArray();
      if (ids.Length == 0) return new Dictionary<int, List<PropSummaryDto>>();

      var dbResults = await _Context.ItemProperties
        .AsNoTracking()
        .Where(ip => ids.Contains(ip.ItemId))
        .Select(p => new PropSummaryDto {          
          Id = p.Id,
          ItemId = p.ItemId,
          Name = p.Name,
          Value = (p.EditorTypeId != null && ((WeEditorType)p.EditorTypeId) == WeEditorType.Password) ? "********" : p.Value ?? "",
          DataType = p.ValueType == null ? null : ((WeDataType)p.ValueType.Id).ToString(),
          EditorType = p.Editor == null ? null : p.Editor.Name,
          ReferenceType = p.ReferenceItemType == null ? null : p.ReferenceItemType.Name
        })
        .ToListAsync(cancellationToken);      

      return dbResults  
        .GroupBy(r => r.ItemId)
        .ToDictionary(g => g.Key, g => g.ToList());
    }
  }
}