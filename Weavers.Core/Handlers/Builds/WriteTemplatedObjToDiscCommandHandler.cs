using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Extensions;
using Weavers.Core.Models;
using Weavers.Core.Enums;

namespace Weavers.Core.Handlers.Builds {

  public record WriteTemplatedObjToDiscCommand(int ItemId) : IRequest<WriteTemplateResponse>;
  class WriteTemplatedObjToDiscCommandHandler : IRequestHandler<WriteTemplatedObjToDiscCommand, WriteTemplateResponse> {
    private readonly FabricDbContext _context;
    public WriteTemplatedObjToDiscCommandHandler(FabricDbContext context) {
      _context = context;
    }

    /*
     working towards: 
    1. Create Build record (Status = Pending)
    2. Walk ALL file-generating descendants → insert BuildFiles rows (full manifest)
    3. Load previous Build's BuildFiles → diff against current manifest → orphans get WasDeleted = true, file gets deleted from disk
    4. Filter current manifest where dirty (WrittenAt null || Established > WrittenAt) 
         → write each →   ( <- believe we are here )
         stamp WrittenAt → set WasWritten = true
    5. Update Build status = Compiling
    6. Shell dotnet build → capture output
    7. Update Build CompilerOutput, Status = Success/Failed, CompletedAt
     */

    public async Task<WriteTemplateResponse> Handle(WriteTemplatedObjToDiscCommand request, CancellationToken cancellationToken) {
      var gentypes = WeItemTypeExtensions.GetGenerativeTypes();
      var item = await _context.GetItemDtoById(request.ItemId, cancellationToken);
      if (item == null) return new WriteTemplateResponse(false, $"Failed to lookup id {request.ItemId}");
      if (!gentypes.Contains((WeItemType)item.ItemTypeId)) return new WriteTemplateResponse(false, $"Item type {(WeItemType)item.ItemTypeId} is not generative.");

      var templateContent = item.Description;

      string propKey = item.ItemTypeId.GetFolderPropertyName();
      if (propKey == "") return new WriteTemplateResponse(false, $"Failed to find filename property by type {(WeItemType)item.ItemTypeId}.");

      var folderProp = item.Properties.FirstOrDefault(p => p.Name == propKey);
      if (folderProp == null) return new WriteTemplateResponse(false, $"Failed to find filename property in item");

      var fileNamePath = folderProp.Name;
      if (string.IsNullOrEmpty(fileNamePath)) return new WriteTemplateResponse(false, $"Filename is empty.");

      var filesFolder = Path.GetDirectoryName(fileNamePath);
      if (filesFolder != null && !Directory.Exists(filesFolder)) {
        try {
          Directory.CreateDirectory(filesFolder);
        } catch (Exception ex) {          
          return new WriteTemplateResponse(false, $"Failed Ex creating directory for {filesFolder} {ex.Message} ");
        }
      } else {
        return new WriteTemplateResponse(false, $"Failed Path.GetDirectoryName returnd null for {fileNamePath}");
      }

      try {
        if (File.Exists(fileNamePath)) {
          File.Delete(fileNamePath);
        }
        System.IO.File.WriteAllText(fileNamePath, templateContent);


        return new WriteTemplateResponse(true, "");
      } catch (Exception ex) {        
        return new WriteTemplateResponse(false, $"Failed Exception {ex.Message}"); 
      }

    }
  }

  public record WriteTemplateResponse(bool Success, string Message = "");




}
