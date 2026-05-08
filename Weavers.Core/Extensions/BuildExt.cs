using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {

  public static class BuildExt {
    //public static async Task 

    public static async Task<BuildDto?> GetBuildDtoById(this FabricDbContext context,
      int buildId, CancellationToken cancellationToken) {

      // leaves off navs for performance. If we need them, we can add them in later.
      var build = await context.Builds
        .AsNoTracking()
        .Where(b => b.Id == buildId)
        .Select(b => new BuildDto {
          Id = b.Id,
          StartedAt = b.StartedAt,
          CompletedAt = b.CompletedAt,
          Status = b.Status,
          BuildOutput = b.BuildOutput,
          CompilerOutput = b.CompilerOutput,
          LibraryItemId = b.LibraryItemId,
          BuildFiles = b.BuildFiles.Select(bf => new BuildFileDto {
            Id = bf.Id,
            BuildId = bf.Id,
            ItemId = bf.ItemId,
            FilePath = bf.FilePath,
            WasWritten = bf.WasWritten,
            WasDeleted = bf.WasDeleted,
          }).ToList()
        })
        .FirstOrDefaultAsync(cancellationToken);
      if (build == null) return null;
      return build;
    }


    public static async Task<BuildFileDto> MarkBuildFileRemoved(this FabricDbContext context, int buildFileId, CancellationToken cancellationToken) {
      var buildFile = await context.BuildFiles.Where(bf => bf.Id == buildFileId).FirstOrDefaultAsync(cancellationToken);
      if (buildFile == null) throw new Exception($"BuildFile with id {buildFileId} not found.");
      buildFile.WasDeleted = true;
      await context.SaveChangesAsync(cancellationToken);
      return new BuildFileDto {
        Id = buildFile.Id,
        BuildId = buildFile.BuildId,
        ItemId = buildFile.ItemId,
        FilePath = buildFile.FilePath,
        WasWritten = buildFile.WasWritten,
        WasDeleted = buildFile.WasDeleted
      };


    }
  }
}
