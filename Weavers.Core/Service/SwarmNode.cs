using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Handlers.Sessions;

namespace Weavers.Core.Service {

  public sealed record LineState(
    int HarnessAppSessionId,
    int FootholdId,
    string Line,
    bool Running,
    long Seq
  );

  public static class SwarmLines {
    public const string Main = "Main";
    public const string Comfy = "Comfy";
    public const string MainTest = "MainTest";
    public static readonly string[] All = [Main, Comfy, MainTest];
  }

  public sealed class SwarmHub(SwarmNode node) : Hub {
    public void PushLineState(LineState s) => node.OnRemoteLineState(s);
    public IReadOnlyCollection<LineState> Snapshot() => node.MyLines;
  }

  public sealed class SwarmNode : IAsyncDisposable {
    private readonly ISwarmRegistry registry;
    private readonly AppSessionResponse mySession;
    private readonly string advertiseAddress;

    readonly SynchronizationContext? _ui = SynchronizationContext.Current; // construct on UI thread
    readonly ConcurrentDictionary<int, HubConnection> _peers = new();
    readonly object _gate = new();
    readonly Dictionary<string, LineState> _myLines = new();                          // this instance, all supported lines
    readonly Dictionary<(int SessionId, string Line), LineState> _view = new();       // self + every peer
    readonly CancellationTokenSource _cts = new();
    WebApplication? _app;
    PeerInfo? _me;

    public SwarmNode(ISwarmRegistry registry, AppSessionResponse mySession, string advertiseAddress, IEnumerable<string> lines) {
      this.registry = registry;
      this.mySession = mySession;
      this.advertiseAddress = advertiseAddress;
      foreach (var l in lines) {
        var s = new LineState(mySession.SessionId, mySession.HarnessId, l, Running: false, Seq: 0);
        _myLines[l] = s;
        _view[(mySession.SessionId, l)] = s;
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
    public bool IsRunningHere(string line) { lock (_gate) return _myLines.TryGetValue(line, out var s) && s.Running; }

    public async Task StartAsync() {
      var b = WebApplication.CreateSlimBuilder();
      b.WebHost.ConfigureKestrel(o => o.ListenAnyIP(0));
      b.Services.AddSignalR();
      b.Services.AddSingleton(this);               // hub gets the node via DI
      _app = b.Build();
      _app.MapHub<SwarmHub>("/swarm");
      await _app.StartAsync(_cts.Token);           // bind failures throw HERE
      var port = new Uri(_app.Urls.First()).Port;
      _me = new(Environment.MachineName, GatewaySessionId, Environment.ProcessId, advertiseAddress, port);
      await registry.RegisterAsync(_me, _cts.Token);
      _ = LoopAsync(_cts.Token);                   // heartbeat + discover new peers
    }

    public Task SetLineAsync(string line, bool running) {
      LineState s;
      lock (_gate) {
        if (!_myLines.TryGetValue(line, out var cur)) throw new ArgumentException($"Unknown line '{line}'");
        if (cur.Running == running) return Task.CompletedTask;
        s = cur with { Running = running, Seq = cur.Seq + 1 };
        _myLines[line] = s;
      }
      Apply(s);                                    // self goes through the same path as peers
      return Task.WhenAll(_peers.Values.Select(c =>
        Safe(() => c.InvokeAsync("PushLineState", s), "push line state")));
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
      var c = new HubConnectionBuilder().WithUrl($"http://{p.Address}:{p.Port}/swarm").Build();
      c.Closed += _ => {
        _peers.TryRemove(p.HarnessAppSessionId, out var _);
        PurgePeer(p.HarnessAppSessionId);
        return Task.CompletedTask;
      };
      await c.StartAsync(_cts.Token);
      _peers[p.HarnessAppSessionId] = c;
      Raise(() => PeerJoined?.Invoke(p));
      foreach (var s in await c.InvokeAsync<LineState[]>("Snapshot")) Apply(s);
    }

    internal void OnRemoteLineState(LineState s) => Apply(s);

    void Apply(LineState s) {
      lock (_gate) {
        var key = (s.HarnessAppSessionId, s.Line);
        if (_view.TryGetValue(key, out var old) && s.Seq <= old.Seq) return;   // stale or duplicate
        _view[key] = s;
      }
      Raise(() => { LineStateChanged?.Invoke(s); ViewChanged?.Invoke(); });
    }

    void PurgePeer(int sessionId) {
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
