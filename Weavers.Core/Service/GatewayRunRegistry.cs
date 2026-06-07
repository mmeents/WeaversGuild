

namespace Weavers.Core.Service {

  // ============================================================================
  // Per-harness lock registry — register as a SINGLETON in DI.
  // One SemaphoreSlim(1,1) per harness id => one in-flight session per gateway,
  // independent across your n machines. The UI can also query IsBusy(harnessId)
  // to disable a run button while that harness is working.
  // ============================================================================

  public interface IGatewayRunRegistry {
    SemaphoreSlim GetGate(int harnessId);
    bool IsBusy(int harnessId);
  }

  public class GatewayRunRegistry : IGatewayRunRegistry {
    private readonly System.Collections.Concurrent.ConcurrentDictionary<int, SemaphoreSlim> _gates = new();
    public SemaphoreSlim GetGate(int harnessId) =>
        _gates.GetOrAdd(harnessId, _ => new SemaphoreSlim(1, 1));
    public bool IsBusy(int harnessId) =>
        _gates.TryGetValue(harnessId, out var s) && s.CurrentCount == 0;
  }

}
