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
  public record WriteDocumentCommand(int ItemId) : IRequest<WriteDocumentCmdResult>;

  public class WriteDocumentCmdResult {
    public bool Success { get; set; }
    public string Message { get; set; } = "";
  }

  public class WriteDocumentCommandHandler : IRequestHandler<WriteDocumentCommand, WriteDocumentCmdResult> {
    private readonly FabricDbContext _context;
    public WriteDocumentCommandHandler(FabricDbContext context) {
      _context = context;
    }
    public async Task<WriteDocumentCmdResult> Handle(WriteDocumentCommand request, CancellationToken cancellationToken) {

      var item = await _context.GetItemDtoById(request.ItemId, cancellationToken);
      if (item == null) return new WriteDocumentCmdResult { Success = false, Message = $"Failed to lookup id {request.ItemId}" };
      if (!item.ItemTypeId.IsContentType()) return new WriteDocumentCmdResult { 
        Success = false, 
        Message = $"Item type {(WeItemType)item.ItemTypeId} is not document type." 
      };
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
