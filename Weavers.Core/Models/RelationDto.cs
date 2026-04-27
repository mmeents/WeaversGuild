using Weavers.Core.Entities;

namespace Weavers.Core.Models {
  public class RelationDto {
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int? RelatedItemId { get; set; } = null;
    public int? RelatedItemTypeId { get; set; } = null;
    public string RelatedItemName { get; set; } = string.Empty;
    public int RelationTypeId { get; set; }
    public string RelationTypeName { get; set; } = string.Empty;
    public DateTime Established { get; set; }
    public int? Rank { get; set; } = null;
    public bool RelatedItemHasChildren { get; set; } = false;
  }

  public static class ItemRelationExtensions {

    public static RelationDto ToDto(this Relation relation) {
      return new RelationDto {
        Id = relation.Id,
        ItemId = relation.ItemId,
        ItemName = relation.Item?.Name ?? string.Empty,
        RelatedItemId = relation.RelatedItemId,
        RelatedItemTypeId = relation.RelatedItem?.ItemTypeId,
        RelatedItemName = relation.RelatedItem?.Name ?? string.Empty,
        RelationTypeId = relation.RelationTypeId,
        RelationTypeName = relation.RelationType?.Name ?? string.Empty,
        Rank = relation.Rank,
        Established = relation.Established,
        RelatedItemHasChildren = relation.RelatedItem?.Relations != null && relation.RelatedItem.Relations.Any()
      };
    }
  }
}
