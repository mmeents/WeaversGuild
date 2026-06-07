using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;

namespace Weavers.Core.Models {
  public class RoleModel {
    public string Desk { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public List<RoleCommand> RoleCommands { get; set; } = new List<RoleCommand>();
  }

  public class RoleCommand {
    public WeItemType CommandType { get; set; }
    public string Command { get; set; } = string.Empty;
    public RoleCommand(WeItemType command) {
      CommandType = command;
      Command = RolesExts.GetMcpCommandString(command) ?? string.Empty;
    }
  }

  public static class RolesExts {

    public static RoleModel AsRoleModel(this WeItemType deskRole, string deskName, string operatorName) {
      var role = new RoleModel {
        Desk = deskName,
        Operator = operatorName,
        Role = RolesExts.DeskRoleToString(deskRole)
      };
      switch (deskRole) {
        case WeItemType.RoleResearch:        
          role.WithCommand(WeItemType.CmdHelp)
            .WithCommand(WeItemType.CmdGetSummaryById)
            .WithCommand(WeItemType.CmdAddOrgFile);
          break;
        case WeItemType.RoleReviewResearch:
          role.WithCommand(WeItemType.CmdHelp)
            .WithCommand(WeItemType.CmdGetSummaryById);
            //.WithCommand(WeItemType.CmdAddReviewRating);
          break;
        case WeItemType.RoleDesign:
        case WeItemType.RoleReviewDesign:
          role.WithCommand(WeItemType.CmdUpdateItemName)
              .WithCommand(WeItemType.CmdUpdateItemContent)
              .WithCommand(WeItemType.CmdUpdateItemProperty);
          break;
        case WeItemType.RolePlan:
        case WeItemType.RoleReviewPlan:
          role.WithCommand(WeItemType.CmdAddProjectRoot)
              .WithCommand(WeItemType.CmdAddSubFolder);
          break;
      }

      return role;  
    }

    public static RoleModel WithCommand(this RoleModel roleModel, WeItemType command) {
      roleModel.RoleCommands.Add(new RoleCommand(command));
      return roleModel;
    }

    public static string DeskRoleToString(WeItemType deskRole) {
      return deskRole switch {        
        WeItemType.RoleResearch => "Research",
        WeItemType.RoleReviewResearch => "Review Research",
        WeItemType.RoleDesign => "Design",
        WeItemType.RoleReviewDesign => "Review Design",
        WeItemType.RolePlan => "Plan",
        WeItemType.RoleReviewPlan => "Review Plan",
        WeItemType.RoleBuildingOut => "Building Out",
        WeItemType.RoleReviewBuildOut => "Review Build Out",
        WeItemType.RoleTesting => "Testing",
        WeItemType.RoleReviewTests => "Review Tests",
        WeItemType.RoleDocument => "Document",
        WeItemType.RoleReviewDocument => "Review Document",
        WeItemType.RolePackaging => "Packaging",
        WeItemType.RoleReviewPackaging => "Review Packaging",
        _ => ""
      };
    }

    public static string? GetMcpCommandString(WeItemType loomCommand) {
      return loomCommand switch {
        WeItemType.CmdHelp => Cx.CmdHelp,
        WeItemType.CmdListProjects => Cx.CmdListProjects,
        WeItemType.CmdSearch => Cx.CmdSearch,
        WeItemType.CmdGetSummaryById => Cx.CmdGetSummaryById,
        WeItemType.CmdGetTypeDetails => Cx.CmdGetTypeDetails,
        WeItemType.CmdUpdateItemName => Cx.CmdUpdateItemName,
        WeItemType.CmdUpdateItemContent => Cx.CmdUpdateItemContent,
        WeItemType.CmdUpdateItemProperty => Cx.CmdUpdateItemProperty,
        WeItemType.CmdAddDigitalOperatior => Cx.CmdAddDigitalOperator,
        WeItemType.CmdAddOrgFolder => Cx.CmdAddOrgFolder,
        WeItemType.CmdAddOrgFile => Cx.CmdAddOrgFile,
        WeItemType.CmdAddProjectRoot => Cx.CmdAddProjectRoot, 
        WeItemType.CmdAddSubFolder => Cx.CmdAddSubFolder,
        WeItemType.CmdAddSolution => Cx.CmdAddSolution,
        WeItemType.CmdAddSolutionImport => Cx.CmdAddSolutionImport,
        WeItemType.CmdAddMdFile => Cx.CmdAddMdFile,
        WeItemType.CmdAddHtmlFile => Cx.CmdAddHtmlFile,
        WeItemType.CmdAddConfigFile => Cx.CmdAddConfigFile,
        WeItemType.CmdAddLibrary => Cx.CmdAddLibrary, 
        WeItemType.CmdAddNamespace => Cx.CmdAddNamespace,
        WeItemType.CmdAddClass => Cx.CmdAddClass,  
        WeItemType.CmdAddClassImport => Cx.CmdAddClassImport,
        WeItemType.CmdAddClassProperty => Cx.CmdAddClassProperty,
        WeItemType.CmdAddClassMethod => Cx.CmdAddClassMethod,
        WeItemType.CmdAddClassMethodParam => Cx.CmdAddClassMethodParam,
        WeItemType.CmdAddEntityClass => Cx.CmdAddEntityClass,  
        WeItemType.CmdAddEntityClassImport => Cx.CmdAddEntityClassImport,
        WeItemType.CmdAddEntityProperty => Cx.CmdAddEntityProperty,
        _ => null 
      };
    }
  }


}
