using MediatR;
using Weavers.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Weavers.Core.Handlers.RelationTypes {
  public record GetAllRelationTypesQuery : IRequest<List<RelationTypeDto>>;
  public class GetAllRelationTypesQueryHandler : IRequestHandler<GetAllRelationTypesQuery, List<RelationTypeDto>> {
    private readonly FabricDbContext _context;
    public GetAllRelationTypesQueryHandler(FabricDbContext context) {
      _context = context;
    }

    public async Task<List<RelationTypeDto>> Handle(GetAllRelationTypesQuery request, CancellationToken cancellationToken) {
      var result = await _context.RelationTypes
        .AsNoTracking()
        .Select(rt => new RelationTypeDto {
          Id = rt.Id,
          Name = rt.Name,
          Description = rt.Description
        })
        .ToListAsync(cancellationToken);
      return result;
    }
  }
}