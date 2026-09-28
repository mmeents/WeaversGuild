using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;
using Weavers.Core.Constants;
using System.Text.Json.Nodes;

namespace Weavers.Core.Handlers.Comfy {

  public record AddComfyTodoCommand(int ParentItemId, string Name, int workflowId) : IRequest<ItemDto?>;
    
  public class AddComfyTodoCommandHandler : IRequestHandler<AddComfyTodoCommand, ItemDto?> {
    private readonly IServiceScopeFactory _scopeFactory;

    public AddComfyTodoCommandHandler(IServiceScopeFactory scopeFactory) {
      _scopeFactory = scopeFactory;
    }
    public async Task<ItemDto?> Handle(AddComfyTodoCommand request, CancellationToken cancellationToken) {

      using var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var context = scope.ServiceProvider.GetRequiredService<FabricDbContext>();

      if (request.ParentItemId <= 0) {
        throw new ArgumentException("ParentItemId must be greater than 0", nameof(request.ParentItemId));
      }

      List<int> itemIds = new List<int> { request.ParentItemId, request.workflowId };
      var items = await mediator.Send(new GetItemsByIdsQuery(itemIds));

      var parentItem = items.FirstOrDefault(i => i.Id == request.ParentItemId);
      if (parentItem == null) {
        throw new ArgumentException($"Parent item with id {request.ParentItemId} not found", nameof(request.ParentItemId));
      }
      if (parentItem.ItemTypeId != (int)WeItemType.ComfyOperationsModel) {
        throw new ArgumentException($"Parent item with id {request.ParentItemId} is not a Comfy Operations folder", nameof(request.ParentItemId));
      }

      var workflowItem = items.FirstOrDefault(i => i.Id == request.workflowId);
      if (workflowItem == null) {
        throw new ArgumentException($"Workflow item with id {request.workflowId} not found", nameof(request.workflowId));
      }
      if (workflowItem.ItemTypeId != (int)WeItemType.ComfyWorkflowTemplate) {
        throw new ArgumentException($"Workflow item with id {request.workflowId} is not a Comfy Workflow Template", nameof(request.workflowId));
      }

      var workflowInstructions = workflowItem.Description;
      var fileContent = workflowItem.Data;   
      var workflowTimeoutSec = workflowItem.Properties.FirstOrDefault(p => p.Name == Cx.ItTimeoutSec)?.Value ?? "300";

      var newItem = await mediator.Send(new CreateRelatedItemCommand(request.ParentItemId,
        (int)WeRelationTypes.Contains, (int)WeItemType.ComfyOpTodoModel,
          request.Name, workflowInstructions, fileContent));

      if (newItem == null) {
        throw new Exception("Failed to create new workflow item");
      }

      var TimeoutProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItTimeoutSec);
      if (TimeoutProp != null) {
        TimeoutProp.Value = workflowTimeoutSec;
        await TimeoutProp.SaveProp(newItem, mediator);
      }

      var WfTemplateProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItWfTemplate);
      if (WfTemplateProp != null) {
        WfTemplateProp.Value = request.workflowId.ToString();
        await WfTemplateProp.SaveProp(newItem, mediator);
      }

      var workflow = JsonNode.Parse(fileContent)!.AsObject();
      var paramOverrides = workflowItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.ComfyWfParamModel).ToList();
      var paramOverrideIds = paramOverrides.Where(r => r.RelatedItemId.HasValue).Select(r => r.RelatedItemId!.Value).ToList() ?? new List<int>();
      var paramOverrideItems = new List<ItemDto>();
      if (paramOverrideIds.Count > 0) {
        paramOverrideItems = await mediator.Send(new GetItemsByIdsQuery(paramOverrideIds));
        foreach (var paramOverrideItem in paramOverrideItems) {

          var newParamOverrideItem = await mediator.Send(new CreateRelatedItemCommand(newItem.Id,
            (int)WeRelationTypes.Contains, (int)WeItemType.ComfyOpParamModel,
              paramOverrideItem.Name, "", paramOverrideItem.Data));

          if (newParamOverrideItem == null) {
            throw new Exception($"Failed to create new parameter override item for {paramOverrideItem.Name}");
          }

          var PropValueProp = newParamOverrideItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropValue);
          var overridePropValue = paramOverrideItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropValue)?.Value;
          if (PropValueProp != null) {
            PropValueProp.Value = overridePropValue;
            await PropValueProp.SaveProp(newParamOverrideItem, mediator);
          }

          var SectionKeyProp = newParamOverrideItem.Properties.FirstOrDefault(p => p.Name == Cx.ItSectionKey);
          var overrideSectionKey = paramOverrideItem.Properties.FirstOrDefault(p => p.Name == Cx.ItSectionKey)?.Value;
          if (SectionKeyProp != null) {
            SectionKeyProp.Value = overrideSectionKey;
            await SectionKeyProp.SaveProp(newParamOverrideItem, mediator);
          }

          var ObjectKeyProp = newParamOverrideItem.Properties.FirstOrDefault(p => p.Name == Cx.ItObjectKey);
          var overrideObjectKey = paramOverrideItem.Properties.FirstOrDefault(p => p.Name == Cx.ItObjectKey)?.Value;
          if (ObjectKeyProp != null) {
            ObjectKeyProp.Value = overrideObjectKey;
            await ObjectKeyProp.SaveProp(newParamOverrideItem, mediator);
          }

          var PropKeyProp = newParamOverrideItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropKey);
          var overridePropKey = paramOverrideItem.Properties.FirstOrDefault(p => p.Name == Cx.ItPropKey)?.Value;
          if (PropKeyProp != null) {
            PropKeyProp.Value = overridePropKey;
            await PropKeyProp.SaveProp(newParamOverrideItem, mediator);
          }

          var OverrideTypeProp = newParamOverrideItem.Properties.FirstOrDefault(p => p.Name == Cx.ItOverrideType);
          var overrideType = paramOverrideItem.Properties.FirstOrDefault(p => p.Name == Cx.ItOverrideType)?.Value;
          var overrideTypeInt = int.TryParse(overrideType, out var parsedOverrideType) ? parsedOverrideType : (int)WeItemType.CtOverrideString;
          if (OverrideTypeProp != null) {
            OverrideTypeProp.Value = overrideType;
            await OverrideTypeProp.SaveProp(newParamOverrideItem, mediator);
          }

          if (string.IsNullOrEmpty(overrideObjectKey) || string.IsNullOrEmpty(overrideSectionKey) || string.IsNullOrEmpty(overridePropKey)) continue;
          if (workflow[overrideSectionKey]?[overrideObjectKey] is not JsonObject targetSection) continue; // node missing → leave template default, per your "removed prop = default stands" rule

          // Apply the override based on the type
          if (overrideTypeInt == (int)WeItemType.CtOverrideSeed) {
            targetSection[overridePropKey] = Random.Shared.NextInt64(0, long.MaxValue);
          } else if (overrideTypeInt == (int)WeItemType.CtOverrideFilePath && File.Exists(overridePropValue)) {            
            targetSection[overridePropKey] = overridePropValue;
          } else if (overrideTypeInt == (int)WeItemType.CtOverrideInt) {
            if (!string.IsNullOrEmpty(overridePropValue)) {
              targetSection[overridePropKey] = int.Parse(overridePropValue);
            }            
          } else if (overrideTypeInt == (int)WeItemType.CtOverrideDecimal) {
            if (!string.IsNullOrEmpty(overridePropValue)) {
              targetSection[overridePropKey] = decimal.Parse(overridePropValue);
            }
          } else {
            if (!string.IsNullOrEmpty(overridePropValue)) {
              targetSection[overridePropKey] = overridePropValue;
            }
          }

        } // endof foreach
        newItem.Data = workflow.ToJsonString();
        newItem = await mediator.Send(newItem.ToUpdateCmd());
      }


      return newItem;
    }
  }
}
