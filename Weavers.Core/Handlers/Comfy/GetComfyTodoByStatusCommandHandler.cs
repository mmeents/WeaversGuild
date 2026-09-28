using MediatR;
using Weavers.Core.Enums;
using Weavers.Core.Handlers.Todo;
using Microsoft.EntityFrameworkCore;
using Weavers.Core.Service;
using Weavers.Core.Constants;


namespace Weavers.Core.Handlers.Comfy {
  public record GetComfyTodoByStatusCommand(int HarnessId, WeItemType TodoStatusFilter, bool ReadyFilter) : IRequest<IReadOnlyList< ReadyTodoRow>>;
  public class GetComfyTodoByStatusCommandHandler : IRequestHandler<GetComfyTodoByStatusCommand, IReadOnlyList< ReadyTodoRow>> {
    private readonly FabricDbContext _context;
    private readonly IAppSessionService _sessionService;
    public GetComfyTodoByStatusCommandHandler(FabricDbContext context, IAppSessionService sessionService) {
      _context = context;
      _sessionService = sessionService;
    }
    public async Task<IReadOnlyList< ReadyTodoRow>> Handle(GetComfyTodoByStatusCommand request, CancellationToken cancellationToken) {

      var harnessId = request.HarnessId;
      const int HarnessType = (int)WeItemType.HarnessAppModel;   // 1010
      const int HarnessGatewayType = (int)WeItemType.HarnessGatewaysModel; // 1013
      const int ComfyServiceModelType = (int)WeItemType.ComfyServiceModel; // 1152
      const int ComfyWorkflowFolderModelType = (int)WeItemType.ComfyWorkflowFolderModel; // 1151
      const int ComfyOpTodoModelType = (int)WeItemType.ComfyOpTodoModel; // 1157
      var isReadyNeeded = true;
      string statusFilter = "";
      if (request.TodoStatusFilter == WeItemType.TodoNotStarted) {
        statusFilter = $"AND (CAST(itPS.Value AS int) = {(int)WeItemType.TodoNotStarted})";
      } else if (request.TodoStatusFilter == WeItemType.TodoInProgress) {
        statusFilter = $"AND (CAST(itPS.Value AS int) = {(int)WeItemType.TodoNotStarted} or CAST(itPS.Value AS int) = {(int)WeItemType.TodoInProgress})";
      } else if (request.TodoStatusFilter == WeItemType.TodoCompleteForward) {
        isReadyNeeded = false;
        statusFilter = $"AND (CAST(itPS.Value AS int) in ({(int)WeItemType.TodoCompleteForward}, {(int)WeItemType.TodoFailedForward}, {(int)WeItemType.TodoAbortedPushBack}))";
      } else { 
        throw new ArgumentException($"Invalid TodoStatusFilter value: {request.TodoStatusFilter}");
      }


      string isReady = request.ReadyFilter ? "1" : "0";  // on todo.

      // only require enabled if the request is for ready todos and the harness is the current harness.
      bool isEnabledNeeded = request.ReadyFilter && (_sessionService.HarnessId == harnessId);  
      string isEnabled = isEnabledNeeded ? "1" : "0";  // require on template to be enabled.          
      
      var sql = @$"       
        {(isReadyNeeded ? $"Declare @IsReady bit; set @IsReady = {isReady};" : "")}        
        Declare @IsEnabled bit; set @IsEnabled = {isEnabled};

        select 
          itTodo.Id, 
          itTodo.ItemTypeId, 
          itTodo.Name,
          itTemplate.Name DeskName,
          itTemplate.ItemTypeId DeskItemTypeId,
          itTodo.Established,            
          0 TodoDepth
 
        FROM dbo.Items itTodo  
          {(isReadyNeeded ? $"JOIN dbo.ItemProperties itR  ON itR.ItemId  = itTodo.Id AND itR.Name = '{Cx.ItConfirmedReady}' AND CAST(itR.Value AS bit) = @IsReady" : "")}          

          JOIN dbo.ItemProperties itT ON itT.ItemId = itTodo.Id AND itT.Name = '{Cx.ItWfTemplate}'
          Join dbo.Items itTemplate on itTemplate.Id = Cast(itT.Value as int)
          JOIN dbo.ItemProperties itTE  ON itTE.ItemId  = itTemplate.Id AND itTE.Name = '{Cx.ItEnabled}' 
          JOIN dbo.ItemProperties itPS ON itPS.ItemId = itTodo.Id AND itPS.Name = '{Cx.ItStatus}'

          join dbo.Relations pr on pr.RelatedItemId = itTemplate.Id
          Join dbo.Items pit on pit.Id = pr.ItemId and pit.ItemTypeId = {ComfyWorkflowFolderModelType}  -- parent is a workflow folder is parent of template

          join dbo.Relations csmr on csmr.RelatedItemId = pit.Id
          Join dbo.Items csm on csm.Id = csmr.ItemId and csm.ItemTypeId = {ComfyServiceModelType}  -- parent is a Comfy Gateway ServiceModel

          join dbo.Relations hgmr on hgmr.RelatedItemId = csm.Id
          Join dbo.Items hgm on hgm.Id = hgmr.ItemId and hgm.ItemTypeId = {HarnessGatewayType}  -- app harness gateway hasComfyPresence prop.

          join dbo.Relations hamr on hamr.RelatedItemId = hgm.Id
          Join dbo.Items ham on ham.Id = hamr.ItemId and ham.ItemTypeId = {HarnessType}  -- app harness app model has machine.
          where 
            itTodo.ItemTypeId = {ComfyOpTodoModelType}
            {statusFilter}
            AND ham.Id = {harnessId}
            {(isEnabledNeeded ? "AND CAST(itTE.Value AS bit) = @IsEnabled" : "")}
            AND itTodo.IsActive = 1
          ORDER BY itTodo.Id ASC";

      var rows = await _context.Set<ReadyTodoRow>().FromSqlRaw(sql).AsNoTracking().ToListAsync(cancellationToken);
      return rows;
    }
  }
}
