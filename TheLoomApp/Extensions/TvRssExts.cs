using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLoomApp.Models;
using Weavers.Core.Enums;
using Weavers.Core.Service;
using Weavers.Core.Extensions;

namespace TheLoomApp.Extensions {
  public static class TvRssExts {

    public static async Task AddRssFolder(this TreeView _tv, IAppGraphOrgService graphSrvs, string name) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || !item.IsValidRssFolderParent()) { return; }
      var newSubItem = await graphSrvs.AddRssFolder(item, name);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task AddRssChannel(this TreeView _tv, IAppGraphOrgService graphSrvs, string name, string? channelUrl = null) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.RssFolderModel) { return; }
      var newSubItem = await graphSrvs.AddRssChannel(item, name, channelUrl);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task RssResyncChannel(this TreeView _tv, IAppGraphOrgService graphSrvs) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.RssChannelModel) { return; }
      var updatedChannelItem = await graphSrvs.RssResyncChannel(item);
    }

    public static async Task AddLinkedHtml(this TreeView _tv, IAppGraphOrgService graphSrvs, string name, string? channelUrl = null) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || item.ItemTypeId != (int)WeItemType.RssFolderModel) { return; }
      var newSubItem = await graphSrvs.AddLinkedHtml(item, name);
      if (newSubItem == null) { return; }
      await _tv.AddNewItem(newSubItem);
    }

    public static async Task RssResolveLink(this TreeView _tv, IAppGraphOrgService graphSrvs, CancellationToken ct = default) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || (
        item.ItemTypeId != (int)WeItemType.RssItemModel && item.ItemTypeId != (int)WeItemType.RssLinkedHtmlModel)
      ) { return; }
      var updatedChannelItem = await graphSrvs.RssResolveLink(item, ct);
    }
    public static async Task RssExtractLinks(this TreeView _tv, IAppGraphOrgService graphSrvs, CancellationToken ct = default) {
      ItemNode? _selectedNode = _tv.SelectedNode as ItemNode;
      var item = _selectedNode?.Item;
      if (_selectedNode == null || item == null || (
        item.ItemTypeId != (int)WeItemType.RssItemModel && item.ItemTypeId != (int)WeItemType.RssLinkedHtmlModel)
      ) { return; }
      var updatedChannelItem = await graphSrvs.RssExtractLinks(item, ct);
    }
  }
}
