using MediatR;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace Weavers.Core.Handlers.Builds {

  // Note: Main use case is the WriteDocument button in the loom app.
  // Folders are only set up to run the Export which is geared up to write a local static website that could also be published on a github static pages.
  // An attempt at foreach folder, any Realms found are listed in a index.html file for the folder. Each Realm gets a R{Id}.json file go with it.
  // Each Realm.html has similar formate to display the Stories and Albums in the Relam as a list to pick.
  // Each Story and Album has a [A,S]{Id}.json file to go with it. The Story and Album html pages are similar to the Realm.html page but with the list
  // of the StoryRollupModel items in the Story or Album gets a list of tracks and the patter repeats there as well.(kind of tracks don't get a .json file but are part of the A.json file.)
  public record WriteDocumentCommand(int ItemId) : IRequest<WriteDocumentCmdResult>;

  public class WriteDocumentCmdResult {
    public bool Success { get; set; }
    public string Message { get; set; } = "";
  }

  public class WriteDocumentCommandHandler : IRequestHandler<WriteDocumentCommand, WriteDocumentCmdResult> {
    private readonly FabricDbContext _context;
    private readonly IMediator _mediator;
    public WriteDocumentCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }
    public async Task<WriteDocumentCmdResult> Handle(WriteDocumentCommand request, CancellationToken cancellationToken) {

      var item = await _context.GetItemDtoById(request.ItemId, cancellationToken);
      if (item == null) return new WriteDocumentCmdResult { Success = false, Message = $"Failed to lookup id {request.ItemId}" };
      if (item.ItemTypeId.IsFolderType()) {
        var folderPath = item.ResolveParentFolderPath("0");
        if (!folderPath.ValidatePath()) {
          return new WriteDocumentCmdResult { Success = false, Message = $"Failed to resolve folder path for item {item.Name} (id {item.Id})" };
        }
        var expResult = await _mediator.Send(new ExportItemCommand(item.Id), cancellationToken);        
        return new WriteDocumentCmdResult {
          Success = expResult.Success,
          Message = $"Folder {item.Name} (id {item.Id}) and child items written successfully."
        };
      }

      if (!item.ItemTypeId.IsContentType()) return new WriteDocumentCmdResult { 
        Success = false, 
        Message = $"Item type {(WeItemType)item.ItemTypeId} is not document type." 
      };

      // this was v1, UI should have this option turned off...
      if (item.ItemTypeId == (int)WeItemType.RealmModel) {
        try {       
          return new WriteDocumentCmdResult { Success = false, Message = $"decomissioned route, Realm runs from folder holding realm." };
        } catch (Exception ex) {
          return new WriteDocumentCmdResult { Success = false, Message = $"Failed writing realm documents: {ex.Message}" };
        }
      } else if (item.ItemTypeId == (int)WeItemType.StoryRollupModel) {
        try {       
          return new WriteDocumentCmdResult { Success = false, Message = $"decomissioned route, Story rollup runs from folder holding realm." };
        } catch (Exception ex) {
          return new WriteDocumentCmdResult { Success = false, Message = $"Failed writing story rollup document: {ex.Message}" };
        }
      }

      var templateContent = item.Description;
    
      var folderProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (folderProp == null) return new WriteDocumentCmdResult { Success = false, Message = $"Failed to find filename property in item" };

      var fileNamePath = folderProp.Value;
      if (string.IsNullOrEmpty(fileNamePath)) return new WriteDocumentCmdResult { Success = false, Message = $"Filename is empty." };

      var filesFolder = Path.GetDirectoryName(fileNamePath);
      if (filesFolder != null && !Directory.Exists(filesFolder)) {
        try {
          Directory.CreateDirectory(filesFolder);
        } catch (Exception ex) {
          return new WriteDocumentCmdResult { Success = false, Message = $"Failed Ex creating directory for {filesFolder} {ex.Message} " };
        }
      } 

      try {
        if (File.Exists(fileNamePath)) {
          File.Delete(fileNamePath);
        }

        File.WriteAllText(fileNamePath, templateContent);

        return new WriteDocumentCmdResult { Success = true, Message = "Document written successfully." };
      } catch (Exception ex) {
        return new WriteDocumentCmdResult { Success = false, Message = $"Failed Exception {ex.Message}" };
      }

    }     

  }
 

}
