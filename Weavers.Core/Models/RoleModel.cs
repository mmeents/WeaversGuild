using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

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
      Command = command.GetMcpCommandString() ?? string.Empty;
    }
  }

  public static class RolesExts {

    public static RoleModel AsRoleModel(this ItemDto roleItem, string deskName, string operatorName) {

      var roleCommandsProp = roleItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRoleCommands);
      var roleCmdPropValue = roleCommandsProp?.Value ?? string.Empty;
      var roleCommands = roleCmdPropValue.Split(',', StringSplitOptions.RemoveEmptyEntries)
              .Select(s => int.TryParse(s, out int id) ? id : (int?)null)
              .Where(id => id.HasValue)
              .Select(id => id!.Value)
              .ToHashSet();

      var role = new RoleModel {
        Desk = deskName,
        Operator = operatorName,
        Role = roleItem.Name
      };

      foreach(var cmdId in roleCommands) {
        if (Enum.IsDefined(typeof(WeItemType), cmdId)) {
          var cmdType = (WeItemType)cmdId;
          role.RoleCommands.Add(new RoleCommand(cmdType));
        }
      }

      return role;
    }

    public static RoleModel WithCommand(this RoleModel roleModel, WeItemType command) {
      roleModel.RoleCommands.Add(new RoleCommand(command));
      return roleModel;
    }

    public static string DeskRoleToString(WeItemType deskRole) {
      return deskRole switch {                
        _ => ""
      };
    }

  }


}
