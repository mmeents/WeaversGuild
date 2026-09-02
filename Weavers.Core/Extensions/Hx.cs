using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Extensions {
  public static class Hx {
    public static int StableId(int itemTypeId, string key) {
      var bytes = System.Text.Encoding.UTF8.GetBytes($"{itemTypeId}|{key}");
      var hash = System.Security.Cryptography.SHA256.HashData(bytes);
      return (int)(BitConverter.ToUInt32(hash, 0) & 0x7FFFFFFF);
    }


  }
}
