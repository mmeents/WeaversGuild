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
      var orgId = _appSessionService.OrganizationId;
      var orgItem = await _context.GetItemDtoById(orgId, cancellationToken);
      if (orgItem == null) { return new ImportOrgResponse("Organization item not found.", false); }

      var orgRootFolder = orgItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder)?.Value ?? "NoOrgRootFolder";
      if (orgRootFolder == "NoOrgRootFolder") { return new ImportOrgResponse("Organization root folder not found.", false); }

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
      }

      return new ImportOrgResponse("Document creation returned null unexpectedly.", false);
    }

  }



}
