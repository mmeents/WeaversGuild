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
        case WeItemType.RoleOrgDocWriter:
          role.WithCommand(WeItemType.CmdHelp)
             .WithCommand(WeItemType.CmdGetSummaryById)
             .WithCommand(WeItemType.CmdAddOrgFile)
             .WithCommand(WeItemType.CmdUpdateItemContent)
             .WithCommand(WeItemType.CmdCompleteTodo)
             .WithCommand(WeItemType.CmdRejectTodo);
          break;
        case WeItemType.RoleReviewOrgDocWriter:
          role.WithCommand(WeItemType.CmdHelp)
            .WithCommand(WeItemType.CmdGetSummaryById)
            .WithCommand(WeItemType.CmdReviewPass)
            .WithCommand(WeItemType.CmdReviewFail);
          break;
        case WeItemType.RoleOrgResearcher:
          role.WithCommand(WeItemType.CmdHelp)
            .WithCommand(WeItemType.CmdGetSummaryById)
            .WithCommand(WeItemType.CmdAddOrgFile)
            .WithCommand(WeItemType.CmdUpdateItemContent)
            .WithCommand(WeItemType.CmdCompleteTodo)
            .WithCommand(WeItemType.CmdRejectTodo);
          break;
        case WeItemType.RoleReviewOrgResearcher:
          role.WithCommand(WeItemType.CmdHelp)
            .WithCommand(WeItemType.CmdGetSummaryById)
            .WithCommand(WeItemType.CmdReviewPass)
            .WithCommand(WeItemType.CmdReviewFail);
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
        WeItemType.RoleOrgDocWriter => "Org Doc Writer",
        WeItemType.RoleReviewOrgDocWriter => "Review Org Doc Writer",
        WeItemType.RoleOrgResearcher => "Org Researcher",
        WeItemType.RoleReviewOrgResearcher => "Review Org Researcher",      
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

        WeItemType.CmdCompleteTodo => Cx.CmdCompleteTodo,
        WeItemType.CmdRejectTodo => Cx.CmdRejectTodo,
        WeItemType.CmdReviewPass => Cx.CmdReviewPass,
        WeItemType.CmdReviewFail => Cx.CmdReviewFail,

        WeItemType.CmdAddOrgDesk => Cx.CmdAddOrgDesk,
        WeItemType.CmdAddDeskTodo => Cx.CmdAddDeskTodo,
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
