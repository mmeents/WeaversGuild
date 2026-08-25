using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.ItemSummaries {
  public record GetSummariesByIdsQuery(List<int> Ids) : IRequest<List<ItemSummaryDto>>; 
  public class GetSummariesByIdsQueryHandler : IRequestHandler<GetSummariesByIdsQuery, List<ItemSummaryDto>> {
    private readonly FabricDbContext _context;
    public GetSummariesByIdsQueryHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<List<ItemSummaryDto>> Handle(GetSummariesByIdsQuery request, CancellationToken cancellationToken) {
      if (request == null) throw new ArgumentNullException(nameof(request));
      if (request.Ids == null || request.Ids.Count == 0) throw new ArgumentException("Ids list cannot be null or empty.", nameof(request.Ids));
      HashSet<int> ids = new HashSet<int>(request.Ids);
          

      var dbResults = await _context.Items
        .Where(i => ids.Contains(i.Id))
        .OrderBy(f => f.Name)        
        .Select(i => new ItemSummaryDto {
          Id = i.Id,
          ParentId = i.IncomingRelations.Select(r => r.ItemId).FirstOrDefault(),
          TypeId = i.ItemTypeId,
          TypeName = i.ItemType.Name,
          Name = i.Name,
          Data = i.Data,
          Content = i.Description,
          NodesUp = false,
          Props = i.Properties.Select(p => new PropSummaryDto {
            Id = p.Id,
            Name = p.Name,
            Value = (p.EditorTypeId != null && ((WeEditorType)p.EditorTypeId) == WeEditorType.Password) ? "********" : p.Value ?? "",
            DataType = p.ValueType == null ? null : ((WeDataType)p.ValueType.Id).ToString(),
            EditorType = p.Editor == null ? null : p.Editor.Name,
            ReferenceType = p.ReferenceItemType == null ? null : p.ReferenceItemType.Name
          }).ToList()
        })
        .AsSplitQuery()
        .ToListAsync(cancellationToken); // Execute database query here

      return dbResults;
    }

  }



}
