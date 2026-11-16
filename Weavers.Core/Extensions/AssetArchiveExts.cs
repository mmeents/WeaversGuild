using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {
  public static class AssetArchiveExts {

    public static async Task<IEnumerable<ArchiveAssetDto>?> GetArchiveAssets(this FabricDbContext context, int archiveId, 
      int PageNo, int PageSize, CancellationToken cancellationToken) 
    {
      int apn = PageNo < 1 ? 1 : PageNo;
      var result = await context.ArchiveAssets
        .AsNoTracking()
        .Where(a => a.ArchiveId == archiveId)
        .Select(ItemProjections.ToArchiveAssetDto)
        .AsSplitQuery()       
        .OrderBy(a => a.FileMtimeUtc)
        .Skip((apn-1) * PageSize)
        .Take(PageSize)
        .ToListAsync(cancellationToken);
      return result;
    }



  }



}
