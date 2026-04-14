using Weavers.Core.Entities;
using Weavers.Core.Extensions;
using Microsoft.EntityFrameworkCore;  

namespace Weavers.Core.Service {

  public interface IAppSettingService { 
    AppSetting? this[string key] { get; set; }
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task<AppSetting?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AppSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<List<AppSetting>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, string?>> GetAllAsDictionaryAsync(CancellationToken cancellationToken = default);
  }


  public class AppSettingService : IAppSettingService {
    private readonly FabricDbContext _context;
    public AppSettingService(FabricDbContext context) {
      _context = context;
    }
    private readonly Dictionary<string, AppSetting> _cache = new Dictionary<string, AppSetting>();
    public AppSetting? this[string key] {
      get {
        if (_cache.TryGetValue(key, out var cachedSetting)) {
          return cachedSetting;
        }
        var setting = _context.GetAppSetting(key);
        if (setting != null) {
          _cache[setting.Key] = setting;
        }
        return setting;
      }
      set {
        var existingSetting = _context.SetAppSetting(value);
        if (existingSetting != null && value != null && value.Key != null) {
          _cache[value.Key] = existingSetting;
        }
      }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default) {
      return await _context.AppSettings.AnyAsync(s => s.Key == key, cancellationToken);
    }

    public async Task<AppSetting?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
      return await _context.AppSettings
          .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<AppSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default) {
      return await _context.AppSettings
          .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
    }

    public async Task<List<AppSetting>> GetAllAsync(CancellationToken cancellationToken = default) {
      return await _context.AppSettings
          .OrderBy(s => s.Key)
          .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<string, string?>> GetAllAsDictionaryAsync(CancellationToken cancellationToken = default) {
      return await _context.AppSettings
          .ToDictionaryAsync(s => s.Key, s => s.Value, cancellationToken);
    }





  }
}
