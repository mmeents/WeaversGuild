using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;

namespace Weavers.Core.Models {
  public class OperatorExportModels {
    public int Id { get; set; } = 0;
    public string OperatorName { get; set; } = string.Empty;
    public string SysPrompt { get; set; } = string.Empty;

  }

  public class DeskRoleExportModels {
    public int Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string RoleCommands { get; set; } = string.Empty;
    public string DeskPreAsserts { get; set; } = string.Empty;

  }


  public class DeskExportModels {
    
    public int MaxAttempts { get; set; } = 0; 
    public int DeskId { get; set; } = 0; 
    public string DeskName { get; set; } = string.Empty;
    public int OperatorId { get; set; } = 0;
    public string Operator { get; set; } = string.Empty;
    public int DeskRoleEnumInt { get; set; } = 0;
    public string SysPromptTemplate { get; set; } = string.Empty;
    public string PushDeskName { get; set; } = string.Empty;
    public string FailDeskName { get; set; } = string.Empty;
    public string SuccessDeskName { get; set; } = string.Empty;
  }


  public static class DeskRoleExportModelsExt {
    public static DeskRoleExportModels ToExportDeskRoleModel(this ItemDto deskRoleItem) {
      string roleCommands = deskRoleItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRoleCommands)?.Value ?? "";
      string deskPreAsserts = deskRoleItem.Properties.FirstOrDefault(p => p.Name == Cx.ItDeskPreAsserts)?.Value ?? "";
      return new DeskRoleExportModels {
        Id = deskRoleItem.Id,
        Name = deskRoleItem.Name,
        RoleCommands = roleCommands,
        DeskPreAsserts = deskPreAsserts
      };
    }
    public static string ToJson(this DeskRoleExportModels model) {
      return System.Text.Json.JsonSerializer.Serialize(model);
    }
    public static DeskRoleExportModels? FromJsonToDeskRole(this string json) {
      try {
        return System.Text.Json.JsonSerializer.Deserialize<DeskRoleExportModels>(json);
      } catch (Exception) {
        return null;
      }
    }
  }

  public static class DeskExportModelsExt {
    public static DeskExportModels ToExportDeskModel(this FabricDbContext _context, ItemDto deskItem) {
      string sysPromptTemplate = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItSystemPromptTemplate)?.Value ?? "";
      string pushDeskId = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItOnPushbackSendTo)?.Value ?? "";
      string failDeskId = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItOnFailSendTo)?.Value ?? "";
      string successDeskId = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItOnSuccessSendTo)?.Value ?? "";
      string operatorId = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItOperator)?.Value ?? "";

      var pushDeskName = "";
      if (pushDeskId != null && int.TryParse(pushDeskId, out int pushDeskInt)) {
        var pushDesk = _context.Items.FirstOrDefault(i => i.Id == pushDeskInt);
        if (pushDesk != null) {
          pushDeskName = pushDesk.Name;
        }
      }

      var failDeskName = "";
      if (failDeskId != null && int.TryParse(failDeskId, out int failDeskInt)) {
        var failDesk = _context.Items.FirstOrDefault(i => i.Id == failDeskInt);
        if (failDesk != null) {
          failDeskName = failDesk.Name;
        }
      }

      var successDeskName = "";
      if (successDeskId != null && int.TryParse(successDeskId, out int successDeskInt)) {
        var successDesk = _context.Items.FirstOrDefault(i => i.Id == successDeskInt);
        if (successDesk != null) {
          successDeskName = successDesk.Name;
        }
      }

      var operatorName = "";       
      int opId = 0;
      if (operatorId != null && int.TryParse(operatorId, out int operatorInt)) {
        opId = operatorInt;
        var operatorItem = _context.Items.FirstOrDefault(i => i.Id == operatorInt);
        if (operatorItem != null) {
          operatorName = operatorItem.Name;
        }
      }

      int maxAttempts = 0;
      var maxAttemptsProp = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItMaxAttempts);
      if (maxAttemptsProp != null) {
        int.TryParse(maxAttemptsProp.Value, out maxAttempts);
      }
      int deskRoleEnumInt = 0;
      var roleProp = deskItem.Properties.FirstOrDefault(p => p.Name == Cx.ItDeskRole);
      if (roleProp != null) {
        int.TryParse(roleProp.Value, out deskRoleEnumInt);
      }
      return new DeskExportModels {
        DeskId = deskItem.Id,
        DeskName = deskItem.Name,
        OperatorId = opId,
        Operator = operatorName,
        SysPromptTemplate = sysPromptTemplate,
        PushDeskName = pushDeskName,
        FailDeskName = failDeskName,
        SuccessDeskName = successDeskName,
        MaxAttempts = maxAttempts,
        DeskRoleEnumInt = deskRoleEnumInt
      };
    }
    public static string ToJson(this DeskExportModels model) {
      return System.Text.Json.JsonSerializer.Serialize(model);
    }
    public static DeskExportModels? FromJsonToDesk(this string json) {
      try {
        return System.Text.Json.JsonSerializer.Deserialize<DeskExportModels>(json);
      } catch (Exception) {
        return null;
      }
    }
  }



  public static class OperatorExportModelsExt {
    public static OperatorExportModels ToExportOperatorModel(this ItemDto operatorItem) {      
      string sysPrompt = operatorItem.Properties.FirstOrDefault(p => p.Name == Cx.ItSystemPrompt)?.Value ?? "";
      return new OperatorExportModels {
        Id = operatorItem.Id,
        OperatorName = operatorItem.Name,
        SysPrompt = sysPrompt
      };
    }

    public static string ToJson(this OperatorExportModels model) {
      return System.Text.Json.JsonSerializer.Serialize(model);
    }
    public static OperatorExportModels? FromJsonToOperator(this string json) {
      try {
        return System.Text.Json.JsonSerializer.Deserialize<OperatorExportModels>(json);
      } catch (Exception) {
        return null;
      }
    }
  }


}
