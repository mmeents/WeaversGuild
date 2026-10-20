using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Handlers.Sessions;

namespace Weavers.Core.Service {

  public sealed record LineState(
    int HarnessAppSessionId,
    string MachineName,
    string Line,
    bool Running,
    long Seq,
    DateTime ChangedUtc = default
  );

  public sealed record SwarmMember(PeerInfo Peer, bool IsSelf, IReadOnlyList<LineState> Lines);

  public static class SwarmLines {
    public const string Main = "Main";
    public const string Comfy = "Comfy";
    public const string MainTest = "MainTest";
    public static readonly string[] All = [Main, Comfy, MainTest];
  }

  public sealed class SwarmHub(SwarmNode node) : Hub {
    PeerInfo Peer => (PeerInfo)Context.GetHttpContext()!.Items["swarm.peer"]!;

    public void PushLineState(LineState s) =>
      node.OnRemoteLineState(s with { HarnessAppSessionId = Peer.HarnessAppSessionId, MachineName = Peer.MachineName });

    public IReadOnlyCollection<LineState> Snapshot() => node.MyLines;

    public override Task OnDisconnectedAsync(Exception? ex) {
      return base.OnDisconnectedAsync(ex);
    }

  }

  public sealed class SwarmNode : IAsyncDisposable {
    private readonly ISwarmRegistry registry;
    private readonly AppSessionResponse mySession;
    private readonly string advertiseAddress;

    readonly SynchronizationContext? _ui = SynchronizationContext.Current; // construct on UI thread
    readonly ConcurrentDictionary<int, HubConnection> _peers = new();
    readonly ConcurrentDictionary<int, PeerInfo> _peerInfo = new();
    readonly object _gate = new();
    readonly Dictionary<string, LineState> _myLines = new();                          // this instance, all supported lines
    readonly Dictionary<(string MachineName, int SessionId, string Line), LineState> _view = new();       // self + every peer
    readonly CancellationTokenSource _cts = new();
    WebApplication? _app;
    PeerInfo? _me;    
    readonly string _token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    internal static byte[] HashToken(string t) => SHA256.HashData(Encoding.UTF8.GetBytes(t));

    public SwarmNode(ISwarmRegistry registry, AppSessionResponse mySession, string advertiseAddress, IEnumerable<string> lines) {
      this.registry = registry;
      this.mySession = mySession;
      this.advertiseAddress = advertiseAddress;
      foreach (var l in lines) {
        var s = new LineState(mySession.SessionId, Environment.MachineName, l, Running: false, Seq: 0);
        _myLines[l] = s;
        _view[(Environment.MachineName, mySession.SessionId, l)] = s;
      }
    }

    public int GatewaySessionId => mySession.SessionId;
    public int FootholdId => mySession.HarnessId;
    public int Port => _me?.Port ?? 0;

    public event Action<LineState>? LineStateChanged;   // fires for self and peers
    public event Action<PeerInfo>? PeerJoined;
    public event Action<int>? PeerLeft;
    public event Action? ViewChanged;                   // anything in the view changed → refresh UI
    public event Action<string>? Log;

    public IReadOnlyCollection<LineState> MyLines { get { lock (_gate) return _myLines.Values.ToArray(); } }
    public IReadOnlyList<LineState> View { get { lock (_gate) return _view.Values.ToArray(); } }
    public bool IsRunningAnywhere(string line) { lock (_gate) return _view.Values.Any(v => v.Line == line && v.Running); }
    public IReadOnlyList<SwarmMember> Members() {
      lock (_gate) {
        var bySession = _view.Values.GroupBy(v => v.HarnessAppSessionId)
          .ToDictionary(g => g.Key, g => (IReadOnlyList<LineState>)g.ToList());
        var list = new List<SwarmMember>();
        if (_me is not null) list.Add(new(_me, true, bySession.GetValueOrDefault(GatewaySessionId, [])));
        foreach (var p in _peerInfo.Values)
          list.Add(new(p, false, bySession.GetValueOrDefault(p.HarnessAppSessionId, [])));
        return list;
      }
    }

    public LineState? MachineHolder() {
      lock (_gate) return _view.Values.FirstOrDefault(v => v.Running && v.MachineName == Environment.MachineName);
    }
    public bool IsMachineFree() => MachineHolder() is null;

    public bool IsRunningHere(string line) { lock (_gate) return _myLines.TryGetValue(line, out var s) && s.Running; }

    public async Task StartAsync() {
      var b = WebApplication.CreateSlimBuilder();
      b.WebHost.ConfigureKestrel(o => o.ListenAnyIP(0));
      b.Services.AddSignalR();
      b.Services.AddSingleton(this);               // hub gets the node via DI
      _app = b.Build();
      _app.Use(async (ctx, next) => {
        if (ctx.Request.Path.StartsWithSegments("/swarm")) {
          var auth = ctx.Request.Headers.Authorization.ToString();
          if (!int.TryParse(ctx.Request.Headers["X-Swarm-Session"], out var sid)
              || !auth.StartsWith("Bearer ")
              || await registry.ValidateAsync(sid, HashToken(auth["Bearer ".Length..]), ctx.RequestAborted) is not { } peer) {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
          }
          ctx.Items["swarm.peer"] = peer;
        }
        await next();
      });
      _app.MapHub<SwarmHub>("/swarm");
      await _app.StartAsync(_cts.Token);           // bind failures throw HERE
      var port = new Uri(_app.Urls.First()).Port;
      _me = new(Environment.MachineName, GatewaySessionId, Environment.ProcessId, advertiseAddress, port);
      await registry.RegisterAsync(_me, HashToken(_token), _cts.Token);
      _ = LoopAsync(_cts.Token);                   // heartbeat + discover new peers
    }

    public async Task<bool> SetLineAsync(string line, bool running) {
      if (running && !await registry.TryAcquireLineAsync(Environment.MachineName, GatewaySessionId, line, Cx.HearbeatLookback, _cts.Token))
        return false;                                          // caller doesn't start the schedule
      if (!running)
        await registry.ReleaseLineAsync(Environment.MachineName, GatewaySessionId, line, _cts.Token);

      LineState s;
      lock (_gate) {
        if (!_myLines.TryGetValue(line, out var cur)) throw new ArgumentException($"Unknown line '{line}'");
        if (cur.Running == running) return false;             // currently running so block. 
        s = cur with { Running = running, Seq = cur.Seq + 1, ChangedUtc = DateTime.UtcNow };
        _myLines[line] = s;
      }
      Apply(s);                                    // self goes through the same path as peers
      await  Task.WhenAll(_peers.Values.Select(c =>
        Safe(() => c.InvokeAsync("PushLineState", s), "push line state")));
      return true;
    }

    public Task TouchLineAsync(string line) {
      LineState s;
      lock (_gate) {
        if (!_myLines.TryGetValue(line, out var cur)) throw new ArgumentException($"Unknown line '{line}'");
        s = cur with { Seq = cur.Seq + 1 };          // same state, newer seq
        _myLines[line] = s;
      }
      Apply(s);
      return Task.WhenAll(_peers.Values.Select(c =>
        Safe(() => c.InvokeAsync("PushLineState", s), "push line state")));
    }

    async Task LoopAsync(CancellationToken ct) {
      using var t = new PeriodicTimer(Cx.HearbeatFrequency);
      try {
        do {
          try {
            await registry.HeartbeatAsync(GatewaySessionId, ct);
            var live = await registry.GetLiveAsync(Cx.HearbeatLookback, ct);
            foreach (var p in live.Where(p => p.HarnessAppSessionId != GatewaySessionId && !_peers.ContainsKey(p.HarnessAppSessionId)))
              await Safe(() => ConnectAsync(p), $"connect to session {p.HarnessAppSessionId}");
          } catch (OperationCanceledException) when (ct.IsCancellationRequested) {
            throw;
          } catch (Exception ex) {
            RaiseLog($"Swarm loop: {ex.Message}");   // keep looping; next tick retries
          }
        } while (await t.WaitForNextTickAsync(ct));
      } catch (OperationCanceledException) { /* disposed */ }
    }

    async Task ConnectAsync(PeerInfo p) {
      var c = new HubConnectionBuilder().WithUrl($"http://{p.Address}:{p.Port}/swarm", o => {
        o.Headers["X-Swarm-Session"] = GatewaySessionId.ToString();
        o.Headers["Authorization"] = $"Bearer {_token}";
      }).Build();
      c.Closed += _ => {
        _peers.TryRemove(p.HarnessAppSessionId, out var _);
        _peerInfo.TryRemove(p.HarnessAppSessionId, out var _);
        PurgePeer(p.HarnessAppSessionId);
        return Task.CompletedTask;
      };
      _peers[p.HarnessAppSessionId] = c;
      _peerInfo[p.HarnessAppSessionId] = p;
      await c.StartAsync(_cts.Token);      
      Raise(() => PeerJoined?.Invoke(p));      
      foreach (var s in MyLines) await c.InvokeAsync("PushLineState", s, _cts.Token);
      foreach (var s in await c.InvokeAsync<LineState[]>("Snapshot", _cts.Token)) Apply(s);
    }

    internal void OnRemoteLineState(LineState s) => Apply(s);

    void Apply(LineState s) {
      lock (_gate) {
        var key = (s.MachineName, s.HarnessAppSessionId, s.Line);
        if (_view.TryGetValue(key, out var old) && s.Seq <= old.Seq) return;   // stale or duplicate
        _view[key] = s;
      }
      Raise(() => { LineStateChanged?.Invoke(s); ViewChanged?.Invoke(); });
    }

    internal void PurgePeer(int sessionId) {
      lock (_gate)
        foreach (var k in _view.Keys.Where(k => k.SessionId == sessionId).ToList()) _view.Remove(k);
      Raise(() => { PeerLeft?.Invoke(sessionId); ViewChanged?.Invoke(); });
    }

    void Raise(Action a) { if (_ui is null) a(); else _ui.Post(_ => a(), null); }
    void RaiseLog(string msg) => Raise(() => Log?.Invoke(msg));

    async Task Safe(Func<Task> f, string what) {
      try { await f(); } catch (Exception ex) { RaiseLog($"Swarm {what} failed: {ex.Message}"); }
    }

    public async ValueTask DisposeAsync() {
      _cts.Cancel();
      await Safe(() => registry.RemoveAsync(GatewaySessionId, CancellationToken.None), "remove from registry");
      foreach (var c in _peers.Values) await c.DisposeAsync();
      if (_app is not null) { await _app.StopAsync(); await _app.DisposeAsync(); }
    }
  }
}
