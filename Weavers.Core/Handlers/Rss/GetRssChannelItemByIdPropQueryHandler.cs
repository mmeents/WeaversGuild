using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace Weavers.Core.Handlers.Rss {

  public record GetRssChannelItemByIdPropQuery(int RssChannelItemId, string searchUrl) : IRequest<IdOnlyRow>;

  public class GetRssChannelItemByIdPropQueryHandler : IRequestHandler<GetRssChannelItemByIdPropQuery, IdOnlyRow> {
    private readonly FabricDbContext _context;
    public GetRssChannelItemByIdPropQueryHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<IdOnlyRow> Handle(GetRssChannelItemByIdPropQuery request, CancellationToken cancellationToken) {

      const int itemModelType = (int)WeItemType.RssItemModel;
      const int linkedHtmlModelType = (int)WeItemType.RssLinkedHtmlModel; 
      long? valueHash = request.searchUrl.ComputeHash();

      var id = await _context.Items.AsNoTracking()
        .Where(it => (it.ItemTypeId == linkedHtmlModelType || it.ItemTypeId == itemModelType)          
          && it.Properties.Any(p => p.Name == Cx.ItHasUrl && p.ValueHash == valueHash))
        .Select(it => (int?)it.Id)
        .FirstOrDefaultAsync(cancellationToken);

      return new IdOnlyRow { ItemId = id };

    }
  }

  public record IdOnlyRow {
    public int? ItemId { get; init; }
  }

}
