using MediatR;
using Scriban;
using Scriban.Runtime;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Templates {
  public record RenderFieldTemplate(ItemPropertyDto field) : IRequest<string>;
  public class RenderTemplateFieldCommandHandler : IRequestHandler<RenderFieldTemplate, string> {
    private readonly FabricDbContext _context;

    public RenderTemplateFieldCommandHandler(FabricDbContext context) {
      _context = context;
    }

    public async Task<string> Handle(RenderFieldTemplate request, CancellationToken cancellationToken) {

      var fieldsItemId = request.field.ItemId;
      var templateText = request.field.Value;
      var fieldsItem = await _context.GetItemDtoById(fieldsItemId);
      if (fieldsItem != null && !string.IsNullOrEmpty(templateText)) {        
        var fieldItemTypeId = fieldsItem.ItemTypeId;
        if (fieldItemTypeId == (int)WeItemType.DeskModel) {

          var deskName = fieldsItem.Name;
          var operatorItemId = fieldsItem.Properties.FirstOrDefault(p => p.Name == Cx.ItOperator)?.Value;
          string operatorName = "Unassigned";
          if (!String.IsNullOrEmpty(operatorItemId)) {
            var operatorItem = await _context.GetItemDtoById(int.Parse(operatorItemId));
            if (operatorItem != null) {
              operatorName = operatorItem.Name;
            }
          }
          var roleProp = fieldsItem.Properties.FirstOrDefault(p => p.Name == Cx.ItDeskRole);
          var roleIdStr = roleProp != null ? roleProp.Value : WeItemType.RoleNone.AsIntString();
          if (int.TryParse(roleIdStr, out int roleId) 
            && roleId > (int)WeItemType.RoleNone 
            && roleId < (int)WeItemType.TodoStatuses)
          {
            WeItemType roleItemType = (WeItemType)roleId;
            var model = roleItemType.AsRoleModel(deskName, operatorName);
            var scriptObject1 = new ScriptObject();
            scriptObject1["model"] = model;
            return DoRendering(templateText, scriptObject1);
          }
          return $"Invalid role ID: {roleIdStr} found on {deskName}";

        } else if (fieldItemTypeId == (int)WeItemType.TodoModel) { 
          
          var refItemIdStr = fieldsItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem)?.Value ?? "0";
          int refItemId = int.TryParse(refItemIdStr, out int tempRefId) ? tempRefId : 0;
          if (refItemId >= 1) { 
            var refItem = await _context.GetItemDtoById(refItemId);
            if (refItem != null) {
              TodoTemplateModel model = await _context.TodoToTemplateModel(fieldsItem, refItem, cancellationToken);
              var scriptObject1 = new ScriptObject();
              scriptObject1["model"] = model;
              return DoRendering(templateText, scriptObject1);              
            }
            return $"Referenced item with ID {refItemId} not found";
          }
          return $"Todo Reference itemId did not parse";

        }
        return $"Referenced unsupported field type.";
      }
      return "Invalid field or template";
    }

    private string DoRendering(string template, ScriptObject script) {
      var context = new TemplateContext();
      context.PushGlobal(script);
      var templateObj = Template.Parse(template);
      if (templateObj.HasErrors) {
        return $"Template parsing errors: {string.Join(", ", templateObj.Messages.Select(m => m.Message))}";
      }
      var result = templateObj.Render(context);
      return result;
    }


  }
}
