using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.ItemSummaries {

  public record GetSummaryByIdQuery(int Id, bool NodesUp, bool IncludeProps = true) : IMcpRequest, IRequest<ItemSummaryDto?>;

  public class GetSummaryByIdQueryHandler(IServiceScopeFactory serviceScopeFactory, ILogger<GetSummaryByIdQueryHandler> logger) : IRequestHandler<GetSummaryByIdQuery, ItemSummaryDto?> {
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<GetSummaryByIdQueryHandler> _logger = logger;

    public async Task<ItemSummaryDto?> Handle(GetSummaryByIdQuery request, CancellationToken cancellationToken) {
      try {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FabricDbContext>();
        var itemSummary = await dbContext.GetSummaryDtoById(request.Id, request.NodesUp, request.IncludeProps, cancellationToken);        
        return itemSummary;
      } catch (Exception ex) {
        _logger.LogError(ex, "Error retrieving item by id");
        return null;
      }
    }
  }

}
