using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Weavers.Core;
using Weavers.Core.Entities;

namespace Weavers.Core.Service {
  public interface ISwarmRegistry {
    Task RegisterAsync(PeerInfo me, byte[] tokenHash, CancellationToken ct);       // validate + address/port + first beat
    Task HeartbeatAsync(int sessionId, CancellationToken ct);
    Task RemoveAsync(int sessionId, CancellationToken ct);
    Task<IReadOnlyList<PeerInfo>> GetLiveAsync(TimeSpan ttl, CancellationToken ct);
    Task<PeerInfo?> ValidateAsync(int sessionId, byte[] tokenHash, CancellationToken ct);
    Task<bool> TryAcquireLineAsync(string machine, int sid, string line, TimeSpan ttl, CancellationToken ct);
    Task ReleaseLineAsync(string machine, int sid, string line, CancellationToken ct);
  }

  public class SwarmRegistryService(IDbContextFactory<FabricDbContext> factory) : ISwarmRegistry {

    public async Task RegisterAsync(PeerInfo me, byte[] tokenHash, CancellationToken ct) {
      await using var db = await factory.CreateDbContextAsync(ct);
      var s = await db.AppPeers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.HarnessAppSessionId == me.HarnessAppSessionId, ct);
      if (s == null) {
        // SwarmNode
        AppPeer toAdd = new() {
          Machine = me.MachineName,
          HarnessAppSessionId = me.HarnessAppSessionId,
          ProcessNum = me.ProcessNum,
          Address = me.Address,
          Port = me.Port,
          TokenHash = tokenHash,
        };
        await db.AppPeers.AddAsync(toAdd, ct);
        await db.SaveChangesAsync(ct);
      } else { 
        await db.AppPeers.Where(x => x.HarnessAppSessionId == me.HarnessAppSessionId)
        .ExecuteUpdateAsync(u => u.SetProperty(x => x.Address, me.Address)
          .SetProperty(x => x.Port, me.Port)
          .SetProperty(x => x.ProcessNum, me.ProcessNum)
          .SetProperty(x => x.TokenHash, tokenHash)
          .SetProperty(x => x.HeartBeat, x => SqlFns.SysUtcDateTime()), ct); ;
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
        .ExecuteUpdateAsync(u => u.SetProperty(x => x.Port, 0).SetProperty(x => x.TokenHash, (byte[]?)null), ct);   // Port 0 = not listening, clear access token.
    }

    public async Task<IReadOnlyList<PeerInfo>> GetLiveAsync(TimeSpan ttl, CancellationToken ct) {
      await using var db = await factory.CreateDbContextAsync(ct);
      var secs = -(int)ttl.TotalSeconds;
      return await db.AppPeers.AsNoTracking()
        .Where(s => s.Port > 0 && s.HeartBeat > SqlFns.SysUtcDateTime().AddSeconds(secs))
        .Select(s => new PeerInfo(s.Machine, s.HarnessAppSessionId, s.ProcessNum, s.Address, s.Port))
        .ToListAsync(ct);
    }

    public async Task<PeerInfo?> ValidateAsync(int sessionId, byte[] tokenHash, CancellationToken ct) {
      await using var db = await factory.CreateDbContextAsync(ct);
      var row = await db.AppPeers.AsNoTracking()
        .Where(p => p.HarnessAppSessionId == sessionId && p.Port > 0)
        .Select(p => new { p.TokenHash, Peer = new PeerInfo(p.Machine, p.HarnessAppSessionId, p.ProcessNum, p.Address, p.Port) })
        .FirstOrDefaultAsync(ct);
      return row is not null 
        && row.TokenHash is not null
        && CryptographicOperations.FixedTimeEquals(row.TokenHash, tokenHash) ? row.Peer : null;
    }

    public async Task<bool> TryAcquireLineAsync(string machine, int sid, string line, TimeSpan ttl, CancellationToken ct) {
      await using var db = await factory.CreateDbContextAsync(ct);
      var secs = -(int)ttl.TotalSeconds;

      // Take over if: it's already ours for this line, or the holder's session is dead/not listening.
      var n = await db.LineLeases
        .Where(l => l.Machine == machine &&
          ((l.HarnessAppSessionId == sid && l.Line == line) ||
           !db.AppPeers.Any(p => p.HarnessAppSessionId == l.HarnessAppSessionId
                              && p.Port > 0
                              && p.HeartBeat > SqlFns.SysUtcDateTime().AddSeconds(secs))))
        .ExecuteUpdateAsync(u => u
          .SetProperty(l => l.Line, line)
          .SetProperty(l => l.HarnessAppSessionId, sid)
          .SetProperty(l => l.AcquiredAt, l => SqlFns.SysUtcDateTime()), ct);
      if (n == 1) return true;

      try {   // no row yet for this machine
        db.LineLeases.Add(new LineLease { Machine = machine, Line = line, HarnessAppSessionId = sid });
        await db.SaveChangesAsync(ct);
        return true;
      } catch (DbUpdateException) { return false; }   // PK hit: someone live holds it
    }

    private async Task WithDb(CancellationToken ct, Func<FabricDbContext, Task> action) {
      await using var db = await factory.CreateDbContextAsync(ct);
      await action(db);
    }

    public Task ReleaseLineAsync(string machine, int sid, string line, CancellationToken ct) =>
      WithDb(ct, db => db.LineLeases
        .Where(l => l.Machine == machine && l.HarnessAppSessionId == sid && l.Line == line)
        .ExecuteDeleteAsync(ct));
  }

  public sealed record PeerInfo(
    string MachineName,
    int HarnessAppSessionId,    
    int ProcessNum,
    string Address,
    int Port
  );
}
