using Azure.Core;
using MediatR;
using System.Text;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;


namespace Weavers.Core.Handlers.Builds {

  public record WriteOrganizationCommand() : IRequest<BuildContext>;

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

      var organization = await _context.GetOrganizationItemDto(cancellationToken);
      if (organization == null) {
        return buildContext.Fail("Organization not found");
      }
      var organizationId = organization.Id;
      if (organization.ItemTypeId != (int)WeItemType.OrganizationModel) {
        return buildContext.Fail("OrganizationId is not an organization");
      }

      BuildDto? lastBuild = null;
      if (organization.Builds.Any()) {  
        lastBuild = organization.Builds.OrderByDescending(b => b.StartedAt).FirstOrDefault();
        if (lastBuild == null) {
          return buildContext.Fail($"Item claimed there was build but returned null.");
        }
        if (lastBuild.BuildStatus == BuildStatus.InProgress) {
          return buildContext.Fail($" OrganizationId:{organizationId} has build in progress.");
        }
        var oldBuildIdList = organization.Builds
          .Where(bf => bf.Id != lastBuild.Id).Select(bf => bf.Id).ToList();

        bool buildsRemoved = false;
        foreach (var buildId in oldBuildIdList) {
          var aBuild = await _context.Builds.FindAsync(buildId, cancellationToken);
          if (aBuild != null) {
            _context.Builds.Remove(aBuild);
            buildsRemoved = true;
            buildContext.Warnings.Add($"Note: Older build {buildId} was removed.");
          }
        }
        if (buildsRemoved) {
          await _context.SaveChangesAsync(cancellationToken);
        }
      }  // cleanup old builds except the last one. 

      var started = DateTime.UtcNow;  // add build record for this set of files.
      var build = new Build {
        LibraryItemId = organizationId,
        StartedAt = started,
        Status = (int)BuildStatus.Pending
      };
      _context.Builds.Add(build);
      await _context.SaveChangesAsync();
      buildContext = new BuildContext {
        BuildId = build.Id,
        ForceWrite = true
      };
      buildContext.LibItems[organization.Id] = organization;

      var orgRootFolder = organization.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder)?.Value;
      if (orgRootFolder == null) {
        return buildContext.Fail($"Organization with id {organizationId} does not have a root folder property.");
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

      await WriteDigitalOperators(organization, buildContext, cancellationToken);
      await WriteOrgDeskRoles(organization, buildContext, cancellationToken);
      await WriteOrgChart(organization, buildContext, cancellationToken);
      await WriteFolder(organization, buildContext, cancellationToken);
      await WriteOrganization(organization, buildContext, cancellationToken);

      if (lastBuild != null && buildContext.Success) {

        // cleanup of previous build files that are no longer relevant.
        foreach (var buildFile in lastBuild.BuildFiles) {
          if (!buildContext.LibItems.Keys.Contains(buildFile.ItemId)) {

            var fileNamePath = buildFile.FilePath;
            if (File.Exists(fileNamePath)) {
              File.Delete(fileNamePath);
              buildContext.FilesRemoved++;
              await _context.MarkBuildFileRemoved(buildFile.Id, cancellationToken);
            }

          } else { // was there... check for differences in path, if different then remove the old named version.
            var bi = buildContext.LibItems[buildFile.ItemId];
            var biprop = bi.Properties.FirstOrDefault(p => p.Name == bi.ItemTypeId.GetFolderPropertyName());
            if (biprop != null) {
              if (buildFile.FilePath != (biprop.Value ?? "")) {
                var fileNamePath = buildFile.FilePath;
                if (File.Exists(fileNamePath)) {
                  File.Delete(fileNamePath);
                  buildContext.FilesRemoved++;
                  await _context.MarkBuildFileRemoved(buildFile.Id, cancellationToken);
                }
              }
            }
          }
        }
      }


      // save status and output to build record.
      var result = buildContext.Ok($"{organization.Name} export completed.");
      var sbOutput = new StringBuilder();
      sbOutput.AppendLine($"{DateTime.UtcNow} Utc, {DateTime.Now} Local, {organization.Name} Sync to file system results.");
      foreach (var x in buildContext.Errors) {
        if (x != null && x != "") {
          sbOutput.AppendLine($"Error: {x}");
        }
      }
      foreach (var x in buildContext.Warnings) {
        if (x != null && x != "") {
          sbOutput.AppendLine($"Warning: {x}");
        }
      }
      sbOutput.AppendLine($"Written: {result.FilesWritten}");
      sbOutput.AppendLine($"Skipped: {result.FilesSkipped}");
      sbOutput.AppendLine($"Removed: {result.FilesRemoved}");
      build.BuildOutput = sbOutput.ToString();
      build.Status = (int)(buildContext.Success ? BuildStatus.Success : BuildStatus.Failed);
      await _context.SaveChangesAsync();


      return result;
    }

    public async Task<BuildContext> WriteDigitalOperators(ItemDto organizationItem, BuildContext bldContext, CancellationToken cancellationToken) { 

      var poolItemId = organizationItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.DigitalOperatorPoolModel)?.RelatedItemId;
      if (poolItemId == null) {
        return bldContext.Fail($"No Operator Pool found for organization with id {organizationItem.Id}");
      }
      
      var poolItem = await _context.GetItemDtoById(poolItemId.Value, cancellationToken);
      if (poolItem == null) { 
        return bldContext.Fail($"Operator Pool with id {poolItemId.Value} not found.");
      }
        
      var poolFolderPath = poolItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder)?.Value ?? "";
      if (string.IsNullOrEmpty(poolFolderPath)) {
        return bldContext.Fail($"Operator Pool with id {poolItemId.Value} does not have a relative folder property.");
      }

      try {
        if (!Directory.Exists(poolFolderPath)) {
          Directory.CreateDirectory(poolFolderPath);
          if (!Directory.Exists(poolFolderPath)) {
            return bldContext.Fail($"Failed to create directory for Operator Pool file at path: {poolFolderPath}");
          }
        }
      } catch (Exception ex) {
        return bldContext.Fail($"Error accessing Operator Pool file path: {ex.Message}");
      }

      var relOperatorIds = poolItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.DigitalOperatorModel)
        .Select(r => r.RelatedItemId).Where(id => id.HasValue).Select(id => id!.Value);
      foreach (var itemId in relOperatorIds) {
        var operatorItem = await _context.GetItemDtoById(itemId, cancellationToken);
        if (operatorItem != null) {
          bldContext.LibItems[operatorItem.Id] = operatorItem;
          try {
            string fileContent = operatorItem.ToExportOperatorModel().ToJson();
            string filePath = operatorItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath)?.Value ?? "";
            if (filePath != "" && File.Exists(filePath)) {
              File.Delete(filePath);
            }
            await File.WriteAllTextAsync(filePath, fileContent, cancellationToken);
            await _context.MarkItemUpdated(operatorItem.Id, cancellationToken);
            await _context.AddBuildItem(bldContext.BuildId, operatorItem.Id, filePath, cancellationToken);
            bldContext.FilesWritten++;
          } catch (Exception ex) {
            bldContext.Errors.Add($"Error writing document '{operatorItem.Name}' to file: {ex.Message}");
          }

        } else {
          bldContext.Errors.Add($"Related digital operator with id {itemId} not found.");
        }
      }            

      return bldContext;
    }



    public async Task<BuildContext> WriteOrgDeskRoles(ItemDto organizationItem, BuildContext bldContext, CancellationToken cancellationToken) {
      var OrgDeskRolesModelId = organizationItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.OrgDeskRolesModel)?.RelatedItemId;
      if (OrgDeskRolesModelId == null) {
        return bldContext.Fail($"No OrgDeskRolesModel found for organization with id {organizationItem.Id}");
      }

      var OrgDeskRolesItem = await _context.GetItemDtoById(OrgDeskRolesModelId.Value, cancellationToken);
      if (OrgDeskRolesItem == null) {
        return bldContext.Fail($"OrgDeskRolesModel with id {OrgDeskRolesModelId.Value} not found.");
      }

      var RoleFolderPath = OrgDeskRolesItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder)?.Value ?? "";
      if (string.IsNullOrEmpty(RoleFolderPath)) {
        return bldContext.Fail($"OrgDeskRolesModel with id {OrgDeskRolesModelId.Value} does not have a relative folder property.");
      }

      try {
        if (!Directory.Exists(RoleFolderPath)) {
          Directory.CreateDirectory(RoleFolderPath);
          if (!Directory.Exists(RoleFolderPath)) {
            return bldContext.Fail($"Failed to create directory for OrgDeskRolesModel file at path: {RoleFolderPath}");
          }
        }
      } catch (Exception ex) {
        return bldContext.Fail($"Error accessing OrgDeskRolesModel file path: {ex.Message}");
      }

      var RolesIds = OrgDeskRolesItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.DeskRoleModel)
        .Select(r => r.RelatedItemId).Where(id => id.HasValue).Select(id => id!.Value);
      foreach (var roleId in RolesIds) {
        var roleItem = await _context.GetItemDtoById(roleId, cancellationToken);
        if (roleItem != null) {
          bldContext.LibItems[roleItem.Id] = roleItem;
          try {
            string fileContent = roleItem.ToExportDeskRoleModel().ToJson();
            string filePath = roleItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath)?.Value ?? "";
            if (filePath != "" && File.Exists(filePath)) {
              File.Delete(filePath);
            }
            await File.WriteAllTextAsync(filePath, fileContent, cancellationToken);
            await _context.MarkItemUpdated(roleItem.Id, cancellationToken);
            await _context.AddBuildItem(bldContext.BuildId, roleItem.Id, filePath, cancellationToken);
            bldContext.FilesWritten++;
          } catch (Exception ex) {
            bldContext.Errors.Add($"Error writing desk role '{roleItem.Name}' to file: {ex.Message}");
          }

        } else {
          bldContext.Errors.Add($"Related Desk Role with id {roleId} not found.");
        }
      }

      return bldContext;
    }


    public async Task<BuildContext> WriteOrgChart(ItemDto organizationItem, BuildContext bldContext, CancellationToken cancellationToken) {
      var OrgChartItemId = organizationItem.Relations.FirstOrDefault(r => r.RelatedItemTypeId == (int)WeItemType.OrgChartModel)?.RelatedItemId;
      if (OrgChartItemId == null) {
        return bldContext.Fail($"No Operator Pool found for organization with id {organizationItem.Id}");
      }

      var OrgChartItem = await _context.GetItemDtoById(OrgChartItemId.Value, cancellationToken);
      if (OrgChartItem == null) {
        return bldContext.Fail($"Operator Pool with id {OrgChartItemId.Value} not found.");
      }

      var ChartFolderPath = OrgChartItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder)?.Value ?? "";
      if (string.IsNullOrEmpty(ChartFolderPath)) {
        return bldContext.Fail($"Operator Pool with id {OrgChartItemId.Value} does not have a relative folder property.");
      }

      try {
        if (!Directory.Exists(ChartFolderPath)) {
          Directory.CreateDirectory(ChartFolderPath);
          if (!Directory.Exists(ChartFolderPath)) {
            return bldContext.Fail($"Failed to create directory for Operator Pool file at path: {ChartFolderPath}");
          }
        }
      } catch (Exception ex) {
        return bldContext.Fail($"Error accessing Operator Pool file path: {ex.Message}");
      }

      var DeskIds = OrgChartItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.DeskModel)
        .Select(r => r.RelatedItemId).Where(id => id.HasValue).Select(id => id!.Value);
      foreach (var itemId in DeskIds) {
        var deskItem = await _context.GetItemDtoById(itemId, cancellationToken);
        if (deskItem != null) {
          bldContext.LibItems[deskItem.Id] = deskItem;
          try {
            string fileContent = _context.ToExportDeskModel(deskItem).ToJson();
            string filePath = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath)?.Value ?? "";
            if (filePath != "" && File.Exists(filePath)) {
              File.Delete(filePath);
            }
            await File.WriteAllTextAsync(filePath, fileContent, cancellationToken);
            await _context.MarkItemUpdated(deskItem.Id, cancellationToken);
            await _context.AddBuildItem(bldContext.BuildId, deskItem.Id, filePath, cancellationToken);
            bldContext.FilesWritten++;
          } catch (Exception ex) {
            bldContext.Errors.Add($"Error writing document '{deskItem.Name}' to file: {ex.Message}");
          }

        } else {
          bldContext.Errors.Add($"Related digital operator with id {itemId} not found.");
        }
      }

      return bldContext;
    }

    private async Task<BuildContext> WriteOrganization(ItemDto organizationItem, BuildContext bldContext, CancellationToken cancellationToken) {
      // For now, the organization file will represent the folder location listing of Org Docs and folders also written.
      try {
        var sbOrg = new StringBuilder();
        sbOrg.AppendLine($"# Organization: {organizationItem.Name}");
        sbOrg.AppendLine($"## Overview");
        sbOrg.AppendLine($"{organizationItem.Description}");
        sbOrg.AppendLine($"## Export Manifest");
        string orgRootFolder = organizationItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder)?.Value ?? "";
        var docIdList = bldContext.LibItems.Keys.ToList();
        foreach (var docId in docIdList) { 
          if (bldContext.LibItems.TryGetValue(docId, out var docItem)) {
            if (docItem.ItemTypeId == (int)WeItemType.OrgDocModel) {
              var fPath = docItem.Properties.FirstOrDefault(p => p.Name == docItem.ItemTypeId.GetFolderPropertyName())?.Value ?? "";
              if (fPath != "") {
                var relPath = Path.GetRelativePath(orgRootFolder, fPath);
                sbOrg.AppendLine($"- [{docItem.Name}]({relPath})");
              }
            }
          }
        }

        sbOrg.AppendLine("## Export Details");
        sbOrg.AppendLine("Schema: 1.0");
        
        string filePath = Path.Combine( orgRootFolder, Cx.AppOrgExport);
        sbOrg.AppendLine($"Location: {filePath}");
        sbOrg.AppendLine($"Exported at: {DateTime.UtcNow} Utc, {DateTime.Now} Local");

        string fileContent = sbOrg.ToString();  
        if (filePath != "" && File.Exists(filePath)) {
          File.Delete(filePath);
        }
        await File.WriteAllTextAsync(filePath, fileContent, cancellationToken);
        await _context.MarkItemUpdated(organizationItem.Id, cancellationToken);
        await _context.AddBuildItem(bldContext.BuildId, organizationItem.Id, filePath, cancellationToken);
        bldContext.FilesWritten++;
      } catch (Exception ex) {
        bldContext.Errors.Add($"Error writing document '{organizationItem.Name}' to file: {ex.Message}");
      }
      return bldContext;
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
            bldContext.LibItems[docItem.Id] = docItem;
            await WriteDocument(docItem, bldContext, cancellationToken);
          }

          foreach (var relation in folderItem.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.OrgDocFolderModel)) {
            var childFolderItem = await _context.GetItemDtoById(relation.RelatedItemId!.Value, cancellationToken);
            if (childFolderItem == null) {
              bldContext.Errors.Add($"Related folder with id {relation.RelatedItemId.Value} not found.");
              continue;
            }
            bldContext.LibItems[childFolderItem.Id] = childFolderItem;
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
        await _context.AddBuildItem(bldContext.BuildId, docItem.Id, filePath, cancellationToken);
        bldContext.FilesWritten++;
      } catch (Exception ex) {
        bldContext.Errors.Add($"Error writing document '{docItem.Name}' to file: {ex.Message}");
      }
      return bldContext;
    }
  }
}
