using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Service;

namespace Weavers.Core.Extensions {
  public static class AppSessionExt {

    public static string GetHumanHarnessKey(this int harnessId) {
      return $"{harnessId}-HumanUserPresence";
    }
    public static string GetHumanOperatorKey(this int harnessId) {
      return $"{harnessId}-HumanUserOperator";
    }
    public static string GetHumanTodoKey(this int harnessId) {
      return $"{harnessId}-HumanUserTodo";
    }
  }
}
