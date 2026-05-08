using MediatR;
using System.Collections.Concurrent;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Builds;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Templates {

  public record GenerateWriteItemByTypeCommand(
    int itemId, 
    int buildId,
    BuildContext buildContext
    ) : IRequest<BuildContext?>;

  public class GenerateWriteItemByTypeCommandHandler : IRequestHandler<GenerateWriteItemByTypeCommand, BuildContext?> {
    private readonly FabricDbContext _context;
    private HashSet<WeItemType> _generatableTypes = WeItemTypeExtensions.GetGenerativeTypes();
    private readonly IMediator _mediator;
    public GenerateWriteItemByTypeCommandHandler(FabricDbContext context, IMediator mediator) {
      _context = context;
      _mediator = mediator;
    }
    public async Task<BuildContext?> Handle(GenerateWriteItemByTypeCommand request, CancellationToken cancellationToken) {
      var buildContext = request.buildContext;
      var item = await _context.GetItemDtoById(request.itemId);
      if (item == null) return buildContext.Fail($"ItemId {request.itemId} not found");

      WeItemType itemType = (WeItemType)item.ItemTypeId;
      if (!_generatableTypes.Contains(itemType)) return buildContext.Fail($"Item {item.Name} type not generatable");

      var folderPropName = item.ItemTypeId.GetFolderPropertyName();
      var fileNamePath = "";
      if (folderPropName == "") { return buildContext.Fail("Item not writeable."); }      
      var folderProp = item.Properties.FirstOrDefault(p => p.Name == folderPropName);
      if (folderProp == null) { return buildContext.Fail("Item path property not found."); }
      if (folderProp.Value == "") { return buildContext.Fail("Item path property is empty."); }
      try {
        fileNamePath = folderProp.Value ?? "";
        if (fileNamePath == "") { return buildContext.Fail("Item path property is empty."); }
        var path = Path.GetDirectoryName(fileNamePath);
        if (!Directory.Exists(path)) {
          Directory.CreateDirectory(path);  // except if it fails.
        } 
      } catch {
        return buildContext.Fail("Failed to locate directory.");
      }      

      string? result = null;
      switch (itemType) {
        case WeItemType.SolutionModel:
          result = await _mediator.Send(new GetSolutionTemplateCommand(item.Id));         
          break;
        case WeItemType.LibraryModel:
          result = await _mediator.Send(new GetLibraryTemplateCommand(item.Id));       
          break;
        case WeItemType.DependencyInjectionModel:
          result = await _mediator.Send(new GetDependencyInjectionTemplateCommand(item.Id));       
          break;
        case WeItemType.DbContextModel:
          result = await _mediator.Send(new GetDbContextTemplateCommand(item.Id));       
          break;
        case WeItemType.ClassModel:
          result = await _mediator.Send(new GetClassTemplateCommand(item.Id));       
          break;
        case WeItemType.EntityClassModel:
          result = await _mediator.Send(new GetEntityClassTemplateCommand(item.Id));       
          break;      
        default:
          break;
      }

      if (result != null) {
        item.Description = result;        
        await _mediator.Send(new UpdateItemCommand(item.Id, item.ItemTypeId, item.Name,
          item.Description, item.Data, item.IsActive, item.WrittenAt));  // this sets established.

        try {
          if (File.Exists(fileNamePath)) {
            File.Delete(fileNamePath);
          }

          File.WriteAllText(fileNamePath, result);
        } catch (Exception ex) {
          return buildContext.Fail($"Item {item.Name} excepted {ex.Message}"); 
        }
        await _mediator.Send(new MarkItemWrittenCommand(item.Id));  // marks it written later than established.


        var buildFile = new BuildFile {
          BuildId = request.buildId,
          ItemId = item.Id,
          FilePath = fileNamePath,
          WasWritten = true,
          WasDeleted = false
        };
        _context.BuildFiles.Add(buildFile);
        await _context.SaveChangesAsync(cancellationToken);

        item = await _context.GetItemDtoById(request.itemId);
        if (item != null) {
          buildContext.LibItems[item.Id] = item;
        }
        return buildContext;

      }
      return buildContext;
    }
  }
}
