using MediatR;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Weavers.Core.Handlers.Documentation {

  public record HelpQuery(string? Command) : IRequest<string>;

  internal class HelpQueryHandler : IRequestHandler<HelpQuery, string> {
    public async Task<string> Handle(HelpQuery request, CancellationToken cancellationToken) {


      var sb = new StringBuilder();
      sb.AppendLine($"TheLoomMcp Operators Guide: now is {DateTime.Now}");
      sb.AppendLine("The loom is building models and letting infra do the coding.");
      sb.AppendLine("The looms nodes are Items but you only ever get served SummaryDtos.");
      sb.AppendLine("All Items have Properties and Relations.(InboundRelations to you the parent in the Summary.) Relations link to other Items. Properties are a list of ItemProperties. In SummaryDto you have Nodes as name of Relations.");
      sb.AppendLine("Some ItemTypes have children PropertyModels. example: The EntityClassModel TypeId:600 is an item, possibly has Relations to: EntityClassImportModel 605, EntityPropertyModel 610, EntityNavigationModel 614, EntityInboundNavigationModel 616 are also item types. They all are items that have properties.)");
      sb.AppendLine($"Folder tracking starts with Items having ItemType ProjectFolderModel 100. use {Cx.CmdListProjects} for list of root projects.");
      sb.AppendLine("The root is a folder, can have items typed as RelativeFolderModel 110 as Relations as child folders.  All items in the tree stack on each other.");
      sb.AppendLine("All tools starting with Add take the id of the Item we want to add to as first parameter, the Name of the new part we're adding the second. They populate default ItemProperties for each new item added.");
      sb.AppendLine($"Use {Cx.CmdUpdateItemName} to change an items name and effects will ripple through related items.");
      sb.AppendLine($"The only way to update an Item's Property is to find the property id and use it with the {Cx.CmdUpdateItemProperty} command.");
      sb.AppendLine($"Use {Cx.CmdSearch} or {Cx.CmdGetSummaryById} to search for Items by name or Id. nodesUp means include children, includeProps includes each items list of properties.");
      sb.AppendLine($"Namespaces start with items of Type LibraryModel 200. A folder inside a library is a item with type NamespaceModel 400. ");
      sb.AppendLine($"All Classes track on the parent's filePath and Namespace.  Namespace Models Add to parents for their own. C# Classes and Entities inherit namespace and folder from parent.");
      sb.AppendLine($"Use {Cx.CmdGetTypeDetails} <TypeId> to get details about a specific type. 0 to get the catagories. CSharpTypes {WeItemType.CSharpTypes.AsIntString()} the main one for property types.");
      sb.AppendLine($"If you pass a Typeid >= ProjectFolderModel 100, you will get a list of Items that match that type.");

      return sb.ToString();
    }
  }
}
