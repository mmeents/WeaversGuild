using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;
using Weavers.Core.Service;


namespace Weavers.Core.Handlers.Import {
  public record ImportOrgDocCommand(string OrgDocFullPath, string OrgDocRelPath, bool OverwriteExisting) : IRequest<ImportOrgResponse>;

  public record ImportOrgResponse(string Message, bool IsSuccess);

  public class ImportOrgDocCommandHandler : IRequestHandler<ImportOrgDocCommand, ImportOrgResponse> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    private readonly IAppSessionService _appSessionService;
    public ImportOrgDocCommandHandler(IMediator mediator, FabricDbContext context, IAppSessionService appSessionService) {
      _mediator = mediator;
      _context = context;
      _appSessionService = appSessionService;
    }

    public async Task<ImportOrgResponse> Handle(ImportOrgDocCommand request, CancellationToken cancellationToken) {
      
      if (request.OrgDocFullPath == null || !File.Exists(request.OrgDocFullPath)) {
        return new ImportOrgResponse("Organization document not found.", false);
      }
      string fileExt = Path.GetExtension(request.OrgDocFullPath).ToLower();
      bool isOrgDoc = fileExt == ".md";
      bool isDigitalOperator = fileExt == ".json" && request.OrgDocFullPath.Contains("DigitalOperators");
      bool isDesk = fileExt == ".json" && request.OrgDocFullPath.Contains("OrgChart");

      var orgId = _appSessionService.OrganizationId;
      var orgItem = await _context.GetItemDtoById(orgId, cancellationToken);
      if (orgItem == null) { return new ImportOrgResponse("Organization item not found.", false); }

      var orgRootFolder = orgItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder)?.Value ?? "NoOrgRootFolder";
      if (orgRootFolder == "NoOrgRootFolder") { return new ImportOrgResponse("Organization root folder not found.", false); }

      if (isOrgDoc) {  
        var splitChars = Path.DirectorySeparatorChar+" ";
        var pathParts = request.OrgDocRelPath.Replace('/', Path.DirectorySeparatorChar).Parse(splitChars);
        var pathPartsCount = pathParts.Length;
        var lastFolderItem = orgItem;  // start from org per path only one per handler.
        var newFolderPath = orgRootFolder; // start from root folder
        for (var i = 0; i < pathPartsCount; i++) {
          var pathPart = pathParts[i];
          var isLastPart = i == pathPartsCount - 1;
          if (isLastPart) {  // is the file name need to make the doc object
            string nameWithoutExt = Path.GetFileNameWithoutExtension(pathPart);
            string fileContent = await File.ReadAllTextAsync(request.OrgDocFullPath, cancellationToken).ConfigureAwait(false);

            var childItemId = lastFolderItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.OrgDocModel
              && r.RelatedItemName == nameWithoutExt)?.RelatedItemId ?? 0;
            if (childItemId != 0) {
              if (request.OverwriteExisting) {
                var existingDocItem = await _context.GetItemDtoById(childItemId, cancellationToken);
                if (existingDocItem != null) {
                  existingDocItem.Description = fileContent;
                  await _mediator.Send(existingDocItem.ToUpdateCmd()).ConfigureAwait(false);
                  return new ImportOrgResponse("Document updated successfully.", true);
                } else {
                  return new ImportOrgResponse("Existing document item not found for update.", false);
                }
              } else {
                return new ImportOrgResponse("Document already exists and overwrite is false.", false);
              }
            } else {
              var docItem = await _mediator.Send(
                new CreateRelatedItemCommand(lastFolderItem.Id, (int)WeRelationTypes.Contains,
                  (int)WeItemType.OrgDocModel, nameWithoutExt, fileContent, "{}")).ConfigureAwait(false);
              if (docItem != null) {
                var docPath = Path.Combine(newFolderPath, pathPart);
                await _mediator.SetProperty(docItem, Cx.ItFilePath, docPath).ConfigureAwait(false);
                return new ImportOrgResponse("Document imported successfully.", true);
              }
            }

          } else {

            var childItemId = lastFolderItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.OrgDocFolderModel
              && r.RelatedItemName == pathPart)?.RelatedItemId ?? 0;
            if (childItemId == 0) {
              ItemDto? DocsFolderItem = await _mediator.Send(
                new CreateRelatedItemCommand(lastFolderItem.Id, (int)WeRelationTypes.Contains,
                  (int)WeItemType.OrgDocFolderModel, pathPart, "", "{}")).ConfigureAwait(false);

              if (DocsFolderItem != null) {
                var folderPath = Path.Combine(newFolderPath, pathPart);
                await _mediator.SetProperty(DocsFolderItem, Cx.ItRelativeFolder, folderPath).ConfigureAwait(false);
                lastFolderItem = DocsFolderItem;
                newFolderPath = folderPath;
              }
            } else {   // it did exist.
              var existingFolder = await _context.GetItemDtoById(childItemId, cancellationToken);
              if (existingFolder != null) {
                lastFolderItem = existingFolder;
                newFolderPath = Path.Combine(newFolderPath, pathPart);
              }
            }
          }

        } // for loop end
      }

      if (isDigitalOperator) { 
        var poolItemId = orgItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.DigitalOperatorPoolModel)?.RelatedItemId ?? 0;
        if (poolItemId == 0) {
          return new ImportOrgResponse("Digital operator pool not found in organization.", false);
        }
        var poolItem = await _context.GetItemDtoById(poolItemId, cancellationToken);
        if (poolItem != null) {
          string fileContent = await File.ReadAllTextAsync(request.OrgDocFullPath, cancellationToken).ConfigureAwait(false);
          OperatorExportModels? model = fileContent.FromJsonToOperator();
          if (model != null && model.OperatorName != string.Empty) {

            var existingOperatorId = poolItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.DigitalOperatorModel
              && r.RelatedItemName == model.OperatorName)?.RelatedItemId ?? 0;
            if (existingOperatorId != 0) {
              if (request.OverwriteExisting) {
                var existingOperator = await _context.GetItemDtoById(existingOperatorId, cancellationToken);
                if (existingOperator != null) {                  
                  var sysPromptProp = existingOperator.Properties.FirstOrDefault(p => p.Name == Cx.ItSystemPrompt);
                  if (sysPromptProp != null) {
                    sysPromptProp.Value = model.SysPrompt;
                    await _mediator.UpdateItemProp(existingOperator, sysPromptProp).ConfigureAwait(false);
                  }
                  return new ImportOrgResponse("Digital operator updated successfully.", true);
                } else {
                  return new ImportOrgResponse("Existing digital operator item not found for update.", false);
                }
              } else {
                return new ImportOrgResponse("Digital operator already exists and overwrite is false.", false);
              }
            } else {
              var operatorItem = await _mediator.Send(
                new CreateRelatedItemCommand(poolItem.Id, (int)WeRelationTypes.Contains,
                  (int)WeItemType.DigitalOperatorModel, model.OperatorName, "", "{}")).ConfigureAwait(false);
              if (operatorItem != null) {
                var newFolderPath = Path.Combine(orgRootFolder, "DigitalOperators");
                var docPath = Path.Combine(newFolderPath, model.OperatorName+".json");
                await _mediator.SetProperty(operatorItem, Cx.ItFilePath, docPath).ConfigureAwait(false);

                var sysPromptProp = operatorItem.Properties.FirstOrDefault(p => p.Name == Cx.ItSystemPrompt);
                if (sysPromptProp != null) {
                  sysPromptProp.Value = model.SysPrompt;
                  await _mediator.UpdateItemProp(operatorItem, sysPromptProp).ConfigureAwait(false);
                }
                return new ImportOrgResponse("Digital operator imported successfully.", true);
              }
            }

          }

        }
      }

      if (isDesk) {
        var orgChartId = orgItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.OrgChartModel)?.RelatedItemId ?? 0;
        if (orgChartId == 0) {
          return new ImportOrgResponse("Org chart not found in organization.", false);
        }
        var orgChartItem = await _context.GetItemDtoById(orgChartId, cancellationToken);
        if (orgChartItem == null) {
          return new ImportOrgResponse("Org chart item not found.", false);
        }
        string fileContent = await File.ReadAllTextAsync(request.OrgDocFullPath, cancellationToken).ConfigureAwait(false);
        DeskExportModels? model = fileContent.FromJsonToDesk();
        if (model == null || model.DeskName == string.Empty) {
          return new ImportOrgResponse("Invalid desk model data.", false);
        }

        var existingDeskId = orgChartItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.DeskModel
          && r.RelatedItemName == model.DeskName)?.RelatedItemId ?? 0;

        ItemDto? deskItem = null;
        if (existingDeskId == 0) {          
          deskItem = await _mediator.Send(
            new CreateRelatedItemCommand(orgChartItem.Id, (int)WeRelationTypes.Contains,
              (int)WeItemType.DeskModel, model.DeskName, "", "{}")).ConfigureAwait(false);
        } else {
          deskItem = await _context.GetItemDtoById(existingDeskId, cancellationToken);
        }

        if (deskItem != null) {

          var newFolderPath = Path.Combine(orgRootFolder, Cx.OrgChartFolder);
          var docPath = Path.Combine(newFolderPath, model.DeskName + ".json");
          var itemPath = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath)?.Value ?? string.Empty;
          if (itemPath != null && itemPath != docPath) {
            await _mediator.SetProperty(deskItem, Cx.ItFilePath, docPath).ConfigureAwait(false);
          }

          var deskRoleEnumInt = model.DeskRoleEnumInt;
          if (deskRoleEnumInt >= (int)WeItemType.DeskRoles && deskRoleEnumInt < (int)WeItemType.RunStatus) {
            var deskRoleProp = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItDeskRole);
            if (deskRoleProp != null) {
              if (deskRoleProp.Value != model.DeskRoleEnumInt.ToString()) {
                deskRoleProp.Value = model.DeskRoleEnumInt.ToString();
                await _mediator.UpdateItemProp(deskItem, deskRoleProp).ConfigureAwait(false);
              }
            }
          }    

          var sysPromptTemplateProp = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItSystemPromptTemplate);
          if (sysPromptTemplateProp != null) {
            if (sysPromptTemplateProp.Value != model.SysPromptTemplate) {
              sysPromptTemplateProp.Value = model.SysPromptTemplate;
              await _mediator.UpdateItemProp(deskItem, sysPromptTemplateProp).ConfigureAwait(false);
            }
          }

          var pushDeskName = model.PushDeskName ?? string.Empty;
          var pushDeskProp = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItOnPushbackSendTo);
          if (pushDeskName != string.Empty && pushDeskProp != null && pushDeskProp.Value != pushDeskName) {
            var pushDesk = _context.Items.FirstOrDefault(i => ( i.ItemTypeId == (int)WeItemType.DeskModel || i.ItemTypeId == (int)WeItemType.DeskLogModel ) 
              && i.Name == pushDeskName);
            var pushDeskId = pushDesk != null ? pushDesk.Id : 0;
            if (pushDeskId != 0) {
              await _mediator.SetProperty(deskItem, Cx.ItOnPushbackSendTo, pushDeskId.ToString()).ConfigureAwait(false);
            }
          }

          var failDeskName = model.FailDeskName ?? string.Empty;
          var failDeskProp = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItOnFailSendTo);
          if (failDeskName != string.Empty && failDeskProp != null && failDeskProp.Value != failDeskName) {
            var failDesk = _context.Items.FirstOrDefault(i => ( i.ItemTypeId == (int)WeItemType.DeskModel || i.ItemTypeId == (int)WeItemType.DeskLogModel ) 
              && i.Name == failDeskName);
            var failDeskId = failDesk != null ? failDesk.Id : 0;
            if (failDeskId != 0) {
              await _mediator.SetProperty(deskItem, Cx.ItOnFailSendTo, failDeskId.ToString()).ConfigureAwait(false);
            }
          }

          var successDeskName = model.SuccessDeskName ?? string.Empty;
          var successDeskProp = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItOnSuccessSendTo);
          if (successDeskName != string.Empty && successDeskProp != null && successDeskProp.Value != successDeskName) {
            var successDesk = _context.Items.FirstOrDefault(i => ( i.ItemTypeId == (int)WeItemType.DeskModel || i.ItemTypeId == (int)WeItemType.DeskLogModel ) 
              && i.Name == successDeskName);
            var successDeskId = successDesk != null ? successDesk.Id : 0;
            if (successDeskId != 0) {
              await _mediator.SetProperty(deskItem, Cx.ItOnSuccessSendTo, successDeskId.ToString()).ConfigureAwait(false);
            }
          }

          var operatorName = model.Operator ?? string.Empty;
          var operatorProp = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItOperator);
          if (operatorName != string.Empty && operatorProp != null && operatorProp.Value != operatorName) {
            var operatorItem = _context.Items.FirstOrDefault(i => i.ItemTypeId == (int)WeItemType.DigitalOperatorModel && i.Name == operatorName);
            var operatorId = operatorItem != null ? operatorItem.Id : 0;
            if (operatorId != 0) {
              await _mediator.SetProperty(deskItem, Cx.ItOperator, operatorId.ToString()).ConfigureAwait(false);
            }
          }

          return new ImportOrgResponse("Desk imported completed.", true);
        }
          
      }
         
      return new ImportOrgResponse("Document creation returned null unexpectedly.", false);
    }

  }

}
