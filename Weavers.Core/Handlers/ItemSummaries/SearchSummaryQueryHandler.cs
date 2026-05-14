using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.ItemSummaries {
  public record SearchSummaryQuery( string SearchTerm, int ItemTypeId, int MaxResults ) : IMcpRequest, IRequest<List<ItemSummaryDto>>;
  public class SearchSummaryQueryHandler : IRequestHandler<SearchSummaryQuery, List<ItemSummaryDto>> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;

    public SearchSummaryQueryHandler( FabricDbContext context, IMediator mediator ) {
      _context = context;
      _mediator = mediator;
    }

    public async Task<List<ItemSummaryDto>> Handle(SearchSummaryQuery request, CancellationToken cancellationToken) {
      var searchTerm = request.SearchTerm;
      var itemTypeId = request.ItemTypeId;
      var maxResults = request.MaxResults;

      IQueryable<Item> query = _context.Items.AsQueryable();
      if (itemTypeId > 0) { 
        query = query.Where(i => i.ItemTypeId == itemTypeId);
      }

      if (!string.IsNullOrWhiteSpace(searchTerm)) {
        var searchTerms = GetSearchTerms(searchTerm);        
        if (searchTerms.Count != 0) {
          foreach (var term in searchTerms) {
            var termCopy = term; // Avoid closure issues
            query = query.Where(f =>
                EF.Functions.Like(f.Name, $"%{termCopy}%"));
          }
        }
      }

      var totalCount = await query.CountAsync(cancellationToken);

      if (totalCount == 0) {  // No results, redo with OR logic.
        query = _context.Items.AsQueryable();
        if (itemTypeId > 0) {
          query = query.Where(i => i.ItemTypeId == itemTypeId);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm)) {
          var searchTerms = GetSearchTerms(searchTerm);

          if (searchTerms.Count != 0) {
            var parameter = Expression.Parameter(typeof(Item), "f");
            Expression? body = null;

            foreach (var term in searchTerms) {
              var likeExpr = Expression.Call(
                  typeof(DbFunctionsExtensions),
                  nameof(DbFunctionsExtensions.Like),
                  Type.EmptyTypes,
                  Expression.Constant(EF.Functions),
                  Expression.Property(parameter, nameof(Item.Name)),
                  Expression.Constant($"%{term}%")
              );

              body = body == null ? likeExpr : Expression.OrElse(body, likeExpr);
            }

            var lambda = Expression.Lambda<Func<Item, bool>>(body!, parameter);
            query = query.Where(lambda);
          }
        }
        totalCount = await query.CountAsync(cancellationToken);
      }

      var dbResults = await query
        .OrderBy(f => f.Name)        
        .Take(request.MaxResults)
        .Select(i => new ItemSummaryDto {
          Id = i.Id,
          ParentId = i.IncomingRelations.Select(r => r.ItemId).FirstOrDefault(),
          TypeId = i.ItemTypeId,
          TypeName = i.ItemType.Name,
          Name = i.Name,
          NodesUp = false,                    
        })
        .ToListAsync(cancellationToken); // Execute database query here
      
      return dbResults;
    }



    private List<string> GetSearchTerms(string filter) {
      var terms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      var parts = filter.Split(new[] { ' ', '_', '-', '.', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (var part in parts) {
        terms.Add(part.ToLower());
        var camelParts = part.SplitCamelCase();
        foreach (var cp in camelParts) {
          if (cp.Length > 1) { // Skip single characters            
            terms.Add(cp.ToLower());
          }
        }
      }
      return terms.ToList();
    }



  }
}
