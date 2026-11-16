using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.AssetArchive;
using MediatR;
using Weavers.Core.Constants;
using Weavers.Core.Models;
using Weavers.Core.Handlers.Items;

namespace Weavers.Core.Service {

  public interface IAssetArchiveSettingsStore {
    /// <summary>Read ServiceHost / RootFolder / ArchiveKind / Available off the AssetArchiveModel item; 
    /// null if not that type.</summary>
    Task<AssetArchiveSettings?> LoadAsync(int itemId, CancellationToken ct);
    Task SetAvailableAsync(int itemId, bool available, CancellationToken ct);
  }

  public sealed record AssetArchiveSettings(int ItemId, int ServiceHostId, string RootFolder, int ArchiveKind, bool Available, List<ItemDto> Policies);

  public class AssetArchiveSettingsStore : IAssetArchiveSettingsStore {
    private FabricDbContext _db;
    private IMediator _mediator;

    public AssetArchiveSettingsStore(FabricDbContext db, IMediator mediator) {
      _db = db;
      _mediator = mediator;
    }

    public async Task<AssetArchiveSettings?> LoadAsync(int itemId, CancellationToken ct) {
      var itemDto = await _db.GetItemDtoById(itemId, ct);
      if (itemDto == null || itemDto.ItemTypeId != (int)WeItemType.AssetArchiveModel) {
        return null;
      }
      var serviceHostId = itemDto.Properties.FirstOrDefault(p => p.Name == Cx.ItServiceHost)?.Value.AsInt32() ?? 0;
      var rootFolder = itemDto.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder)?.Value ?? string.Empty;
      var archiveKind = itemDto.Properties.FirstOrDefault(p => p.Name == Cx.ItArchiveKind)?.Value.AsInt32() ?? 0;
      var available = itemDto.Properties.FirstOrDefault(p => p.Name == Cx.ItAvailable)?.Value.AsBoolean() ?? false;
      var archivePoliciesRelItemIds = itemDto.Relations.Where(r => 
        r.RelatedItemTypeId == (int)WeItemType.OffloadPolicyModel || r.RelatedItemTypeId == (int)WeItemType.ReplicaPolicyModel)
        .Select(r => r.RelatedItemId).Where(id => id.HasValue && id.Value != 0).Select(i => i.Value).ToList();
      if (archivePoliciesRelItemIds.Count == 0) {
        return new AssetArchiveSettings(itemId, serviceHostId, rootFolder, archiveKind, available, new List<ItemDto>());
      } else {
        var policies = await _mediator.Send(new GetItemsByIdsQuery(archivePoliciesRelItemIds), ct);
        return new AssetArchiveSettings(itemId, serviceHostId, rootFolder, archiveKind, available, policies);
      }            
    }

    public async Task SetAvailableAsync(int itemId, bool available, CancellationToken ct) {

      var itemDto = await _db.GetItemDtoById(itemId, ct);
      if (itemDto == null || itemDto.ItemTypeId != (int)WeItemType.AssetArchiveModel) {
        throw new InvalidOperationException($"Item {itemId} is not an AssetArchiveModel.");
      }
      var availableProperty = itemDto.Properties.FirstOrDefault(p => p.Name == Cx.ItAvailable);
      if (availableProperty == null) {
        throw new InvalidOperationException($"Item {itemId} does not have an Available property.");
      }
      availableProperty.Value = available ? "1" : "0";
      await availableProperty.SaveProp(itemDto, _mediator);     

    }
  }
}
