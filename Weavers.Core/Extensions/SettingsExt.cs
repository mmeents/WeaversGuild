using Weavers.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Weavers.Core.Extensions {

  public static class SettingsExt {
    public static AppSetting? GetAppSetting(this FabricDbContext context, string key) {
      var setting = context.AppSettings.AsNoTracking().Where(s => s.Key == key).FirstOrDefault();
      return setting;
    }

    public static AppSetting? SetAppSetting(this FabricDbContext context, AppSetting? value) {
      if (value == null) {
        return null;
      }
      var setting = context.AppSettings.Where(s => s.Key == value.Key).FirstOrDefault();
      if (setting == null) {
        setting = new AppSetting { Key = value.Key, Value = value.Value, ValueInt = value.ValueInt };
        context.AppSettings.Add(setting);
      } else {
        setting.Value = value.Value;
        setting.ValueInt = value.ValueInt;
        context.AppSettings.Update(setting);
      }
      context.SaveChanges();
      return setting;

    }
  }
}
