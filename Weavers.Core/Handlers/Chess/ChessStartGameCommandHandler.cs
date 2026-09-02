using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Chess {
  public record ChessStartGameCommand(int ChessGameId) : IMcpRequest, IRequest<string>;
  public class ChessStartGameCommandHandler : IRequestHandler<ChessStartGameCommand, string> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public ChessStartGameCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;

    }
    public async Task<string> Handle(ChessStartGameCommand request, CancellationToken cancellationToken) {

      // get the chess game item by id validate.
      var gameItem = await _context.GetItemDtoById(request.ChessGameId, cancellationToken);
      if (gameItem == null) {
        throw new Exception($"Chess game with id {request.ChessGameId} not found");
      }

      // check if the game is in NotStarted state.
      var gameResultStatus = gameItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGameStatus)?.Value.AsInt() ?? 0; 
      if (gameResultStatus != (int)WeItemType.GameNotStarted) {
        throw new Exception($"Chess game with id {request.ChessGameId} is not in NotStarted state. Current state: {gameResultStatus}");
      }

      // load the players desks and validate that they are assigned. 
      var whiteDeskId = gameItem.Properties.FirstOrDefault(p => p.Name == Cx.ItWhiteDesk)?.Value.AsInt() ?? 0;
      var blackDeskId = gameItem.Properties.FirstOrDefault(p => p.Name == Cx.ItBlackDesk)?.Value.AsInt() ?? 0;
      List<int> deskIds = new List<int>();
      if (whiteDeskId > 0) { deskIds.Add(whiteDeskId); }
      if (blackDeskId > 0) { deskIds.Add(blackDeskId); }
      ItemDto? whiteDesk = null;
      ItemDto? blackDesk = null;
      ResolveAttributionQueryResult? resWhite = null;
      ResolveAttributionQueryResult? resBlack = null;
      bool isDeskActive = false;
      if (deskIds.Count > 0) {  // current attribution state of the desks. 
        var deskItems = await _mediator.Send(new GetItemsByIdsQuery(deskIds), cancellationToken);

        whiteDesk = deskItems.FirstOrDefault(d => d.Id == whiteDeskId);
        if (whiteDesk != null) {
          resWhite = await _mediator.Send(new ResolveDeskAttributionQuery(whiteDesk.Id), cancellationToken);
        } else {
          throw new Exception($"Error: Chess game with id {request.ChessGameId} has no desk assigned for white player. ");
        }
        isDeskActive = whiteDesk.Properties.FirstOrDefault(p => p.Name == Cx.ItEnabled)?.Value.AsBoolean() ?? false;

        blackDesk = deskItems.FirstOrDefault(d => d.Id == blackDeskId);
        if (blackDesk != null) {
          resBlack = await _mediator.Send(new ResolveDeskAttributionQuery(blackDesk.Id), cancellationToken);
        } else {
          throw new Exception($"Error: Chess game with id {request.ChessGameId} has no desk assigned for black player. ");
        }

      } else {
        throw new Exception($"Error: Chess game with id {request.ChessGameId} has no desks assigned for players");
      }

      // update game status to in progress and reset play data if any...
      var gameResultProp = gameItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGameStatus);
      if (gameResultProp != null) {
        gameResultProp.Value = ((int)WeItemType.GameInProgress).ToString();
        await gameResultProp.SaveProp(gameItem, _mediator);
      }
      var updated = await _mediator.Send(
        new UpdateItemCommand(
          gameItem.Id,
          gameItem.ItemTypeId,
          gameItem.Name,
          gameItem.Description,
          "[]",  // reset game data json to empty
          gameItem.IsActive,
          DateTime.Now), cancellationToken);


      // setup todo on whites desk.
      var playTodoName = $"Play next 1 chess move for game Id: {request.ChessGameId} as white.";

      var newTodoItem = await _mediator.Send(
       new CreateRelatedItemCommand(whiteDesk.Id, (int)WeRelationTypes.Contains,
         (int)WeItemType.TodoModel, playTodoName, "", "{}"), cancellationToken);

      if (newTodoItem == null) {
        throw new Exception("Failed to create new todo item on white players desk.");
      }

      bool newTodoPromptSet = false;
      var newTodoPromptProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItUserPromptTemplate);
      if (newTodoPromptProp != null) {
        newTodoPromptProp.Value =
          "TodoId: {{model.todo.id}} {{model.todo.name}}" + Environment.NewLine + playTodoName;
        await newTodoPromptProp.SaveProp(newTodoItem, _mediator);
        newTodoPromptSet = true;
      }

      // finish filling out properties on new todo item.
      bool newTodoRefItemSet = false;
      var newTodoRefItemIdProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem);
      if (newTodoRefItemIdProp != null) {        
        newTodoRefItemIdProp.ReferenceItemTypeId = (int)WeItemType.ChessGameModel;
        newTodoRefItemIdProp.Value = request.ChessGameId.ToString();        
        await newTodoRefItemIdProp.SaveProp(newTodoItem, _mediator);
        newTodoRefItemSet = true;
      }
      
      var newTodoStatusProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItStatus);
      if (newTodoStatusProp != null) {
        newTodoStatusProp.Value = ((int)WeItemType.TodoNotStarted).ToString();
        await newTodoStatusProp.SaveProp(newTodoItem, _mediator);
      }

      if ( isDeskActive && newTodoPromptSet && newTodoRefItemSet) {
        var isReadyProp = newTodoItem.Properties.FirstOrDefault(p => p.Name == Cx.ItConfirmedReady);
        if (isReadyProp != null) {
          isReadyProp.Value = "1";
          await isReadyProp.SaveProp(newTodoItem, _mediator);
        }
      }     


      return $"Chess game with id {request.ChessGameId} started, todo sent to white player's desk.";
    } 
  }
}
