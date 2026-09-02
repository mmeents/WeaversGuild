using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Chess {
  public record AddChessGameCommand(int GameRoomId, string Name, int? WhiteDeskId, int? BlackDeskId) : IMcpRequest, IRequest<ItemDto?> {
  }
  public class AddChessGameCommandHandler : IRequestHandler<AddChessGameCommand, ItemDto?> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public AddChessGameCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }

    public async Task<ItemDto?> Handle(AddChessGameCommand request, CancellationToken cancellationToken) {

      List<int> ids = new List<int> { request.GameRoomId };
      if (request.WhiteDeskId.HasValue) { ids.Add(request.WhiteDeskId.Value); }
      if (request.BlackDeskId.HasValue) { ids.Add(request.BlackDeskId.Value); }
      var parmItems = await _mediator.Send(new GetItemsByIdsQuery(ids));

      var gameRoom = parmItems.FirstOrDefault(i => i.Id == request.GameRoomId);
      if (gameRoom == null) { throw new Exception($"Game room with id {request.GameRoomId} not found"); }
      if (gameRoom.ItemTypeId != (int)WeItemType.GameRoomModel) {
        throw new Exception($"Invalid game room item type {(WeItemType)gameRoom.ItemTypeId}");
      }

      ItemDto? whiteDesk = null;
      if (request.WhiteDeskId.HasValue && request.WhiteDeskId.Value >= 0) {
        whiteDesk = parmItems.FirstOrDefault(i => i.Id == request.WhiteDeskId);
        if (whiteDesk != null) {
          if (whiteDesk.ItemTypeId != (int)WeItemType.DeskModel) {
            throw new Exception($"Invalid white desk item type {(WeItemType)whiteDesk.ItemTypeId}");
          }
        }
      }

      ItemDto? blackDesk = null;
      if (request.BlackDeskId.HasValue && request.BlackDeskId.Value >= 0) {
        blackDesk = parmItems.FirstOrDefault(i => i.Id == request.BlackDeskId);
        if (blackDesk != null) {
          if (blackDesk.ItemTypeId != (int)WeItemType.DeskModel) {
            throw new Exception($"Invalid black desk item type {(WeItemType)blackDesk.ItemTypeId}");
          }
        }
      }     

      var newItem = await _mediator.Send(
        new CreateRelatedItemCommand(gameRoom.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.ChessGameModel, request.Name, "", "[]"));

      if (newItem == null) {
        throw new Exception($"Failed to create chess game under game room {request.GameRoomId}");
      }

      if (whiteDesk != null) {        
        if (whiteDesk.ItemTypeId != (int)WeItemType.DeskModel) {
          throw new Exception($"Invalid white desk item type {(WeItemType)whiteDesk.ItemTypeId}");
        }
        var whiteDeskProp = whiteDesk.Properties.FirstOrDefault(p => p.Name == Cx.ItWhiteDesk);
        if (whiteDeskProp != null) {
          whiteDeskProp.Value = whiteDesk.Id.ToString();
          await whiteDeskProp.SaveProp(newItem, _mediator);
        }
      }

      if (blackDesk != null) {
        if (blackDesk.ItemTypeId != (int)WeItemType.DeskModel) {
          throw new Exception($"Invalid black desk item type {(WeItemType)blackDesk.ItemTypeId}");
        }
        var blackDeskProp = blackDesk.Properties.FirstOrDefault(p => p.Name == Cx.ItBlackDesk);
        if (blackDeskProp != null) {
          blackDeskProp.Value = blackDesk.Id.ToString();
          await blackDeskProp.SaveProp(newItem, _mediator);
        }
      }
      return await _context.GetItemDtoById(newItem.Id, cancellationToken);
    }
  }
}
