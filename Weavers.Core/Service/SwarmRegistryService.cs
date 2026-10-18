using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Weavers.Core;
using Weavers.Core.Entities;

namespace Weavers.Core.Service {
  public interface ISwarmRegistry {
    Task RegisterAsync(PeerInfo me, CancellationToken ct);       // validate + address/port + first beat
    Task HeartbeatAsync(int sessionId, CancellationToken ct);
    Task RemoveAsync(int sessionId, CancellationToken ct);
    Task<IReadOnlyList<PeerInfo>> GetLiveAsync(TimeSpan ttl, CancellationToken ct);
  }

  public class SwarmRegistryService(IDbContextFactory<FabricDbContext> factory) : ISwarmRegistry {

    public async Task RegisterAsync(PeerInfo me, CancellationToken ct) {
      await using var db = await factory.CreateDbContextAsync(ct);
      var s = await db.AppPeers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.HarnessAppSessionId == me.HarnessAppSessionId, ct);
      if (s == null) {
        AppPeer toAdd = new() {
          Machine = me.MachineName,
          HarnessAppSessionId = me.HarnessAppSessionId,
          ProcessNum = me.ProcessNum,
          Address = me.Address,
          Port = me.Port          
        };
        await db.AppPeers.AddAsync(toAdd, ct);
        await db.SaveChangesAsync(ct);
      } else { 
        await db.AppPeers.Where(x => x.HarnessAppSessionId == me.HarnessAppSessionId)
        .ExecuteUpdateAsync(u => u.SetProperty(x => x.Address, me.Address)
          .SetProperty(x => x.Port, me.Port)
          .SetProperty(x => x.HeartBeat, x => SqlFns.SysUtcDateTime()), ct);
      }
    }

    public async Task HeartbeatAsync(int sessionId, CancellationToken ct) {
      await using var db = await factory.CreateDbContextAsync(ct);
      var n = await db.AppPeers.Where(x => x.HarnessAppSessionId == sessionId)
        .ExecuteUpdateAsync(u => u.SetProperty(x => x.HeartBeat, x => SqlFns.SysUtcDateTime()), ct);
      if (n == 0) throw new InvalidOperationException($"GatewaySession {sessionId} no longer exists.");
    }

    public async Task RemoveAsync(int sessionId, CancellationToken ct) {
      await using var db = await factory.CreateDbContextAsync(ct);
      await db.AppPeers.Where(x => x.HarnessAppSessionId == sessionId)
        .ExecuteUpdateAsync(u => u.SetProperty(x => x.Port, 0), ct);   // Port 0 = not listening
    }

    public async Task<IReadOnlyList<PeerInfo>> GetLiveAsync(TimeSpan ttl, CancellationToken ct) {
      await using var db = await factory.CreateDbContextAsync(ct);
      var secs = -(int)ttl.TotalSeconds;
      return await db.AppPeers.AsNoTracking()
        .Where(s => s.Port > 0 && s.HeartBeat > SqlFns.SysUtcDateTime().AddSeconds(secs))
        .Select(s => new PeerInfo(s.Machine, s.HarnessAppSessionId, s.ProcessNum, s.Address, s.Port))
        .ToListAsync(ct);
    }
  }

  public sealed record PeerInfo(
    string MachineName,
    int HarnessAppSessionId,    
    int ProcessNum,
    string Address,
    int Port
  );
}
