using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Models;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;

namespace TheLoomApp.Models {
  public partial class ItemNode : TreeNode {
    public ItemNode() : base() { 
      this.Text = "Loading...";
      this.SelectedImageIndex = 0;
    }
    // grandchildren are delayed loading.
    public bool IsLoaded { get; set; } = false;
    

    // relation is to parent.
    public RelationDto? Relation { get; set; } = null; 

    public ItemDto? Item { get; set; } = null;
    
  }


  public static class ItemNodeExt {

    // special case project.
    public static ItemNode ToItemNode(this ItemDto item) {
      var type = (WeItemType)item.ItemTypeId;
      var node = new ItemNode {
        Name = item.Id.ToString(),
        ImageIndex = type.ImageIndex(),
        SelectedImageIndex = type.ImageIndex(),
        Text = item.Name,
        Item = item,
        IsLoaded = true
      };
      return node;
    }

    // general case 
    public static ItemNode ToItemNode(this RelationDto relation, ItemDto item) {
      var type = (WeItemType)item.ItemTypeId;
      var node = new ItemNode {
        Name = item.Id.ToString(),
        ImageIndex = type.ImageIndex(),
        SelectedImageIndex = type.ImageIndex(),
        Text = item.Name,
        Item = item,
        Relation = relation,
        IsLoaded = true
      };
      return node;
    }

    public static ItemNode? FindAncestorOfType(this ItemNode node, WeItemType itemType) {
      var current = node.Parent as ItemNode;
      while (current != null) {
        if (current.Item?.ItemTypeId == (int)itemType)
          return current;
        current = current.Parent as ItemNode;
      }
      return null;
    }
  }

}
