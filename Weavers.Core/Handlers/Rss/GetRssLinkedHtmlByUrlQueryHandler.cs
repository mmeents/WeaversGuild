using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Enums;
using Weavers.Core.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Extensions;

namespace Weavers.Core.Handlers.Rss {

  public record GetRssLinkedHtmlByUrlQuery(string url) : IRequest<IdOnlyRow>;
  public class GetRssLinkedHtmlByUrlQueryHandler : IRequestHandler<GetRssLinkedHtmlByUrlQuery, IdOnlyRow> {
    private readonly FabricDbContext _context;
    public GetRssLinkedHtmlByUrlQueryHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<IdOnlyRow> Handle(GetRssLinkedHtmlByUrlQuery request, CancellationToken cancellationToken) {
      const int itemModelType = (int)WeItemType.RssItemModel;
      const int linkedHtmlModelType = (int)WeItemType.RssLinkedHtmlModel;
      long? valueHash = request.url.ComputeHash();

      var id = await _context.Items.AsNoTracking()
        .Where(it => (it.ItemTypeId == linkedHtmlModelType || it.ItemTypeId == itemModelType)
          && it.Properties.Any(p => p.Name == Cx.ItHasUrl && p.ValueHash == valueHash))
        .Select(it => (int?)it.Id)
        .FirstOrDefaultAsync(cancellationToken);

      return new IdOnlyRow { ItemId = id };
    }
  }
}
