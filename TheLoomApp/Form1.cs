using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TheLoomApp.Components;
using TheLoomApp.Extensions;
using TheLoomApp.Models;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Import;
using Weavers.Core.Handlers.Presence;
using Weavers.Core.Handlers.Sessions;
using Weavers.Core.Handlers.Todo;
using Weavers.Core.Models;
using Weavers.Core.Service;


namespace TheLoomApp {
  public partial class Form1 : Form {
    #region Initialization and Size Setup
    delegate void LogMessageDelegate(string message);
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IAppDataService _appDataService;
    private readonly IAppGraphOrgService _appGraphOrgService;
    private readonly IAppGraphFileService _appGraphService;
    private readonly IAppGraphClassService _appClassService;
    private readonly IAppItemTemplateService _itemTemplateService;
    private readonly IGraphItemUpdateService _graphItemUpdateService;

    private ItemNode? _selectedNode = null;
    private ItemDto? _CurrentItemBackup = null;
    private readonly Dictionary<int, ItemNode> _itemCache = new();
    private readonly Dictionary<int, ItemTypeDto> _itemTypeCache = new();
    private readonly HashSet<WeItemType> _generatableTypes = WeItemTypeExtensions.GetGenerativeTypes();
    private readonly PropertiesTab _itemPropertiesTab;
    private readonly IAppSettingService _settings;
    private AppSessionResponse? _sessionDetails = null;
    private bool _inResize = false;
    private bool _ItemTabDirty = false;
    private bool _RelationTabDirty = false;
    private bool _inSetupTpItems = false;
    private bool _isExpanding = false;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ItemTabDirty {
      get { return _ItemTabDirty; }
      set {
        _ItemTabDirty = value;
        if (!_inSetupTpItems) {
          btnAbortItem.Visible = value;
          btnUpdateItem.Visible = value;
        }
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RelationTabDirty {
      get { return _RelationTabDirty; }
      set {
        _RelationTabDirty = value;
      }
    }


    public Form1(IServiceScopeFactory serviceScopeFactory) {
      InitializeComponent();

      _serviceScopeFactory = serviceScopeFactory;
      using var scope = _serviceScopeFactory.CreateScope();
      _appDataService = scope.ServiceProvider.GetRequiredService<IAppDataService>();
      _appGraphOrgService = scope.ServiceProvider.GetRequiredService<IAppGraphOrgService>();
      _appGraphService = scope.ServiceProvider.GetRequiredService<IAppGraphFileService>();
      _appClassService = scope.ServiceProvider.GetRequiredService<IAppGraphClassService>();
      _itemTemplateService = scope.ServiceProvider.GetRequiredService<IAppItemTemplateService>();
      _graphItemUpdateService = scope.ServiceProvider.GetRequiredService<IGraphItemUpdateService>();

      _graphItemUpdateService.OnItemAdded += itemId => {
        this.Invoke(() => RefreshNode(itemId));
      };
      _itemPropertiesTab = new PropertiesTab(_serviceScopeFactory);
      _settings = scope.ServiceProvider.GetRequiredService<IAppSettingService>();
      SetupForStartup();
    }
    public void SetupForStartup() {
      lbClaudeLaunch.Text = WeaverExt.ClaudeExecutablePath;
      tabControl2.TabPages.Add(_itemPropertiesTab);
      _itemPropertiesTab.SetLabelRight(76);
      _itemPropertiesTab.OnPostEvent += ProjectTab_OnPostEvent;
      splitContainer3.Panel2Collapsed = true;
      btnAbortItem.Visible = false;
      btnUpdateItem.Visible = false;
      btnGenerateDesc.Visible = false;
      var aDefaultFolder = _settings[Cx.ApsDefaultFolder];
      if (aDefaultFolder != null && !string.IsNullOrEmpty(aDefaultFolder.Value)) {
        edAppDefaultFolder.Text = aDefaultFolder.Value;
      } else {
        _settings[Cx.ApsDefaultFolder] = new AppSetting { Key = Cx.ApsDefaultFolder, Value = WeaverExt.AppProjectsPath };
        edAppDefaultFolder.Text = WeaverExt.AppProjectsPath;
      }
      SettingDefaultFolderDirty = false;

    }

    private void Form1_Resize(object sender, EventArgs e) {
      if (_inResize) return;
      _inResize = true;
      var horizonalSpace = splitContainer1.Panel2.Width - splitContainer1.SplitterWidth;
      var verticalSpace = splitContainer1.Panel2.Height - (splitContainer3.Panel2Collapsed ? 0 : splitContainer3.Panel2.Height + 4);
      btnArchive.Left = horizonalSpace - btnArchive.Width - 10;
      btnGenerateDesc.Left = btnArchive.Left - btnGenerateDesc.Width - 4;
      btnWriteFile.Left = btnGenerateDesc.Left - btnWriteFile.Width - 4;


      btnAbortItem.Left = horizonalSpace - btnAbortItem.Width - 10;
      btnUpdateItem.Left = btnAbortItem.Left - btnUpdateItem.Width - 4;
      edItemName.Width = horizonalSpace - edItemName.Left - 10;
      edItemType.Width = horizonalSpace - edItemType.Left - 14 - (btnUpdateItem.Width * 2);

      tabControl2.Height = verticalSpace - (edItemType.Top + (edItemType.Height * 4));
      _inResize = false;
    }

    private void splitContainer3_SplitterMoved(object sender, SplitterEventArgs e) {
      Form1_Resize(sender, e);
    }

    #endregion

    #region Error Display
    private void tsBtnDismiss_Click(object sender, EventArgs e) {
      splitContainer3.Panel2Collapsed = true;
      btnShowErrors.Visible = true;
      Form1_Resize(sender, e);
    }

    private void btnShowErrors_Click(object sender, EventArgs e) {
      splitContainer3.Panel2Collapsed = false;
      btnShowErrors.Visible = false;
      Form1_Resize(sender, e);
    }
    private void DoLogMessage(string message) {
      if (this.InvokeRequired) {
        this.Invoke(new LogMessageDelegate(DoLogMessage), new object[] { message });
      } else {
        if (splitContainer3.Panel2Collapsed) {
          splitContainer3.Panel2Collapsed = false;
          Form1_Resize(this, EventArgs.Empty);
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        sb.Append(tbErrorOut.Text);
        string finalMessage = sb.ToString();
        if (finalMessage.Length > 10000) {
          finalMessage = finalMessage.Substring(0, 10000); // Keep only the last 10,000 characters
        }
        tbErrorOut.Text = finalMessage;

      }
    }

    #endregion

    #region Tree View Loading and Events
    private async void Form1_Shown(object sender, EventArgs e) {
      _sessionDetails = await _appDataService.GetAppSession(); // create session.
      await _appDataService.EnforceDataRetentionOrgPolicy();
      await LoadRootProjects();
      await LoadItemTypesCache();
      Form1_Resize(sender, e);
      this.Invoke((Action)(async () => {
        await wvDescription.EnsureCoreWebView2Async().ConfigureAwait(false);
      }));
    }

    private async Task LoadRootProjects(int? NodeIdToExpand = null, CancellationToken ct = default) {

      List<int> expandedNodeIds = new List<int>();
      int? selectedItemId = null;

      try {

        if (tvKb.SelectedNode != null) {
          if (NodeIdToExpand.HasValue) {
            expandedNodeIds.Add(NodeIdToExpand.Value);
          }
          selectedItemId = (tvKb.SelectedNode as ItemNode)?.Item?.Id;
          if (selectedItemId.HasValue) expandedNodeIds.Add(selectedItemId.Value);
          var workingNode = tvKb.SelectedNode as ItemNode;
          while (workingNode?.Parent != null) {
            workingNode = workingNode.Parent as ItemNode;
            if (workingNode != null && workingNode.Item != null) {
              expandedNodeIds.Add(workingNode.Item.Id);
            }
          }
        }

        tvKb.BeginUpdate();
        try {
          tvKb.Nodes.Clear();
          var project = await _appDataService.GetItemById(_sessionDetails!.OrganizationId);
          if (project != null) {
            ItemNode projectNode = project.ToItemNode();
            _itemCache[project.Id] = projectNode;
            if (project.Relations.Count > 0) {
              foreach (var rel in project.Relations.OrderBy(r => r.RelatedItemTypeId).ThenBy(r => r.Rank)) {
                if (rel.RelatedItemId.HasValue) {

                  if (expandedNodeIds.Contains(rel.RelatedItemId.Value)) {
                    await AddNodeById(expandedNodeIds, projectNode, rel.RelatedItemId.Value, rel);
                  } else {

                    var item = await _appDataService.GetItemById(rel.RelatedItemId.Value);
                    if (item != null) {

                      ItemNode projectsChildNode = rel.ToItemNode(item);
                      _itemCache[item.Id] = projectsChildNode;
                      projectNode.Nodes.Add(projectsChildNode);


                      if (item.Relations.Count > 0) {
                        projectsChildNode.Nodes.Add(new ItemNode());
                      }
                    }
                  }
                }
              }
            }
            var tnparentIndex = tvKb.Nodes.Add(projectNode);
          }

        } finally {
          tvKb.EndUpdate();
        }

        if (expandedNodeIds.Count > 0 && selectedItemId.HasValue) {
          // expandedNodeIds was built root-first so reverse gives you top-down
          foreach (var id in expandedNodeIds.AsEnumerable().Reverse()) {
            var nodelist = tvKb.Nodes.Find(id.ToString(), true);
            if (nodelist.Length > 0) {
              var node = nodelist[0];
              _isExpanding = true;
              node.Expand();
              _isExpanding = false;
            }
          }
          var target = tvKb.Nodes.Find(selectedItemId.Value.ToString(), true);
          if (target.Length > 0) {
            tvKb.SelectedNode = target[0];
            tvKb.SelectedNode.EnsureVisible();
          }
        }

        ItemTabDirty = false;

      } catch (Exception ex) {
        MessageBox.Show($"Error loading root projects: {ex.Message}");
      }
    }

    private async Task<ItemNode?> AddNodeById(List<int> toLoad, ItemNode parent, int? itemId, RelationDto? relation = null) {
      if (itemId == null) return null;

      if (toLoad.Contains(itemId.Value)) {
        var bItem = await _appDataService.GetItemById(itemId.Value);  // get new item
        if (bItem != null) {
          ItemNode newNode = relation == null ? bItem.ToItemNode() : relation.ToItemNode(bItem);
          _itemCache[bItem.Id] = newNode;

          if (!_showPkgInLib && bItem.ItemTypeId == (int)WeItemType.LibPackageRefModel) {
            return null;  // skip adding to tree.
          }
          if (!_showSessions && (bItem.ItemTypeId == (int)WeItemType.HarnessAppSessionModel)) {
            return null;  // skip adding to tree.
          }

          parent.Nodes.Add(newNode);   // add the node to the tree

          if (bItem.Relations.Count > 0) {
            foreach (var rel in bItem.Relations.OrderBy(r => r.RelatedItemTypeId).ThenBy(r => r.Rank)) {
              var relatedItem = await AddNodeById(toLoad, newNode, rel.RelatedItemId, rel);
            }
          }
          return newNode;
        }
      } else {
        if (relation == null || !relation.RelatedItemId.HasValue) { return null; }
        var aItem = await _appDataService.GetItemById(relation.RelatedItemId.Value);  // get new item
        if (aItem != null) {
          ItemNode projectsChildNode = relation.ToItemNode(aItem);  // make the node
          _itemCache[aItem.Id] = projectsChildNode;

          if (!_showPkgInLib && aItem.ItemTypeId == (int)WeItemType.LibPackageRefModel) {
            return null;  // skip adding to tree.
          }
          if (!_showSessions && (aItem.ItemTypeId == (int)WeItemType.HarnessAppSessionModel)) {
            return null;  // skip adding to tree.
          }
          parent.Nodes.Add(projectsChildNode);  // add the node to the tree
          if (aItem.Relations.Count > 0) {
            projectsChildNode.Nodes.Add(new ItemNode());
          }

        }
      }
      return null;
    }

    private async void TvKb_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
      if (e.Node is not ItemNode node) return;
      if (_isExpanding) return;
      _isExpanding = true;
      e.Cancel = true;  // prevent expand until data is ready
      node.Nodes.Clear();
      if (node.Item == null) return;
      var item = await _appDataService.GetItemById(node.Item.Id);
      if (item != null) {
        foreach (var rel in item.Relations.OrderBy(r => r.RelatedItemTypeId).ThenBy(r => r.Rank)) {
          if (rel.RelatedItemId.HasValue) {
            var itemChild = await _appDataService.GetItemById(rel.RelatedItemId.Value);
            if (itemChild != null) {
              ItemNode itemsChildNode = rel.ToItemNode(itemChild);
              _itemCache[itemChild.Id] = itemsChildNode;
              if (!_showPkgInLib && itemChild.ItemTypeId == (int)WeItemType.LibPackageRefModel) {
                continue;
              }
              if (!_showSessions && (itemChild.ItemTypeId == (int)WeItemType.HarnessAppSessionModel)) {
                continue;
              }
              node.Nodes.Add(itemsChildNode);
              if (itemChild.Relations.Count > 0) {
                itemsChildNode.Nodes.Add(new ItemNode());
              }
            }

          }
        }
        node.Expand();
      }
      _isExpanding = false;
    }

    private async Task LoadItemTypesCache() {
      try {
        _inSetupTpItems = true;
        var itemTypes = await _appDataService.GetAllItemTypesAsync();
        _itemTypeCache.Clear();
        foreach (var itemType in itemTypes) {
          _itemTypeCache[itemType.Id] = itemType;
        }

        edItemType.DataSource = _itemTypeCache.Values.ToList();
        edItemType.DisplayMember = "Name";
        edItemType.ValueMember = "Id";

      } catch (Exception ex) {
        MessageBox.Show($"An error occurred while loading project types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      _inSetupTpItems = false;

    }

    private async void RefreshNode(int itemId) {
      if (_itemCache.TryGetValue(itemId, out ItemNode? node)) {
        tvKb.BeginUpdate();
        try {
          node.Collapse();

          var itemDto = await _appDataService.GetItemById(itemId);
          if (itemDto != null && node.Relation != null) {
            node.Relation.ToItemNode(itemDto);
            node.Item = itemDto;
            _itemCache[itemId] = node;
          }
          node.Expand();
          node.ExpandAll();
        } finally {
          tvKb.EndUpdate();
        }

      }
    }

    private async void RefreshSelectedNode(int itemId) {
      if (_itemCache.TryGetValue(itemId, out ItemNode? node)) {
        tvKb.BeginUpdate();
        try {
          node.Collapse();

          var itemDto = await _appDataService.GetItemById(itemId);
          if (itemDto != null && node.Relation != null) {
            var newNode = node.Relation.ToItemNode(itemDto);
            node.Item = itemDto;
            _itemCache[itemId] = node;

          }
          node.Expand();
          node.ExpandAll();
        } finally {
          tvKb.EndUpdate();
        }

        TreeViewEventArgs ee = new TreeViewEventArgs(_selectedNode);
        tvKb_AfterSelect(this, ee);

      }
    }

    #endregion    

    #region Tree View Selection  

    private void tvKb_AfterSelect(object sender, TreeViewEventArgs e) {
      if (e.Node is not ItemNode) {
        _selectedNode = null;
        return;
      }
      _selectedNode = e.Node as ItemNode;
      var itemId = _selectedNode?.Item?.Id;
      var item = _selectedNode?.Item;
      if (itemId.HasValue && _selectedNode != null && item != null) {
        SetupTpRelations();
        SetupTpItems();
        SetupPropertiesTabForItem(item);
      }
    }


    private void SetupPropertiesTabForItem(ItemDto item) {
      if (_selectedNode != null && _selectedNode.Item != null && _selectedNode.Item.Properties != null) {
        var propList = _selectedNode.Item.Properties.ToList();
        foreach (var prop in propList) {
          prop.Item = item;
        }
        _itemPropertiesTab.ItemProps = propList;
        _itemPropertiesTab.SetLabelRight(Cx.intPropertyLabelLeft);
        _itemPropertiesTab.SetEditingMode(false);
      }
    }

    private void SetupTpItems() {
      if (_selectedNode != null && _selectedNode.Item != null) {
        _inSetupTpItems = true;
        var item = _selectedNode.Item;
        btnWriteFile.Visible = item.ItemTypeId == (int)WeItemType.LibraryModel
          || item.ItemTypeId == (int)WeItemType.SolutionModel
          || item.ItemTypeId == (int)WeItemType.OrganizationModel
          || item.ItemTypeId.IsContentType();
        btnAttemptTodo.Visible = item.ItemTypeId == (int)WeItemType.TodoModel;
        _CurrentItemBackup = _selectedNode.Item.Clone();

        wvDescription.SetupHtmlViewForItem(item);

        lbItemId.Text = "ItemId: " + _selectedNode.Item.Id.ToString();
        edItemType.DataBindings.Clear();
        edItemType.DataBindings.Add("SelectedValue", _selectedNode.Item, "ItemTypeId", true, DataSourceUpdateMode.OnPropertyChanged);
        edItemName.DataBindings.Clear();
        edItemName.DataBindings.Add("Text", _selectedNode.Item, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
        if (item.ItemTypeId.IsMethodCodeType()) {
          edItemDesc.DataBindings.Clear();
          edItemDesc.DataBindings.Add("Text", _selectedNode.Item, "", true, DataSourceUpdateMode.OnPropertyChanged);
        } else {
          edItemDesc.DataBindings.Clear();
          edItemDesc.DataBindings.Add("Text", _selectedNode.Item, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        edItemData.DataBindings.Clear();
        edItemData.DataBindings.Add("Text", _selectedNode.Item, "Data", true, DataSourceUpdateMode.OnPropertyChanged);
        btnGenerateDesc.Visible = _generatableTypes.Contains((WeItemType)item.ItemTypeId);
        _inSetupTpItems = false;
        ItemTabDirty = false;
      }
    }

    private void SetupTpRelations() {
      if (_selectedNode != null && _selectedNode.Item != null && _selectedNode.Relation != null) {
        var rel = _selectedNode.Relation;
        lbRelationId.Text = "Parent Id:" + rel.ItemId.ToString() + " " +
          rel.RelationTypeName + " itemId:" + _selectedNode.Item.Id.ToString();
      } else {
        lbRelationId.Text = "Parent Id: N/A";
      }
    }

    private void tabControl1_SelectedIndexChanged(object sender, EventArgs e) {
      if (tabControl1.SelectedTab == tpSchedule) {
        ReloadSchedules();
      } else if (tabControl1.SelectedTab == tpReview) {
        ReloadReadyTab();
      } else if (tabControl1.SelectedTab == tpResults) {
        ReloadResultsTab();
      }
    }
    #endregion

    #region Editing Items and Relations

    // ---------------post event ------------------------------------------------//
    private async void ProjectTab_OnPostEvent() {
      try {
        if (_selectedNode != null && _selectedNode.Item != null && _selectedNode.Item.Properties != null) {
          _itemPropertiesTab.ItemProps = _selectedNode.Item.Properties.ToList();
          _itemPropertiesTab.SetEditingMode(false);
          _itemPropertiesTab.SetLabelRight(Cx.intPropertyLabelLeft);


          if (_selectedNode.Item.ItemTypeId.IsOnPostPathUpdate()) {
            var folderProp = _selectedNode.Item.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder
              || p.Name == Cx.ItRelativeFolder || p.Name == Cx.ItFilePath);
            if (folderProp != null) {
              string basePath = folderProp.Value ?? "";
              await UpdateFolderPathIfNeededAsync(_selectedNode.Item, basePath);
            }
          } else if (_selectedNode.Item.ItemTypeId == (int)WeItemType.HarnessGatewaysModel) {
            var hasLmStudio = _selectedNode.Item.Properties.FirstOrDefault(p => p.Name == Cx.ItHasLmStudioPresence)?.Value.AsBoolean();
            var hasClaude = _selectedNode.Item.Properties.FirstOrDefault(p => p.Name == Cx.ItHasClaudePresence)?.Value.AsBoolean();
            await _appDataService.SyncHarnessPresence(_selectedNode.Item.Id, hasLmStudio, hasClaude);
            var ee = new TreeViewCancelEventArgs(_selectedNode, false, TreeViewAction.Expand);
            TvKb_BeforeExpand(ee, ee);
          } else if (_selectedNode.Item.ItemTypeId == (int)WeItemType.PresenceLmStudioGatewayModel) {
            var item = _selectedNode.Item;
            var resyncProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItReSync);
            if (resyncProp != null) {
              bool shouldResync = resyncProp.Value.AsBoolean();
              if (shouldResync) {
                var updatedGateway = await _appDataService.SyncLmStudioModels(item.Id);
                if (updatedGateway != null) {
                  _selectedNode.Item = updatedGateway;
                }
                var ee = new TreeViewCancelEventArgs(_selectedNode, false, TreeViewAction.Expand);
                TvKb_BeforeExpand(ee, ee);
              }
            }

          } else if (_selectedNode.Item.ItemTypeId == (int)WeItemType.DependencyInjectionModel) {
            var hasDbContext = _selectedNode.Item.Properties.Any(p => p.Name == Cx.ItHasDbContext && p.Value.AsBoolean());
            await _appDataService.AddRemoveDbContextToLibDi(_selectedNode.Item.Id, hasDbContext);

            if (!hasDbContext && _selectedNode.Nodes.Count > 0) {
              foreach (ItemNode node in _selectedNode.Nodes) {
                if (node.Item != null && node.Item.ItemTypeId == (int)WeItemType.DbContextModel) {
                  node.Remove();
                }
              }
            } else if (hasDbContext && _selectedNode.Nodes.Count == 0) {
              var ee = new TreeViewCancelEventArgs(_selectedNode, false, TreeViewAction.Expand);
              TvKb_BeforeExpand(ee, ee);
            }
            var hasMediatR = _selectedNode.Item.Properties.Any(p => p.Name == Cx.ItHasMediator && p.Value.AsBoolean());
            var itemId = _selectedNode.Item.Id;
            if (itemId != 0 && _itemTemplateService != null) {
              string? newDescription = await _itemTemplateService.GetDependencyInjectionTemplate(itemId);
              if (newDescription != null) {
                _selectedNode.Item.Description = newDescription;
                await _appDataService.UpdateItemAsync(_selectedNode.Item);
              }
            }
          } else if (_selectedNode.Item.ItemTypeId == (int)WeItemType.ClassModel) {
            var itemId = _selectedNode.Item.Id;
            var propGenerateInterface = _selectedNode.Item.Properties.FirstOrDefault(p => p.Name == Cx.ItGenerateInterface);
            bool generateInterface = false;
            if (propGenerateInterface != null) {
              generateInterface = propGenerateInterface.Value.AsBoolean();
            }

            var propRegisterDi = _selectedNode.Item.Properties.FirstOrDefault(p => p.Name == Cx.ItRegisterDi);
            if (propRegisterDi != null) {
              var registerDi = propRegisterDi.Value.AsBoolean();
              await _appDataService.AddRemoveClassToLibDi(_selectedNode.Item.Id, registerDi, generateInterface);
            }

            if (itemId != 0 && _itemTemplateService != null) {
              string? newDesc = await _itemTemplateService.GetClassTemplate(itemId);
              if (newDesc != null) {
                _selectedNode.Item.Description = newDesc;
                await _appDataService.UpdateItemAsync(_selectedNode.Item);
              }
            }

          } else if (_selectedNode.Item.ItemTypeId == (int)WeItemType.EntityPropertyModel
              || _selectedNode.Item.ItemTypeId == (int)WeItemType.EntityNavigationModel) {
            ItemNode? parent = _selectedNode.Parent as ItemNode;
            if (parent != null) {
              var prevParent = _selectedNode;
              while (parent != null && parent.Item != null && parent.Item.ItemTypeId != (int)WeItemType.EntityClassModel) {
                prevParent = parent;
                parent = parent.Parent as ItemNode;
              }
              if (parent != null) {
                await _appDataService.ProcessPropertyUpdate(parent.Item!, prevParent.Item);
              }
            }
          }

        }
      } catch (Exception ex) {
        DoLogMessage("Failed to update item properties - error:" + ex.Message);
        MessageBox.Show($"Error updating item properties: {ex.Message}", "Update Failed");
      }
    }

    private async void btnUpdateItem_Click(object sender, EventArgs e) {
      try {
        await UpdateItemAsync();
      } catch (Exception ex) {
        DoLogMessage("Failed to update item - error:" + ex.Message);
        MessageBox.Show($"Error updating item: {ex.Message}", "Update Failed");
      }
    }

    private async Task UpdateItemAsync() {
      if (_selectedNode != null && _selectedNode.Item != null) {
        await _appDataService.UpdateItemAsync(_selectedNode.Item);
        _selectedNode.Text = _selectedNode.Item.Id.ToString() + ": " + _selectedNode.Item.Name;
        this.Invoke(() => RefreshSelectedNode(_selectedNode.Item.Id));
      }
    }

    private async Task UpdateFolderPathIfNeededAsync(ItemDto item, string basePath) {
      string propKey = item.ItemTypeId.GetFolderPropertyName();
      if (propKey == "") return;

      var folderProp = item.Properties.FirstOrDefault(p => p.Name == propKey);
      if (folderProp == null) return;

      bool isOrg = false;
      var fullPath = "";
      var fileName = "";  // namespaces are folders within a file and get add to the path the file is in.
      var fileBasePath = basePath;
      if (item.ItemTypeId.IsAParentFolder()) {
        fullPath = Path.Combine(basePath, item.Name.UrlSafe());
      } else if (propKey == Cx.ItFilePath) {
        if (item.ItemTypeId.IsFileNameType()) {
          fileBasePath = Path.GetDirectoryName(fileBasePath) ?? basePath;
        }
        fileName = item.GetFileName();
        fullPath = Path.Combine(fileBasePath, fileName);
      } else {
        isOrg = true;
        fullPath = basePath;
      }
      if (folderProp.Value != fullPath || isOrg) {
        folderProp.Value = fullPath;
        var updated = await _appDataService.AddUpdateItemPropertyAsync(folderProp);
        if (updated != null) item.AddOrUpdateProperty(updated);
        await _appDataService.UpdateItemPropertyPathRecursive(item.Id, basePath, fullPath);
      }

    }

    // abort a edit.
    private void btnAbortItem_Click(object sender, EventArgs e) {
      if (_selectedNode != null && _selectedNode.Item != null && _CurrentItemBackup != null) {
        _selectedNode.Item.Name = _CurrentItemBackup.Name;
        _selectedNode.Item.Description = _CurrentItemBackup.Description;
        _selectedNode.Item.Data = _CurrentItemBackup.Data;
        _selectedNode.Item.ItemTypeId = _CurrentItemBackup.ItemTypeId;
        SetupTpItems();
      }
    }

    private void edItemName_TextChanged(object sender, EventArgs e) {
      if (!_inSetupTpItems) {
        ItemTabDirty = true;
      }
    }

    private void edItemDesc2_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
      if (!_inSetupTpItems) {
        ItemTabDirty = true;
      }
    }

    private async void btnArchive_Click(object sender, EventArgs e) {
      if (_selectedNode != null && _selectedNode.Item != null) {
        var confirmResult = MessageBox.Show($"Are you sure you want to archive '{_selectedNode.Item.Name}'? This will remove it from the tree but keep it in the database.", "Confirm Archive", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirmResult == DialogResult.Yes) {
          _selectedNode.Item.IsActive = false;
          await _appDataService.UpdateItemAsync(_selectedNode.Item);
          var newParent = _selectedNode.Parent;
          var archivedNode = _selectedNode;
          tvKb.SelectedNode = newParent;  // this sets the selected node.
          tvKb.Nodes.Remove(archivedNode);
        }
      }
    }

    #endregion

    #region Context Menu Events
    private void cmsTreeMenus_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
      if (_selectedNode == null || _selectedNode.Item == null) {
        miAddOrgRole.Visible = false;
        miAddOrgDesk.Visible = false;
        miAddDeskTodo.Visible = false;
        miAddForeachTodo.Visible = false;
        miAddDigitalOperator.Visible = false;
        miAddOrgFolder.Visible = false;
        miAddOrgFile.Visible = false;
        miAddProjectRoot.Visible = true;
        miAddSubProject.Visible = false;
        miAddSolution.Visible = false;
        miAddSolutionImport.Visible = false;
        miAddFile.Visible = false;
        miAddLibrary.Visible = false;
        miAddDiModel.Visible = false;
        miAddNamespace.Visible = false;
        miAddClass.Visible = false;
        miAddClassImport.Visible = false;
        miAddClassProp.Visible = false;
        miAddClassMethod.Visible = false;
        miAddClassMethodParam.Visible = false;
        miAddEntity.Visible = false;
        miAddEntityProperty.Visible = false;

        miGenerate.Visible = false;
        toolStripSeparator2.Visible = false;
        miDeleteItem.Enabled = false;
      } else {
        var itemType = (WeItemType)_selectedNode.Item.ItemTypeId;
        var hasDiModel = false;
        if (itemType == WeItemType.LibraryModel) {
          hasDiModel = _selectedNode.Item.Relations.Any(r => r.RelationTypeId == (int)WeRelationTypes.Contains
            && r.RelatedItemId != null && r.RelatedItemTypeId == (int)WeItemType.DependencyInjectionModel);
        }
        miAddOrgRole.Visible = itemType == WeItemType.OrgDeskRolesModel;
        miAddOrgDesk.Visible = itemType == WeItemType.WorkGroupModel;
        miAddDeskTodo.Visible = itemType == WeItemType.DeskModel;
        miAddForeachTodo.Visible = itemType == WeItemType.DeskModel;
        miAddDigitalOperator.Visible = itemType == WeItemType.DigitalOperatorPoolModel;
        miAddOrgFolder.Visible = itemType == WeItemType.OrganizationModel || itemType == WeItemType.OrgDocFolderModel;
        miAddOrgFile.Visible = itemType == WeItemType.OrgDocFolderModel || itemType == WeItemType.OrganizationModel;
        miAddProjectRoot.Visible = itemType == WeItemType.OrganizationModel || itemType == WeItemType.ProjectFolderModel || itemType == WeItemType.RelativeFolderModel;
        miAddSubProject.Visible = itemType == WeItemType.ProjectFolderModel || itemType == WeItemType.RelativeFolderModel;
        miAddSolution.Visible = itemType == WeItemType.ProjectFolderModel || itemType == WeItemType.RelativeFolderModel;
        miAddSolutionImport.Visible = itemType == WeItemType.SolutionModel;
        miAddFile.Visible = itemType == WeItemType.ProjectFolderModel || itemType == WeItemType.RelativeFolderModel;
        miAddLibrary.Visible = itemType == WeItemType.ProjectFolderModel || itemType == WeItemType.RelativeFolderModel;
        miAddDiModel.Visible = itemType == WeItemType.LibraryModel && !hasDiModel;
        miAddNamespace.Visible = itemType == WeItemType.LibraryModel || itemType == WeItemType.NamespaceModel;
        miAddClass.Visible = itemType == WeItemType.LibraryModel || itemType == WeItemType.NamespaceModel;
        miAddClassImport.Visible = itemType == WeItemType.ClassModel;
        miAddClassProp.Visible = itemType == WeItemType.ClassModel;
        miAddClassMethod.Visible = itemType == WeItemType.ClassModel;
        miAddClassMethodParam.Visible = itemType == WeItemType.ClassMethodModel;
        miAddEntity.Visible = itemType == WeItemType.LibraryModel || itemType == WeItemType.NamespaceModel;
        miAddEntityProperty.Visible = itemType == WeItemType.EntityClassModel;
        miGenerate.Visible = true;
        toolStripSeparator2.Visible = true;
        miDeleteItem.Enabled = true;

      }
    }

    private async void miReloadTree_Click(object sender, EventArgs e) {
      _appDataService.ClearCache();
      await LoadRootProjects();
    }



    private async void miAddOrgRole_Click(object sender, EventArgs e) {
      try {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.DeskRoleModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          await tvKb.AddOrgDeskRole(_appGraphOrgService, newItemName);
        }

      } catch (Exception ex) {
        DoLogMessage("Failed to add project root - error:" + ex.Message);
        MessageBox.Show($"Error adding project: {ex.Message}", "Add Project Failed");
      }
    }

    private async void miAddDigitalOperator_Click(object sender, EventArgs e) {
      try {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.DigitalOperatorModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          await tvKb.AddDigitalOperator(_appGraphOrgService, newItemName);
        }

      } catch (Exception ex) {
        DoLogMessage("Failed to add project root - error:" + ex.Message);
        MessageBox.Show($"Error adding project: {ex.Message}", "Add Project Failed");
      }
    }

    private async void miAddOrgDesk_Click(object sender, EventArgs e) {
      try {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.DeskModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          await tvKb.AddDesk(_appGraphOrgService, newItemName);
        }

      } catch (Exception ex) {
        DoLogMessage("Failed to add desk - error:" + ex.Message);
        MessageBox.Show($"Error adding desk: {ex.Message}", "Add Desk Failed");
      }
    }

    private async void miAddDeskTodo_Click(object sender, EventArgs e) {
      try {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.TodoModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          await tvKb.AddDeskTodo(_appGraphOrgService, newItemName, null, null);
        }

      } catch (Exception ex) {
        DoLogMessage("Failed to add desk todo - error:" + ex.Message);
        MessageBox.Show($"Error adding desk todo: {ex.Message}", "Add Desk Todo Failed");
      }
    }


    private async void miAddForeachTodo_Click(object sender, EventArgs e) {
      try {

        using var dlg = new AddTodoForeachDialog(_serviceScopeFactory);
        var selectedNode = tvKb.SelectedNode;
        var selectedItem = (selectedNode as ItemNode)?.Item;
        if (selectedItem == null || selectedItem.ItemTypeId != (int)WeItemType.DeskModel) {
          MessageBox.Show("Selected item must be a desk to add a foreach todo.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          return;
        }
        dlg.DeskId = selectedItem.Id;
        dlg.DeskName = selectedItem.Name;
        if (dlg.ShowDialog() == DialogResult.OK) {
          var foreachList = dlg.ForeachList.Split(Environment.NewLine, options: StringSplitOptions.RemoveEmptyEntries);
          var refId = dlg.RefId;
          var promptTemplate = "";
          foreach (var foreachItem in foreachList) {
            promptTemplate = dlg.PromptTemplate + Environment.NewLine + foreachItem;
            await tvKb.AddDeskTodo(_appGraphOrgService, null, refId, promptTemplate);
            // add desk sets selection to new todo.  we need to move selection back to desk.
            if (_selectedNode != null) {
              tvKb.SelectedNode = _selectedNode.Parent;
            }
          }
        }
        var ee = new TreeViewCancelEventArgs(_selectedNode, false, TreeViewAction.Expand);
        TvKb_BeforeExpand(ee, ee);

      } catch (Exception ex) {
        DoLogMessage("Failed to add desk todo - error:" + ex.Message);
        MessageBox.Show($"Error adding desk todo: {ex.Message}", "Add Desk Todo Failed");
      }
    }

    private async void miAddOrgFolder_Click(object sender, EventArgs e) {
      try {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.OrgDocFolderModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          await tvKb.AddOrgFolder(_appGraphOrgService, newItemName);
        }

      } catch (Exception ex) {
        DoLogMessage("Failed to add org folder - error:" + ex.Message);
        MessageBox.Show($"Error adding folder: {ex.Message}", "Add Folder Failed");
      }
    }

    private async void miAddOrgFile_Click(object sender, EventArgs e) {
      try {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.OrgDocModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          await tvKb.AddOrgFile(_appGraphOrgService, newItemName);
        }

      } catch (Exception ex) {
        DoLogMessage("Failed to add file - error:" + ex.Message);
        MessageBox.Show($"Error adding file: {ex.Message}", "Add File Failed");
      }
    }

    private async void miAddProjectRoot_Click(object sender, EventArgs e) {
      try {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.ProjectFolderModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          await tvKb.AddProjectRoot(_appGraphService, newItemName, edAppDefaultFolder.Text);
        }

      } catch (Exception ex) {
        DoLogMessage("Failed to add project root - error:" + ex.Message);
        MessageBox.Show($"Error adding project: {ex.Message}", "Add Project Failed");
      }
    }
    private async void miAddSubProject_Click(object sender, EventArgs e) {
      try {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.RelativeFolderModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          await tvKb.AddSubFolder(_appGraphService, newItemName);
        }

      } catch (Exception ex) {
        DoLogMessage("Failed to add subfolder - error:" + ex.Message);
        MessageBox.Show($"Error adding folder: {ex.Message}", "Add Folder Failed");
      }
    }
    private async void miAddSolution_Click(object sender, EventArgs e) {
      try {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.SolutionModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          await tvKb.AddSolution(_appGraphService, newItemName);
        }

      } catch (Exception ex) {
        DoLogMessage("Failed to add solution - error: " + ex.Message);
        MessageBox.Show($"Error adding solution: {ex.Message}", "Add Solution failed");
      }
    }
    private async void miAddSolutionImport_Click(object sender, EventArgs e) {
      try {
        await tvKb.AddSolutionImport(_appGraphService);
      } catch (Exception ex) {
        DoLogMessage("Failed to add solution import - error: " + ex.Message);
        MessageBox.Show($"Error adding solution import: {ex.Message}", "Add Solution failed");
      }
    }
    private async void miAddFile_Click(object sender, EventArgs e) {
      try {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.FileMdModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          var fileType = dlg.NewFileType;
          if (fileType != null) {
            switch (fileType) {
              case WeItemType.FileMdModel:
                await tvKb.AddMdFile(_appGraphService, newItemName);
                break;
              case WeItemType.FileHtmlModel:
                await tvKb.AddHtmlFile(_appGraphService, newItemName);
                break;
              case WeItemType.FileConfigModel:
                await tvKb.AddConfigFile(_appGraphService, newItemName);
                break;
              default:
                break;
            }
          }
        }

      } catch (Exception ex) {
        DoLogMessage("Failed to add file - error:" + ex.Message);
        MessageBox.Show($"Error adding file: {ex.Message}", "Add File Failed");
      }
    }

    private async void miAddLibrary_Click(object sender, EventArgs e) {
      try {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.LibraryModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          using var scope = _serviceScopeFactory.CreateScope();
          var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
          await tvKb.AddLibrary(mediator, _appClassService, newItemName);
        }

      } catch (Exception ex) {
        DoLogMessage("Failed to add library - error:" + ex.Message);
        MessageBox.Show($"Error adding library: {ex.Message}", "Add Library Failed");
      }
    }
    private async void miAddDiModel_Click(object sender, EventArgs e) {
      try {
        await tvKb.AddDiModel(_appClassService);
      } catch (Exception ex) {
        DoLogMessage("Failed to add DI model - error:" + ex.Message);
        MessageBox.Show($"Error adding DI model: {ex.Message}", "Add DI Model Failed");
      }
    }
    private async void miAddNamespace_Click(object sender, EventArgs e) {
      try {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.NamespaceModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          await tvKb.AddNamespaceModel(_appClassService, newItemName);
        }

      } catch (Exception ex) {
        DoLogMessage("Failed to add namespace model - error:" + ex.Message);
        MessageBox.Show($"Error adding namespace model: {ex.Message}", "Add Namespace Model Failed");
      }
    }
    private async void miAddClass_Click(object sender, EventArgs e) {
      try {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.ClassModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          await tvKb.AddClassModel(_appClassService, newItemName);
        }


      } catch (Exception ex) {
        DoLogMessage("Failed to add class model - error:" + ex.Message);
        MessageBox.Show($"Error adding class model: {ex.Message}", "Add Class Model Failed");
      }
    }
    private async void miAddClassImport_Click(object sender, EventArgs e) {
      try {
        await tvKb.AddClassImportModel(_appClassService);
      } catch (Exception ex) {
        DoLogMessage("Failed to add class Import - error:" + ex.Message);
        MessageBox.Show($"Error adding class import: {ex.Message}", "Add Class Import Failed");
      }
    }
    private async void miAddClassProp_Click(object sender, EventArgs e) {
      try {
        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.ClassPropertyModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          int? newItemTypeId = dlg.NewDataType.HasValue ? (int?)dlg.NewDataType.Value : null;
          int? newItemClassId = dlg.LookupItemId;
          await tvKb.AddClassPropModel(_appClassService, newItemName, newItemTypeId, newItemClassId);
        }
      } catch (Exception ex) {
        DoLogMessage("Failed to add class property - error:" + ex.Message);
        MessageBox.Show($"Error adding class property: {ex.Message}", "Add Class Property Failed");
      }
    }
    private async void miAddClassMethod_Click(object sender, EventArgs e) {
      try {
        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.ClassMethodModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          bool? isAsync = dlg.IsAsync;
          int? returnTypeId = dlg.NewDataType.HasValue ? (int?)dlg.NewDataType.Value : null;
          int? returnClassId = dlg.LookupItemId;
          await tvKb.AddClassMethodModel(_appClassService, newItemName, isAsync, returnTypeId, returnClassId);
        }

      } catch (Exception ex) {
        DoLogMessage("Failed to add class method - error:" + ex.Message);
        MessageBox.Show($"Error adding class method: {ex.Message}", "Add Class Method Failed");
      }
    }
    private async void miAddClassMethodParam_Click(object sender, EventArgs e) {
      try {
        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.ClassMethodParameterModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          int? paramTypeId = dlg.NewDataType.HasValue ? (int?)dlg.NewDataType.Value : null;
          int? paramClassId = dlg.LookupItemId;
          await tvKb.AddClassMethodParameterModel(_appClassService, newItemName, paramTypeId, paramClassId);
        }
      } catch (Exception ex) {
        DoLogMessage("Failed to add class method parameter - error:" + ex.Message);
        MessageBox.Show($"Error adding class method parameter: {ex.Message}", "Add Class Method Parameter Failed");
      }
    }

    private async void miAddEntity_Click(object sender, EventArgs e) {
      try {
        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.EntityClassModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var className = dlg.ItemName;
          var dbTableName = dlg.DbTableName;
          await tvKb.AddEntityClassModel(_appClassService, className, dbTableName);
        }
      } catch (Exception ex) {
        DoLogMessage("Failed to add entity - error:" + ex.Message);
        MessageBox.Show($"Error adding entity: {ex.Message}", "Add Entity Failed");
      }
    }

    private async void miAddEntityProperty_Click(object sender, EventArgs e) {
      try {
        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, WeItemType.EntityPropertyModel);
        if (dlg.ShowDialog() == DialogResult.OK) {
          bool isNavigation = dlg.IsNav;
          var propertyName = dlg.ItemName;
          var propertyType = dlg.NewDataType;
          var entityId = dlg.LookupItemId;
          await tvKb.AddEntityPropertyModel(_appClassService, propertyName, (int?)propertyType, isNavigation, entityId);
        }
      } catch (Exception ex) {
        DoLogMessage("Failed to add entity property - error:" + ex.Message);
        MessageBox.Show($"Error adding entity property: {ex.Message}", "Add Entity Property Failed");
      }
    }


    private void miDeleteItem_Click(object sender, EventArgs e) {
      var itemNode = _selectedNode;
      if (itemNode == null || itemNode.Item == null) return;
      var confirmResult = MessageBox.Show($"Are you sure you want to delete '{itemNode.Item.Name}'? This action cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
      if (confirmResult == DialogResult.Yes) {
        _appDataService.DeleteItemAsync(itemNode.Item.Id);
        var parentNode = itemNode.Parent;
        tvKb.SelectedNode = parentNode;  // this sets the selected node.
        tvKb.Nodes.Remove(itemNode);
      }
    }




    #endregion

    #region Settings Tab

    private bool _settingDefaultFolderDirty = true;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SettingDefaultFolderDirty {
      get { return _settingDefaultFolderDirty; }
      set {
        _settingDefaultFolderDirty = value;
        btnSaveDefaultFolder.Visible = value;
        btnCancelAppDefaultF.Visible = value;
      }
    }
    private void edAppDefaultFolder_TextChanged(object sender, EventArgs e) {
      if (this.InvokeRequired) {
        this.Invoke(new Action(() => edAppDefaultFolder_TextChanged(sender, e)));
      } else {
        SettingDefaultFolderDirty = true;
      }
    }

    private void btnSaveDefaultFolder_Click(object sender, EventArgs e) {
      if (this.InvokeRequired) {
        this.Invoke(new Action(() => btnSaveDefaultFolder_Click(sender, e)));
      } else {
        var setting = _settings[Cx.ApsDefaultFolder];
        if (setting == null) {
          setting = new AppSetting { Key = Cx.ApsDefaultFolder, Value = edAppDefaultFolder.Text };
          _settings[Cx.ApsDefaultFolder] = setting;
        } else {
          setting.Value = edAppDefaultFolder.Text;
          _settings[Cx.ApsDefaultFolder] = setting;
        }
        SettingDefaultFolderDirty = false;
      }
    }

    private void btnCancelAppDefaultF_Click(object sender, EventArgs e) {
      if (this.InvokeRequired) {
        this.Invoke(new Action(() => btnCancelAppDefaultF_Click(sender, e)));
      } else {
        var aDefaultFolder = _settings[Cx.ApsDefaultFolder];
        if (aDefaultFolder != null && !string.IsNullOrEmpty(aDefaultFolder.Value)) {
          edAppDefaultFolder.Text = aDefaultFolder.Value;
        } else {
          edAppDefaultFolder.Text = WeaverExt.AppProjectsPath;
        }
        SettingDefaultFolderDirty = false;
      }
    }

    private void btnAppDefaultFolderBrowse_Click(object sender, EventArgs e) {
      var defFolder = edAppDefaultFolder.Text;
      using (var fbd = new FolderBrowserDialog()) {
        fbd.Description = "Select default folder for projects and files";
        fbd.SelectedPath = Directory.Exists(defFolder) ? defFolder : WeaverExt.AppProjectsPath;
        if (fbd.ShowDialog() == DialogResult.OK) {
          edAppDefaultFolder.Text = fbd.SelectedPath;
        }
      }
    }


    private bool _showPkgInLib = false;
    private void cbShowPkgInLib_CheckedChanged(object sender, EventArgs e) {
      _showPkgInLib = cbShowPkgInLib.Checked;

    }

    private bool _showSessions = false;
    private void cbShowSessions_CheckedChanged(object sender, EventArgs e) {
      _showSessions = cbShowSessions.Checked;
    }


    private async void btnImportOrgDocs_Click(object sender, EventArgs e) {
      using (var fbd = new ImportOrgDocsDialog()) {
        var orgItemId = _sessionDetails?.OrganizationId ?? 0;
        if (orgItemId == 0) { return; } // no session?
        if (_itemCache.TryGetValue(orgItemId, out var orgItem)) {
          if (orgItem != null && orgItem.Item != null) {
            var rootFolder = orgItem.Item.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder)?.Value ?? "";
            fbd.OrgRootFileName = Path.Combine(rootFolder, Cx.AppOrgExport); ;
          }
        }
        List<string> deskList = new List<string>();
        if (fbd.ShowDialog() == DialogResult.OK) {
          var resultList = fbd.FinalRelToFullPathDict;
          foreach (var key in resultList.Keys) {
            string relPath = key;
            string fullPath = resultList[key];

            string fileExt = Path.GetExtension(fullPath).ToLower();
            bool isOrgDoc = fileExt == ".md";
            bool isDigitalOperator = fileExt == ".json" && fullPath.Contains(Cx.AppTeamFolder);
            bool isDesk = fileExt == ".json" && fullPath.Contains(Cx.AppWorkGroupFolder);
            bool isRole = fileExt == ".json" && fullPath.Contains(Cx.AppDeskRolesFolder);

            if (isOrgDoc || isDigitalOperator || isRole || isDesk) {
              var result = await _appDataService.ImportOrgDoc(fullPath, relPath, false);
              if (!result.IsSuccess) {
                DoLogMessage($"Failed to import {relPath} - error: {result.Message}");
              }
              if (isDesk) {
                deskList.Add(relPath);
              }
            }
          }

          foreach (var deskRelPath in deskList) {  // second time for properties that refer to other desks
            string fullPath = resultList[deskRelPath];
            var result = await _appDataService.ImportOrgDoc(fullPath, deskRelPath, false);
            if (!result.IsSuccess) {
              DoLogMessage($"Failed to import desk {deskRelPath} - error: {result.Message}");
            }
          }

          miReloadTree_Click(sender, e);
        }


      }
    }


    private void button1_Click(object sender, EventArgs e) { // temp button to test dialog.
      var itemType = _selectedNode?.Item?.ItemTypeId ?? 0;
      if (itemType != 0) {

        using var dlg = new GetNewItemDetailsDialog(_serviceScopeFactory, (WeItemType)itemType);
        if (dlg.ShowDialog() == DialogResult.OK) {
          var newItemName = dlg.ItemName;
          MessageBox.Show("You entered: " + newItemName, "Item Name", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
      }
    }

    #endregion

    #region On Generate Context Menu and btn clicks
    private async void miGenerate_Click(object sender, EventArgs e) {
      if (_selectedNode == null || _selectedNode.Item == null) {
        MessageBox.Show("Please select an item to generate.", "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }
      if (this.InvokeRequired) {
        this.Invoke(new Action(() => miGenerate_Click(sender, e)));
        return;
      }
      List<string> log = new List<string>();
      try {
        log = await _selectedNode.Regenerate(log);
      } catch (Exception ex) {
        DoLogMessage("Failed to generate - error:" + ex.Message);
        MessageBox.Show($"Error during generation: {ex.Message}", "Generation Failed");
      } finally {
        DoLogMessage("Generation log:");
        foreach (var logEntry in log) {
          DoLogMessage(logEntry);
        }
      }
    }

    private async void btnGenerateDesc_Click(object sender, EventArgs e) {
      if (_selectedNode == null || _selectedNode.Item == null) {
        MessageBox.Show("Please select an item to generate description for.", "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }
      try {
        TreeViewEventArgs ee = new TreeViewEventArgs(_selectedNode);
        var item = _selectedNode.Item;
        WeItemType itemType = (WeItemType)item.ItemTypeId;
        if (_generatableTypes.Contains(itemType)) {
          switch (itemType) {
            case WeItemType.SolutionModel:
              var code = await _itemTemplateService.GetSolutionTemplateCommand(item.Id);
              if (code != null) {
                item.Description = code;
                await _appDataService.UpdateItemAsync(item);
              }
              break;
            case WeItemType.LibraryModel:
              var libCode = await _itemTemplateService.GetLibraryTemplate(item.Id);
              if (libCode != null) {
                item.Description = libCode;
                await _appDataService.UpdateItemAsync(item);
              }
              break;
            case WeItemType.DependencyInjectionModel:
              var diCode = await _itemTemplateService.GetDependencyInjectionTemplate(item.Id);
              if (diCode != null) {
                item.Description = diCode;
                await _appDataService.UpdateItemAsync(item);
              }
              break;
            case WeItemType.DbContextModel:
              var dbContextCode = await _itemTemplateService.GetDbContextTemplate(item.Id);
              if (dbContextCode != null) {
                item.Description = dbContextCode;
                await _appDataService.UpdateItemAsync(item);
              }
              break;
            case WeItemType.ClassModel:
              var classCode = await _itemTemplateService.GetClassTemplate(item.Id);
              if (classCode != null) {
                item.Description = classCode;
                await _appDataService.UpdateItemAsync(item);
              }
              break;
            case WeItemType.EntityClassModel:
              var entityCode = await _itemTemplateService.GetEntityClassTemplate(item.Id);
              if (entityCode != null) {
                item.Description = entityCode;
                await _appDataService.UpdateItemAsync(item);
              }
              break;
            case WeItemType.EntityConfigurationModel:
              var entityConfigCode = await _itemTemplateService.GetEntityClassConfigTemplate(item.Id);
              if (entityConfigCode != null) {
                item.Description = entityConfigCode;
                await _appDataService.UpdateItemAsync(item);
              }
              break;
            default:
              break;
          }
        }
        tvKb_AfterSelect(sender, ee);

      } catch (Exception ex) {
        DoLogMessage("Failed to generate description - error:" + ex.Message);
        MessageBox.Show($"Error during description generation: {ex.Message}", "Generation Failed");
      }
    }

    private async void btnWriteFile_Click(object sender, EventArgs e) {
      if (_selectedNode == null || _selectedNode.Item == null) {
        MessageBox.Show("Please select a item to write.", "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }
      if (_selectedNode.Item.ItemTypeId == (int)WeItemType.LibraryModel) {
        var result = await _appDataService.WriteLibrary(_selectedNode.Item.Id, true);

        foreach (var errorLine in result.Errors) {
          DoLogMessage("Error: " + errorLine);
        }
        foreach (var warnLine in result.Warnings) {
          DoLogMessage("Warning: " + warnLine);
        }
        DoLogMessage($"Written: {result.FilesWritten}");
        DoLogMessage($"Skipped: {result.FilesSkipped}");
        DoLogMessage($"Removed: {result.FilesRemoved}");

      }
      if (_selectedNode.Item.ItemTypeId == (int)WeItemType.SolutionModel) {
        var result = await _appDataService.WriteSolution(_selectedNode.Item.Id, true);

        foreach (var errorLine in result.Errors) {
          DoLogMessage("Error: " + errorLine);
        }
        foreach (var warnLine in result.Warnings) {
          DoLogMessage("Warning: " + warnLine);
        }
        DoLogMessage($"Written: {result.FilesWritten}");
        DoLogMessage($"Skipped: {result.FilesSkipped}");
        DoLogMessage($"Removed: {result.FilesRemoved}");
        DoLogMessage($"Shell Output: {result.ShellOutput}");
        DoLogMessage($"Success: {result.Success}");
      }
      if (_selectedNode.Item.ItemTypeId == (int)WeItemType.OrganizationModel) {
        var result = await _appDataService.WriteOrganization(_selectedNode.Item.Id, true);
        foreach (var errorLine in result.Errors) {
          DoLogMessage("Error: " + errorLine);
        }
        foreach (var warnLine in result.Warnings) {
          DoLogMessage("Warning: " + warnLine);
        }
        DoLogMessage($"Written: {result.FilesWritten}");
        DoLogMessage($"Skipped: {result.FilesSkipped}");
        DoLogMessage($"Removed: {result.FilesRemoved}");
      }
      if (_selectedNode.Item.ItemTypeId.IsContentType()) {
        var result = await _appDataService.WriteDocument(_selectedNode.Item.Id);
        DoLogMessage($"Success: {result.Success} {result.Message}");
      }

    }


    private async void btnAttemptTodo_Click(object sender, EventArgs e) {
      var item = _selectedNode?.Item;
      if (item != null && item.ItemTypeId == (int)WeItemType.TodoModel) {
        var result = await _appDataService.RunTodoItem(item.Id, true);
        var pad = new PreviewAttemptDialog();
        pad.Operator = $"{result.Operator} (Id: {result.OperatorId})";
        pad.SystemPrompt = result.SystemPrompt;
        pad.UserPrompt = result.UserPrompt;
        pad.Harness = $"{result.HarnessName} (Id:{result.HarnessId})";
        if (pad.ShowDialog() == DialogResult.OK) {
          if (result.HarnessId != _sessionDetails!.HarnessId) {
            MessageBox.Show("The harness for this todo is not accessible from the current session harness.", "Harness Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DoLogMessage($"{DateTime.Now}: Aborted Attempting TodoId {item.Id}");
            return;
          }
          DoLogMessage($"{DateTime.Now}: {result.Operator} Attempting TodoId {item.Id}");
          var attemptResult = await _appDataService.RunTodoItem(item.Id, false);
          if (attemptResult.Status == RunTodoAttemptOutcome.SuccessWithResponse) {
            DoLogMessage($"TodoId {item.Id} Attempt Successful, Response: {attemptResult.ResponseText}");
            await LoadRootProjects();
          } else {
            DoLogMessage($"TodoId {item.Id} Attempt Failed, Status: {attemptResult.Status}, Error: {attemptResult.ErrorMessage}");
          }
        }
      }
    }


    #endregion
    #region Ready tab


    private readonly ConcurrentDictionary<int, ReadyTodoRow> _notReadyDict = new();

    private async void ReloadReadyTab() {
      await ReloadReadyTabAsync();
    }

    private async Task<bool> ReloadReadyTabAsync() {
      int lbNotReadyIndex = lbNotReady.SelectedIndex;
      if (lbNotReadyIndex == -1) {
        lbNotReadyIndex = 0;
      }
      var notReady = await _appDataService.GetTodoByStatusReady(_sessionDetails!.HarnessId, WeItemType.TodoNotStarted, false);
      RunOnUi(() => {
        edReadyTodoName.Text = "";
        edReadyRefItem.Text = "";
        edReadyPrompt.Text = "";
        btnUpdateReady.Visible = false;
        btnAbortReadUpdate.Visible = false;
        cbSetReadyfromReview.Checked = false;
        cbDeleteNotReady.Checked = false;
        lbNotReady.Items.Clear();
        _notReadyDict.Clear();
        foreach (var todo in notReady) {
          var todoStr = $"{todo.Id}: on " + todo.DeskName + " " + todo.Name;
          var index = lbNotReady.Items.Add(todoStr);
          _notReadyDict[index] = todo;
        }

        if (lbNotReady.Items.Count > 0) {
          lbNotReady.SelectedIndex = Math.Min(lbNotReadyIndex, lbNotReady.Items.Count - 1);
        }
      });
      return true;
    }

    private async void lbNotReady_SelectedIndexChanged(object sender, EventArgs e) {
      edReadyTodoName.Text = "";
      edReadyRefItem.Text = "";
      edReadyPrompt.Text = "";
      btnUpdateReady.Visible = false;
      btnAbortReadUpdate.Visible = false;
      cbSetReadyfromReview.Checked = false;
      cbDeleteNotReady.Checked = false;

      var selectedIndex = lbNotReady.SelectedIndex;
      if (selectedIndex >= 0 && _notReadyDict.TryGetValue(selectedIndex, out var todo)) {
        edReadyTodoName.Text = todo.Id.ToString() + ": " + todo.Name + " on " + todo.DeskName;
        var item = await _appDataService.GetItemById(todo.Id);
        if (item != null) {
          var refId = item.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem)?.Value ?? "0";
          if (refId != null && refId != "0" && int.TryParse(refId, out var refIdInt)) {
            var refItem = await _appDataService.GetItemById(refIdInt);
            if (refItem != null) {
              edReadyRefItem.Text = $"RefId: {refItem.Id}, {refItem.Name}";
            }
          }

          var preview = await _appDataService.RunTodoItem(item.Id, true);
          if (preview != null) {
            string previewText = $"System Prompt: {preview.SystemPrompt}{Environment.NewLine}User Prompt: {preview.UserPrompt}{Environment.NewLine}Harness: {preview.HarnessName} (Id:{preview.HarnessId})";
            edReadyPrompt.Text = previewText;
          }
        }

      }
    }

    private void cbSetReadyfromReview_CheckedChanged(object sender, EventArgs e) {
      btnUpdateReady.Visible = cbSetReadyfromReview.Checked || cbDeleteNotReady.Checked;
      btnAbortReadUpdate.Visible = cbSetReadyfromReview.Checked || cbDeleteNotReady.Checked;
    }

    private void btnAbortReadUpdate_Click(object sender, EventArgs e) {
      btnUpdateReady.Visible = false;
      btnAbortReadUpdate.Visible = false;
      cbSetReadyfromReview.Checked = false;
      cbDeleteNotReady.Checked = false;
    }

    private async void btnUpdateReady_Click(object sender, EventArgs e) {
      btnUpdateReady.Visible = false;
      btnAbortReadUpdate.Visible = false;
      var isSetReady = cbSetReadyfromReview.Checked;
      var isDelete = cbDeleteNotReady.Checked;
      cbSetReadyfromReview.Checked = false;
      cbDeleteNotReady.Checked = false;

      var selectedIndex = lbNotReady.SelectedIndex;
      if (selectedIndex >= 0 && _notReadyDict.TryGetValue(selectedIndex, out var todo)) {
        var item = await _appDataService.GetItemById(todo.Id);
        if (item != null) {
          if (isSetReady) {
            var readyProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItConfirmedReady);
            if (readyProp != null) {
              readyProp.Value = "1";
              await _appDataService.AddUpdateItemPropertyAsync(readyProp);

            }
          }
          if (isDelete) {
            await _appDataService.DeleteItemAsync(item.Id);
          }
          await ReloadReadyTabAsync();
        }
      }

    }

    #endregion
    #region Schedule Tab

    private readonly ConcurrentDictionary<int, ReadyTodoRow> _readyDict = new();
    private async void ReloadSchedules() {
      await ReloadSchedulesAsync();
    }
    private void cbHarness_SelectedIndexChanged(object sender, EventArgs e) {
      if (_cbHarnessLoading) {
        return;
      }
      ReloadSchedules();
    }

    bool _cbHarnessLoading = false;
    int _currentHarnessId = 0;
    private async Task<bool> ReloadSchedulesAsync() {
      int lbReadyIndex = lbReady.SelectedIndex;
      if (lbReadyIndex == -1) {
        lbReadyIndex = 0;
      }
      await ReloadCbHarnessAsync();
      var selectedHarness = cbHarness.SelectedItem as ItemLookup;
      var selectedHarnessId = Convert.ToInt32(selectedHarness?.Value ?? _sessionDetails!.HarnessId);
      var schedules = await _appDataService.GetTodoByStatusReady(selectedHarnessId, WeItemType.TodoNotStarted, true);
      RunOnUi(() => {
        edWorkingName.Text = "";
        edWorkingRefItem.Text = "";
        edWorkingPrompt.Text = "";
        lbReady.Items.Clear();
        _readyDict.Clear();
        foreach (var schedule in schedules) {
          var todoStr = $"{schedule.Id}: on " + schedule.DeskName + " " + schedule.Name;
          var index = lbReady.Items.Add(todoStr);
          _readyDict[index] = schedule;
        }
        if (lbReady.Items.Count > 0) {
          lbReady.SelectedIndex = Math.Min(lbReadyIndex, lbReady.Items.Count - 1);
        }
      });
      return true;
    }

    private async Task<bool> ReloadCbHarnessAsync() {
      _cbHarnessLoading = true;
      var selectedHarness = cbHarness.SelectedItem as ItemLookup;
      _currentHarnessId = Convert.ToInt32(selectedHarness?.Value ?? _sessionDetails!.HarnessId);

      var orgItem = await _appDataService.GetItemById(_sessionDetails!.OrganizationId);
      var harnesses = orgItem!.Relations
        .Where(r => r.RelatedItemTypeId == (int)WeItemType.HarnessAppModel && r.RelatedItemId.HasValue)
        .Select(r => new ItemLookup(r!.RelatedItemId!.Value, r.RelatedItemName, r.RelatedItemName)).ToList();

      RunOnUi(() => {
        cbHarness.Items.Clear();
        int selIndx = 0;
        foreach (var harness in harnesses) {
          var index = cbHarness.Items.Add(harness);
          if (Convert.ToInt32(harness.Value) == _currentHarnessId) {
            selIndx = index;
          }
        }
        if (cbHarness.Items.Count > 0) {
          cbHarness.SelectedIndex = selIndx;
          btnStartStop.Visible = _currentHarnessId == _sessionDetails?.HarnessId;
        }
      });

      _cbHarnessLoading = false;
      return true;
    }

    private async void tRun_Tick(object sender, EventArgs e) {
      tRun.Enabled = false;
      bool isError = false;
      bool result = false;
      try {
        result = await DoNextScheduledTodo();
      } catch {
        isError = true;
      } finally {

        if (isError || !result || _isStopping || !_engineRunning) {
          _isStopping = true;
          EngineRunning = false;
        } else {
          tRun.Enabled = true;
        }

      }
    }

    private bool _isRunning = false;
    private ReadyTodoRow? _workingTodo = null;
    private async Task<bool> DoNextScheduledTodo() {
      if (_isRunning) {
        return false;
      }
      _isRunning = true;
      bool shouldContinue = false;
      try {
        if (_workingTodo == null) {
          await ReloadSchedulesAsync();
          if (_readyDict.Count == 0) {
            return false;
          }
          if (_readyDict.TryGetValue(0, out var nextTodo)) {
            _workingTodo = nextTodo;
          }
        }
        if (_workingTodo != null) {
          DoLogMessage($"Running scheduled todo {_workingTodo.Name} on {_workingTodo.DeskName} (Id: {_workingTodo.Id})");
          RunOnUi(() => {
            lbWorkingStatus.Text = "Status: Pipeline running "+ _workingTodo.Id.ToString() + ": " + _workingTodo.Name + " on " + _workingTodo.DeskName;
          });
          var result = await _appDataService.RunTodoItem(_workingTodo.Id, false);
          if (result.Status == RunTodoAttemptOutcome.SuccessWithResponse ||
              result.Status == RunTodoAttemptOutcome.MaxAttemptsReached) {
            if (result.Status == RunTodoAttemptOutcome.SuccessWithResponse) {
              DoLogMessage($"Scheduled TodoId {_workingTodo.Id} Attempt Completed, Response: {result.ResponseText}");
            } else {
              DoLogMessage($"Scheduled TodoId {_workingTodo.Id} Failed Forward Max Attempts Reached, Response: {result.ResponseText}");
            }
            await LoadRootProjects();      // UI reload treeview control.
            await ReloadReadyTabAsync();
            _workingTodo = null;
            shouldContinue = true;
          } else if (result.Status == RunTodoAttemptOutcome.RanWithoutClose ||
            result.Status == RunTodoAttemptOutcome.InvocationFailed) {
            DoLogMessage($"Scheduled TodoId {_workingTodo.Id} Attempt did not complete. retrying next. Response: {result.ResponseText}");
            shouldContinue = true;
          } else if (result.Status == RunTodoAttemptOutcome.NotConfigured) {
            DoLogMessage($"Scheduled TodoId {_workingTodo.Id} Attempt Failed, Status: {result.Status}, Error: {result.ErrorMessage}");
            _workingTodo = null;
          }
        }
      } catch (Exception ex) {
        DoLogMessage("Error in scheduled todo execution: " + ex.Message);
        return false;
      } finally {
        _isRunning = false;
      }
      return shouldContinue;
    }

    private void RunOnUi(Action action) {
      if (InvokeRequired) Invoke(action);
      else action();
    }

    private bool _engineRunning = false;
    private bool _isStopping = false;

    private void btnStartStop_Click(object sender, EventArgs e) {
      if (btnStartStop.Text == "Start") {
        EngineRunning = true;
        tRun.Enabled = true;
      } else {
        tRun.Enabled = false;
        EngineRunning = false;
      }
    }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EngineRunning {
      get { return _engineRunning; }
      set {
        _engineRunning = value;
        if (_engineRunning) {
          lbWorkingStatus.Text = "Status: Pipeline Running";
          btnStartStop.Text = "Stop";
          cbHarness.Enabled = false;
          _isStopping = false;
        } else {
          if (!_isStopping) {
            lbWorkingStatus.Text = "Status: Pipeline Stopping";
            btnStartStop.Text = "Waiting";
            btnStartStop.Enabled = false;
            _isStopping = true;
          } else {
            lbWorkingStatus.Text = "Status: Pipeline Stopped";
            btnStartStop.Text = "Start";
            btnStartStop.Enabled = true;
            cbHarness.Enabled = true;
            _isStopping = false;
          }
        }
      }
    }

    private async void lbReady_SelectedIndexChanged(object sender, EventArgs e) {
      edWorkingName.Text = "";
      edWorkingRefItem.Text = "";
      edWorkingPrompt.Text = "";
      cbReadyWorking.Checked = true;
      btnAbortWorking.Visible = false;
      btnUpdateWorking.Visible = false;

      var selectedIndex = lbReady.SelectedIndex;
      if (selectedIndex >= 0 && _readyDict.TryGetValue(selectedIndex, out var todo)) {
        edWorkingName.Text = todo.Id.ToString() + ": " + todo.Name + " on " + todo.DeskName;
        var item = await _appDataService.GetItemById(todo.Id);
        if (item != null) {
          var refId = item.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem)?.Value ?? "0";
          if (refId != null && refId != "0" && int.TryParse(refId, out var refIdInt)) {
            var refItem = await _appDataService.GetItemById(refIdInt);
            if (refItem != null) {
              edWorkingRefItem.Text = $"RefId: {refItem.Id}, {refItem.Name}";
            }
          }

          var preview = await _appDataService.RunTodoItem(item.Id, true);
          if (preview != null) {
            string previewText = $"System Prompt: {preview.SystemPrompt}{Environment.NewLine}User Prompt: {preview.UserPrompt}{Environment.NewLine}Harness: {preview.HarnessName} (Id:{preview.HarnessId})";
            edWorkingPrompt.Text = previewText;
          }
        }

      }
    }

    private void cbReadyWorking_CheckedChanged(object sender, EventArgs e) {
      btnAbortWorking.Visible = !cbReadyWorking.Checked;
      btnUpdateWorking.Visible = !cbReadyWorking.Checked;
    }

    private void btnAbortWorking_Click(object sender, EventArgs e) {
      btnAbortWorking.Visible = false;
      btnUpdateWorking.Visible = false;
      cbReadyWorking.Checked = true;
    }

    private async void btnUpdateWorking_Click(object sender, EventArgs e) {
      btnAbortWorking.Visible = false;
      btnUpdateWorking.Visible = false;
      cbReadyWorking.Checked = true;

      var selectedIndex = lbReady.SelectedIndex;
      if (selectedIndex >= 0 && _readyDict.TryGetValue(selectedIndex, out var todo)) {
        var item = await _appDataService.GetItemById(todo.Id);
        if (item != null) {
          var readyProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItConfirmedReady);
          if (readyProp != null) {
            readyProp.Value = "0";  // update is set it back to not ready. 
            await _appDataService.AddUpdateItemPropertyAsync(readyProp);
            await ReloadSchedulesAsync();
          }
        }

      }


    }


    #endregion
    #region Results tab
    private async void ReloadResultsTab() {
      await ReloadResultsTabAsync();
    }

    private readonly ConcurrentDictionary<int, ReadyTodoRow> _ResultsDict = new();
    private async void cbTodoResultType_SelectedIndexChanged(object sender, EventArgs e) {
      await ReloadResultsTabAsync();
    }
    private async Task<bool> ReloadResultsTabAsync() {
      if (cbTodoResultType.SelectedIndex == -1) {
        cbTodoResultType.SelectedIndex = 0;
      }

      int lbResultIndex = lbTodoResults.SelectedIndex;
      if (lbResultIndex == -1) {
        lbResultIndex = 0;
      }
      var statusFilter = WeItemType.TodoCompleteForward;
      switch (cbTodoResultType.SelectedIndex) {
        case 0:
          statusFilter = WeItemType.TodoCompleteForward;
          break;
        case 1:
          statusFilter = WeItemType.TodoAbortedPushBack;
          break;
        case 2:
          statusFilter = WeItemType.TodoFailedForward;
          break;
      }

      var completed = await _appDataService.GetTodoByStatusReady(_sessionDetails!.HarnessId, statusFilter, true);
      RunOnUi(() => {
        btnUpdateResultStatus.Visible = false;
        btnCancelUpdateResultStatus.Visible = false;
        edResultTodoDetails.Text = "";
        lbTodoResults.Items.Clear();
        _ResultsDict.Clear();
        foreach (var todo in completed) {
          var todoStr = $"{todo.Id}: on " + todo.DeskName + " " + todo.Name;
          var index = lbTodoResults.Items.Add(todoStr);
          _ResultsDict[index] = todo;
        }

        if (lbTodoResults.Items.Count > 0) {
          lbTodoResults.SelectedIndex = Math.Min(lbResultIndex, lbTodoResults.Items.Count - 1);
        }
      });
      return true;
    }

    private async void lbTodoResults_SelectedIndexChanged(object sender, EventArgs e) {
      btnUpdateResultStatus.Visible = false;
      btnCancelUpdateResultStatus.Visible = false;
      cbArchiveResult.Checked = false;
      cbDeleteResult.Checked = false;
      edResultTodoDetails.Text = "";
      var sb = new StringBuilder();
      var selectedIndex = lbTodoResults.SelectedIndex;
      if (selectedIndex >= 0 && _ResultsDict.TryGetValue(selectedIndex, out var todo)) {
        sb.AppendLine(todo.Id.ToString() + ": " + todo.Name + " on " + todo.DeskName);
        var item = await _appDataService.GetItemById(todo.Id);
        if (item != null) {
          var refId = item.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem)?.Value ?? "0";
          if (refId != null && refId != "0" && int.TryParse(refId, out var refIdInt)) {
            var refItem = await _appDataService.GetItemById(refIdInt);
            if (refItem != null) {
              sb.AppendLine($"RefId: {refItem.Id}, {refItem.Name}");
            }
          }

          int lastId = 0;
          string lastTodoName = "";
          foreach (var relation in item.Relations.Where(r => r.RelatedItemTypeId == (int)WeItemType.TodoAttemptModel)) {
            lastId = relation.RelatedItemId ?? 0;
            lastTodoName = relation.RelatedItemName;
          }
          if (lastId != 0) {
            var relatedItem = await _appDataService.GetItemById(lastId);
            if (relatedItem != null) {

              var continueTodo = relatedItem.Properties.FirstOrDefault(p => p.Name == Cx.ItContinueTodo);
              if (continueTodo != null) {
                sb.AppendLine("Continued to TodoId:" + continueTodo.Value);
              }

              var response = relatedItem.Properties.FirstOrDefault(p => p.Name == Cx.ItResponse);
              if (response != null) {
                sb.AppendLine("Response: " + response.Value);
              }

              var userPrompt = relatedItem.Properties.FirstOrDefault(p => p.Name == Cx.ItUserPrompt);
              if (userPrompt != null) {
                sb.AppendLine("User Prompt: " + userPrompt.Value);
              }

              var systemPrompt = relatedItem.Properties.FirstOrDefault(p => p.Name == Cx.ItSystemPrompt);
              if (systemPrompt != null) {
                sb.AppendLine("System Prompt: " + systemPrompt.Value);
              }

            }
          }
          edResultTodoDetails.Text = sb.ToString();
        }

      }
    }


    private void cbArchiveResult_CheckedChanged(object sender, EventArgs e) {
      if (cbDeleteResult.Checked) {
        cbDeleteResult.Checked = false;
      }
      btnUpdateResultStatus.Visible = cbArchiveResult.Checked;
      btnCancelUpdateResultStatus.Visible = cbArchiveResult.Checked;
    }

    private void cbDeleteResult_CheckedChanged(object sender, EventArgs e) {
      if (cbArchiveResult.Checked) {
        cbArchiveResult.Checked = false;
      }
      btnUpdateResultStatus.Visible = cbDeleteResult.Checked;
      btnCancelUpdateResultStatus.Visible = cbDeleteResult.Checked;
    }

    private async void btnUpdateResultStatus_Click(object sender, EventArgs e) {
      btnCancelUpdateResultStatus.Visible = false;
      btnUpdateResultStatus.Visible = false;
      bool isArchive = cbArchiveResult.Checked;
      bool isDelete = cbDeleteResult.Checked;
      cbArchiveResult.Checked = false;
      cbDeleteResult.Checked = false;

      var selectedIndex = lbTodoResults.SelectedIndex;
      if (selectedIndex >= 0 && _ResultsDict.TryGetValue(selectedIndex, out var todo)) {
        if (isArchive) {
          var item = await _appDataService.GetItemById(todo.Id);
          if (item != null) {
            item.IsActive = false;
            await _appDataService.UpdateItemAsync(item);
          }
        }
        if (isDelete) {
          await _appDataService.DeleteItemAsync(todo.Id);
        }
        await ReloadResultsTabAsync();
      }
    }

    private void btnCancelUpdateResultStatus_Click(object sender, EventArgs e) {
      btnCancelUpdateResultStatus.Visible = false;
      btnUpdateResultStatus.Visible = false;
      cbArchiveResult.Checked = false;
      cbDeleteResult.Checked = false;
    }

    #endregion


    private void lbClaudeLaunch_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      // Open the folder containing the Claude executable settings.  Claude should be configured in your path. 
      try {
        var claudePath = WeaverExt.ClaudeExecutablePath;

        if (claudePath != null) {
          System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo() {
            FileName = claudePath,
            UseShellExecute = true,
            Verb = "open"
          });
        } else {
          MessageBox.Show("Could not determine the folder path for the Claude executable to start.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

      } catch (Exception ex) {
        DoLogMessage(ex.Message + " Error opening Claude executable folder.");
        MessageBox.Show("An error occurred while trying to open the Claude executable folder. Please check the logs for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }

}
