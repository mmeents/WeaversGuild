using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Service {

  public interface IAppSessionService {
    string NameOnSession { get; }
    int OrganizationId { get; }
    int HarnessId { get; }
    int SessionId { get; }
    void Initialize(string nameOnSession, int orgId, int harnessId, int sessionId);
  }

  public class AppSessionService : IAppSessionService {
    public string NameOnSession { get; private set; } = "";
    public int OrganizationId { get; private set; } = 0;
    public int HarnessId { get; private set; } = 0;
    public int SessionId { get; private set; } = 0;

    public void Initialize(string nameOnSession, int orgId, int harnessId, int sessionId) {
      NameOnSession = nameOnSession;
      OrganizationId = orgId;
      HarnessId = harnessId;
      SessionId = sessionId;
    }
  }

}
