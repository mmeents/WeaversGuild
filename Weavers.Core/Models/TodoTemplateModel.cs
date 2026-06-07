using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Models;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace Weavers.Core.Models {
  public class TodoTemplateModel {    
    public ItemSummaryDto? Todo { get; set; }
    public ItemSummaryDto? Target { get; set; }

  }


  public static class TodoExts { 
    public static async Task<TodoTemplateModel> TodoToTemplateModel(this FabricDbContext context, ItemDto TodoItem, ItemDto refItem, CancellationToken cancellationToken) {      
      var todoItem = await context.GetSummaryDtoById(TodoItem.Id, false, true, cancellationToken);
      var refModel = await context.GetSummaryDtoById(refItem.Id, true, true, cancellationToken);      
      var result = new TodoTemplateModel {        
        Todo = todoItem,
        Target = refModel,
      };
      return result;
    }
    
  }


}


