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
using Weavers.Core.Handlers.Pipeline;

namespace Weavers.Core.Handlers.Documentation {

  public record HelpQuery(string? Command) : IMcpRequest, IRequest<string>;

  public class HelpQueryHandler : IRequestHandler<HelpQuery, string> {
    public async Task<string> Handle(HelpQuery request, CancellationToken cancellationToken) {
      var sb = new StringBuilder();
      sb.AppendLine($"WeaversGuild presents, TheLoomMcp Operators Guide: now is {DateTime.Now}");
      sb.AppendLine("A virtual Software Development Factory, specializing in C# EF Core development.");
      sb.AppendLine("Core Strategy: build models and generate code, letting infrastructure do the work when it can.");
      sb.AppendLine("The loom's database nodes are Items but you only ever get served SummaryDtos.");
      sb.AppendLine("All Items have Properties, Relations, and InboundRelations. (Summaries simplify; InboundRelations is the parent in the Summary.)");
      sb.AppendLine("Properties are a list of ItemProperties rows. Default Properties and values are created by ItemType when the Item is created.");
      sb.AppendLine("Relations link Items; RelatedItem is the right-side child, Item is the left-side self.  In SummaryDto you have a Parent and Nodes as the name of Relations.");
      sb.AppendLine($"Folder tree starts with an item of type {WeItemType.OrganizationModel} {WeItemType.OrganizationModel.AsIntString()}. It has Org Folders and Project Folders.");
      sb.AppendLine($"Org Folders have main folders with types:");
      sb.AppendLine($"  {WeItemType.HarnessAppModel} {WeItemType.HarnessAppModel.AsIntString()} are Main App Type holds Gateway and Session tracking models.");
      sb.AppendLine($"  {WeItemType.OrgDeskRolesModel} {WeItemType.OrgDeskRolesModel.AsIntString()} is a folder for Desk Roles used to assign commands to roles for a Desk.");
      sb.AppendLine($"  {WeItemType.DigitalOperatorPoolModel} {WeItemType.DigitalOperatorPoolModel.AsIntString()} is a folder for Digital Operators used for desk assignment to presence models.");
      sb.AppendLine($"  {WeItemType.WorkGroupModel} {WeItemType.WorkGroupModel.AsIntString()} is a folder for the desks of the organization.");
      sb.AppendLine($"  {WeItemType.OrgDocFolderModel} {WeItemType.OrgDocFolderModel.AsIntString()} is a root folder for non-project-related organization documents.");
      sb.AppendLine($"Apps have 2 gateway models( {WeItemType.PresenceLmStudioGatewayModel} and {WeItemType.PresenceClaudeGatewayModel}). They are enabled via setting Has<setting> property, the {WeItemType.HarnessAppModel} typed item. Presence LLM models are children of the specific gateway.");
      sb.AppendLine($"Org can have any number of projects.  Project: use {Cx.CmdListProjects} for a list of root project folders.");
      sb.AppendLine("All tools starting with Add take the id of the Item we want to add to as the first parameter, and the Name of the new part we're adding as the second. They populate default ItemProperties for each new item added.");
      sb.AppendLine($"Use {Cx.CmdUpdateItemName} to change an item's name and the changes will ripple through related items.");
      sb.AppendLine($"The only way to update an Item's Property is to find the property id and use it with the {Cx.CmdUpdateItemProperty} command.");
      sb.AppendLine($"Use {Cx.CmdAppendItemContent} to append content to an existing item. Valid types are Md document types: OrgDocModel and FileMdModel. Recommend double pound header followed by md section.");
      sb.AppendLine($"Use {Cx.CmdSearch} or {Cx.CmdGetSummaryById} to search for Items by name or Id. nodesUp means include children; includeProps includes each item's list of properties.");
      sb.AppendLine($"Namespaces start with items of Type LibraryModel {WeItemType.LibraryModel.AsIntString()}. A folder inside a library is an item with type NamespaceModel {WeItemType.NamespaceModel.AsIntString()}.");
      sb.AppendLine($"All Classes track on the parent's filePath and Namespace in their properties.  Adding a NamespaceModel to a parent extends the parent's namespace to form its own. C# Classes and Entities inherit namespace and folder from the parent.");
      sb.AppendLine($"EntityProperties with FK should be named with Id at the end of the property name. After create, updates to the target entity are still needed in the new child nav related items, so verify its kids after.");
      sb.AppendLine($"Use {Cx.CmdGetTypeDetails} <TypeId> to get details about a specific type. 0 to get the categories. CSharpTypes {WeItemType.CSharpTypes.AsIntString()} is the main one for property types.");
      sb.AppendLine($"If you pass a TypeId >= ProjectFolderModel {WeItemType.ProjectFolderModel.AsIntString()}, you will get a list of Items that match that type.");
      sb.AppendLine($"Piece of cake, you got this, good luck.");
      return sb.ToString();
    }
  }
}
