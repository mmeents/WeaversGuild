using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Templates;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Builds {

  public record WriteOrganizationCommand(int OrganizationId) : IRequest<BuildContext>;

  public class WriteOrganizationCommandHandler : IRequestHandler<WriteOrganizationCommand, BuildContext> {
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    public WriteOrganizationCommandHandler(IMediator mediator, FabricDbContext context) {
      _mediator = mediator;
      _context = context;
    }

    public async Task<BuildContext> Handle(WriteOrganizationCommand request, CancellationToken cancellationToken) {
      var buildContext = new BuildContext {
        ForceWrite = true
      };

      var organization = await _context.GetItemDtoById(request.OrganizationId, cancellationToken);
      if (organization == null) {
        return buildContext.Fail("Organization not found");
      }
      if (organization.ItemTypeId != (int)WeItemType.OrganizationModel) {
        return buildContext.Fail("OrganizationId is not an organization");
      }

      var orgRootFolder = organization.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder)?.Value;
      if (orgRootFolder == null) {
        return buildContext.Fail($"Organization with id {request.OrganizationId} does not have a root folder property.");
      }

      try {
        if (!Directory.Exists(orgRootFolder)) {
          Directory.CreateDirectory(orgRootFolder);
          if (!Directory.Exists(orgRootFolder)) {
            return buildContext.Fail($"Failed to create directory for Organization file at path: {orgRootFolder}");
          }
        }
      } catch (Exception ex) {
        return buildContext.Fail($"Error accessing Organization file path: {ex.Message}");
      }
           
      await WriteFolder(organization, buildContext, cancellationToken);
          
            
      return buildContext;
    }

    private async Task<BuildContext> WriteFolder(ItemDto folderItem, BuildContext bldContext, CancellationToken cancellationToken) {
      try { 
        string folderName = folderItem.ItemTypeId.GetFolderPropertyName();
        string folderPath = folderItem.Properties.FirstOrDefault(p => p.Name == folderName)?.Value ?? "";
        bool isGoForWrite = folderPath != "";
        if (folderPath != "" && !Directory.Exists(folderPath)) {
          Directory.CreateDirectory(folderPath);
          if (!Directory.Exists(folderPath)) {
            bldContext.Errors.Add($"Failed to create directory for folder at path: {folderPath}");
            isGoForWrite = false;
          } 
        }
        if (isGoForWrite) {
          foreach (var relation in folderItem.Relations.Where(r => r.RelatedItemId.HasValue
            && r.RelatedItemTypeId == (int)WeItemType.OrgDocModel)) {
            var docItem = await _context.GetItemDtoById(relation.RelatedItemId!.Value, cancellationToken);
            if (docItem == null) {
              bldContext.Errors.Add($"Related document with id {relation.RelatedItemId.Value} not found.");
              continue;
            }
            await WriteDocument(docItem, bldContext, cancellationToken);
          }

          foreach (var relation in folderItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.OrgDocFolderModel)) {
            var childFolderItem = await _context.GetItemDtoById(relation.RelatedItemId!.Value, cancellationToken);
            if (childFolderItem == null) {
              bldContext.Errors.Add($"Related folder with id {relation.RelatedItemId.Value} not found.");
              continue;
            }
            await WriteFolder(childFolderItem, bldContext, cancellationToken);
          }
        }
      } catch (Exception ex) {
        bldContext.Errors.Add($"Error creating directory for folder '{folderItem.Name}': {ex.Message}");
      }
      return bldContext;
    }

    private async Task<BuildContext> WriteDocument(ItemDto docItem, BuildContext bldContext, CancellationToken cancellationToken) {      
      try {
        string fileContent = docItem.Description;
        string filePath = docItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath)?.Value ?? "";
        if (filePath != "" && File.Exists(filePath)) {
          File.Delete(filePath);
        }
        await File.WriteAllTextAsync(filePath, fileContent, cancellationToken);
        await _context.MarkItemUpdated(docItem.Id, cancellationToken);
        bldContext.FilesWritten++;
      } catch (Exception ex) {
        bldContext.Errors.Add($"Error writing document '{docItem.Name}' to file: {ex.Message}");
      }
      return bldContext;
    }
  }
}
