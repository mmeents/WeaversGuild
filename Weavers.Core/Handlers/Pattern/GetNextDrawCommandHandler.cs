using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace Weavers.Core.Handlers.Pattern {
  public record GetNextDrawCommand(int PatternId, int? TodoId) : IRequest<ItemDto?>;
  public class GetNextDrawCommandHandler : IRequestHandler<GetNextDrawCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;    
    public GetNextDrawCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;      
    }
    public async Task<ItemDto?> Handle(GetNextDrawCommand request, CancellationToken cancellationToken) {

      var patternItem = await _context.GetItemDtoById(request.PatternId);
      if (patternItem == null) { throw new Exception($"Parent item with id {request.PatternId} not found"); }
      if (patternItem.ItemTypeId != (int)WeItemType.PatternModel) {
        throw new Exception($"Invalid parent item type {(WeItemType)patternItem.ItemTypeId} valid types are {WeItemType.PatternModel}");
      }

      var dimensionRels = patternItem.Relations
        .Where(r => r.RelatedItemTypeId == (int)WeItemType.PatternDimensionModel && r.RelatedItemId.HasValue)
        .OrderBy(r => r.Rank)
        .ToList();
      var dimensionIds = dimensionRels.Select(r => r.RelatedItemId!.Value).ToList();      
      if (!dimensionIds.Any()) { throw new Exception($"No dimensions found for pattern with id {request.PatternId}"); }
      var dimensionItems = await _mediator.Send( new GetItemsByIdsQuery(dimensionIds));

      var nextRank = 1;
      nextRank = await _mediator.Send(new GetNextItemRankQuery(patternItem.Id)) + 1;
      var nextDrawName = $"Draw {nextRank}";

      var nextDraw = await _mediator.Send(
        new CreateRelatedItemCommand(patternItem.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.PatternDrawModel, nextDrawName, "", "{}"));

      if (nextDraw == null) { throw new Exception($"Failed to create next draw for pattern with id {request.PatternId}"); }

      var addedByProp = nextDraw.Properties.FirstOrDefault(p => p.Name == Cx.ItAddedBy);
      if (addedByProp != null && request.TodoId > 0) {
        int todoId = request.TodoId ?? 0;
        var attribution = await _mediator.Send(new ResolveAttributionQuery(todoId));
        addedByProp.Value = attribution.PresenceModelKey;
        await addedByProp.SaveProp(nextDraw, _mediator);

        var todoItemProp = nextDraw.Properties.FirstOrDefault(p => p.Name == Cx.ItTodoItem);
        if (todoItemProp != null && request.TodoId > 0) {
          todoItemProp.Value = request.TodoId.ToString();
          await todoItemProp.SaveProp(nextDraw, _mediator);
        }
      }

      var rankNum = 100;
      foreach (var dimId in dimensionRels) { 

        var dimItem = dimensionItems.FirstOrDefault(d => d.Id == dimId.RelatedItemId);
        if (dimItem == null) { throw new Exception($"Dimension item with id {dimId.RelatedItemId} not found"); }
        var dimName = dimItem.Name;

        var optionIds = dimItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.PatternOptionModel && r.RelatedItemId.HasValue)
          .OrderBy(r => r.Rank)
          .Select(r => r.RelatedItemId!.Value)
          .ToList();
        var optionItems = await _mediator.Send(new GetItemsByIdsQuery(optionIds));

        var nextOption = NextOptionPick(optionItems);
        string nextOptionValue = nextOption?.Id.ToString() ?? "0";
        string nextOptionName = nextOption?.Name ?? "None";

        var dimProp = new ItemProperty {          
          ItemId = nextDraw.Id,          
          Name = dimName,
          Value = nextOptionName,
          ValueHash = nextOptionName != null ? nextOptionName.ComputeHash() : null,
          ValueDataTypeId = (int)WeDataType.StrAscii,          
          EditorTypeId = (int)WeEditorType.String,
          IsRequired = true,
          IsVisible = true,
          IsReadOnly = true,
          Rank = rankNum
        };
        _context.ItemProperties.Add(dimProp);
        await _context.SaveChangesAsync();
        await AddToIssueCount(nextOption!);
        rankNum++;
      }

      return await _context.GetItemDtoById(nextDraw.Id);
    }

    public ItemDto? NextOptionPick(List<ItemDto> optionItems) {

      if (optionItems.Count == 0) return null;
      var scored = optionItems.Select(o => new {
        Item = o,
        Count = (o.Properties.FirstOrDefault(p => p.Name == Cx.ItIssuedCount)?.Value ?? "0").AsInt()
      }).ToList();
      var min = scored.Min(s => s.Count);
      var tier = scored.Where(s => s.Count == min).ToList();
      return tier[Random.Shared.Next(tier.Count)].Item;

    }

    public async Task AddToIssueCount(ItemDto optionItem) {
      var issuedCountProp = optionItem.Properties.FirstOrDefault(p => p.Name == Cx.ItIssuedCount);
      if (issuedCountProp != null) {
        int issuedCount = issuedCountProp.Value.AsInt();
        issuedCount++;
        issuedCountProp.Value = issuedCount.ToString();
        await issuedCountProp.SaveProp(optionItem, _mediator);
      }
    }

  }
}
