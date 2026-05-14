using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Entities;

namespace Weavers.Core.Extensions {
  public static class McpLogEntryExt {
    public static async Task<bool> WriteMcpLogEntry(this FabricDbContext context, McpLogEntry entry) {      
      context.McpLogEntries.Add(entry);
      await context.SaveChangesAsync();
      return true;      
    }
  }
}
