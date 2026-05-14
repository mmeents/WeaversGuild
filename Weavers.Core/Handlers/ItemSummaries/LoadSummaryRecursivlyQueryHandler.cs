using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.ItemSummaries {

  public record LoadSummaryRecursivlyQuery(int itemId) : IMcpRequest, IRequest<ItemSummaryDto?>;
  public class LoadSummaryRecursivlyQueryHandler : IRequestHandler<LoadSummaryRecursivlyQuery, ItemSummaryDto?> {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<LoadSummaryRecursivlyQueryHandler> _logger;

    public LoadSummaryRecursivlyQueryHandler(IServiceScopeFactory serviceScopeFactory, ILogger<LoadSummaryRecursivlyQueryHandler> logger) {
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
    }

    public async Task<ItemSummaryDto?> Handle(LoadSummaryRecursivlyQuery request, CancellationToken cancellationToken) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var itemSummary = await dbContext.GetSummaryDtoById(request.itemId, true, true, cancellationToken);
        if (itemSummary == null) { return null; }
        itemSummary = await dbContext.LoadSummaryRecursively(itemSummary);
        return itemSummary;
      } catch (Exception ex) {
        _logger.LogError(ex, "Error retrieving item by id");
        return null;
      }
    }
  }

}
