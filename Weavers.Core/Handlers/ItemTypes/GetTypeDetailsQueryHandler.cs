using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.ItemTypes {
  public record GetTypeDetailsQuery(int ItemTypeId) : IMcpRequest, IRequest<GetTypeDetailsResponse>;
  public class GetTypeDetailsQueryHandler : IRequestHandler<GetTypeDetailsQuery, GetTypeDetailsResponse> {
    private readonly FabricDbContext context;
    public GetTypeDetailsQueryHandler(FabricDbContext context) {
      this.context = context;
    }

    public async Task<GetTypeDetailsResponse> Handle(GetTypeDetailsQuery request, CancellationToken cancellationToken) {

      var lookuptypes = WeItemTypeExtensions.GetLookupTypes();
      var typeId = request.ItemTypeId;
      if (typeId <= 0) {
        var res1 = new GetTypeDetailsResponse() {
          WeItemTypeId = 0,
          Name = $"Categories",
          Description = $"Type details response is broken into 2 categories. those with id's less than {(int)WeItemType.ProjectFolderModel}" +
            " the project folder model are references to the WeItemType enum. Choosing items greater returns the list of items of that type. " +
            " related types below are Primary categories"
        };
        var lookups = WeItemTypeExtensions.GetLookupTypes();
        foreach(var lookup in lookups) {
          res1.RelatedTypes.Add(new WeItemTypeDetails() {
            ItemTypeId = (int)lookup,
            Name = lookup.ToString(),
            Description = lookup.Description()
          });
        }
        return res1;
      } else{

        WeItemType weItemType;

        try { 
          weItemType = (WeItemType)typeId;
        } catch (Exception ex) {
            var res2 = new GetTypeDetailsResponse() {
              WeItemTypeId = typeId,
              Name = $"Unknown Type",
              Description = $"No details found for type id {typeId}. Exception: {ex.Message}"
            };
            return res2;
        }

        if (lookuptypes.Contains(weItemType)) {
          var res2 = new GetTypeDetailsResponse() {
            WeItemTypeId = (int)weItemType,
            Name = $"{weItemType.ToString()}",
            Description = $"{weItemType.Description()}"
          };
          var relatedTypes = Enum.GetValues<WeItemType>().Where(t => t.ParentType() == weItemType).ToList();
          foreach (var relatedType in relatedTypes) {
            res2.RelatedTypes.Add(new WeItemTypeDetails() {
              ItemTypeId = (int)relatedType,
              Name = relatedType.ToString(),
              Description = relatedType.Description()
            });
          }
          return res2;

        } else if (typeId < (int)WeItemType.ProjectFolderModel) {

          var res2 = new GetTypeDetailsResponse() {
            WeItemTypeId = (int)weItemType,
            Name = $"{weItemType.ToString()}",
            Description = $"{weItemType.Description()}"
          };
          return res2;

        } else if (weItemType >= WeItemType.ProjectFolderModel) { 
          var res2 = new GetTypeDetailsResponse() {
            WeItemTypeId = (int)weItemType,
            Name = $"{weItemType.ToString()}",
            Description = $"{weItemType.Description()}"
          };

          var items = await context.Items.Where(i => i.ItemTypeId == typeId)
            .Select(i => new ItemLookup(i.Id, i.Name, weItemType.ToString()))
            .ToListAsync(cancellationToken);
          foreach(var item in items) {
            res2.RelatedTypes.Add(new WeItemTypeDetails() {
              ItemTypeId = item.Value is int id ? id : 0,
              Name = item.DisplayText,
              Description = item.Description ?? ""
            });
          }
          return res2;
        }
      }

      var res = new GetTypeDetailsResponse() {
        WeItemTypeId = typeId,
        Name = $"Unknown Type",
        Description = $"No details found for type id {typeId} but wasn't knocked out earlier???"
      };
      return res;


    }
  }

  public class GetTypeDetailsResponse {
    public int WeItemTypeId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public List<WeItemTypeDetails> RelatedTypes { get; set; } = new List<WeItemTypeDetails>();
  }

  public class WeItemTypeDetails {
    public int ItemTypeId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
  }

}
