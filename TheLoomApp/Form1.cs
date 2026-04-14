using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.PropertyGridInternal;
using System.Windows.Media.Media3D;
using TheLoomApp.Components;
using TheLoomApp.Extensions;
using TheLoomApp.Models;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;
using Weavers.Core.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TheLoomApp {
  public partial class Form1 : Form {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IAppDataService _appDataService;
    private ItemNode? _selectedNode = null;
    private Dictionary<int, ItemNode> _itemCache = new Dictionary<int, ItemNode>();
    private Dictionary<int, ItemTypeDto> _itemTypeCache = new Dictionary<int, ItemTypeDto>();
    private IItemTypeLookupComboProvider _itemTypeLookupComboProvider;
    private Components.PropertiesTab _itemPropertiesTab;
    private IAppSettingService _settings;

    #region Initialization and Setup
    public Form1(IServiceScopeFactory serviceScopeFactory) {
      InitializeComponent();
      _serviceScopeFactory = serviceScopeFactory;
      using var scope = _serviceScopeFactory.CreateScope();
      _appDataService = scope.ServiceProvider.GetRequiredService<IAppDataService>();
      _itemTypeLookupComboProvider = scope.ServiceProvider.GetRequiredService<IItemTypeLookupComboProvider>();
      _itemPropertiesTab = new Components.PropertiesTab(_appDataService, _itemTypeLookupComboProvider);
      _settings = scope.ServiceProvider.GetRequiredService<IAppSettingService>();
      SetupForStartup();
    }

    public void SetupForStartup() {
      tabControl2.TabPages.Add(_itemPropertiesTab);
      _itemPropertiesTab.SetLabelRight(76);
      _itemPropertiesTab.OnPostEvent += ProjectTab_OnPostEvent;
      splitContainer3.Panel2Collapsed = true;
      btnCancelRelation.Visible = false;
      btnUpdateRelation.Visible = false;
      btnAbortItem.Visible = false;
      btnUpdateItem.Visible = false;
      var aDefaultFolder = _settings[Cx.ApsDefaultFolder];
      if (aDefaultFolder != null && !string.IsNullOrEmpty(aDefaultFolder.Value)) {
        edAppDefaultFolder.Text = aDefaultFolder.Value;
      } else {
        _settings[Cx.ApsDefaultFolder] = new AppSetting { Key = Cx.ApsDefaultFolder, Value = WeaverExt.AppProjectsPath };
        edAppDefaultFolder.Text = WeaverExt.AppProjectsPath;
      }
      SettingDefaultFolderDirty = false;

    }

    private bool _inResize = false;
    private void Form1_Resize(object sender, EventArgs e) {
      if (_inResize) return;
      _inResize = true;
      var horizonalSpace = splitContainer1.Panel2.Width - splitContainer1.SplitterWidth;
      var verticalSpace = splitContainer1.Panel2.Height - (splitContainer3.Panel2Collapsed ? 0 : splitContainer3.Panel2.Height + 4);
      btnArchive.Left = horizonalSpace - btnArchive.Width - 10;
      btnAbortItem.Left = horizonalSpace - btnAbortItem.Width - 10;
      btnUpdateItem.Left = btnAbortItem.Left - btnUpdateItem.Width - 4;
      edItemName.Width = horizonalSpace - edItemName.Left - 10;
      edItemType.Width = horizonalSpace - edItemType.Left - 14 - (btnUpdateItem.Width * 2);

      tabControl2.Height = verticalSpace - (lbType.Top + (lbType.Height * 4));
      _inResize = false;
    }

    #endregion

    #region Tree View Loading and Events
    private async void Form1_Shown(object sender, EventArgs e) {
      await LoadRootProjects();
      await LoadItemTypesCache();
      Form1_Resize(sender, e);
    }

    private async Task LoadRootProjects(CancellationToken ct = default) {

      List<int> expandedNodeIds = new List<int>();
      int? selectedItemId = null;

      try {

        if (tvKb.SelectedNode != null) {
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
          var items = await _appDataService.GetRootProjectsAsync();
          foreach (var project in items) {
            ItemNode projectNode = project.ToItemNode();
            _itemCache[project.Id] = projectNode;
            if (project.Relations.Count() > 0) {
              foreach (var rel in project.Relations) {
                if (rel.RelatedItemId.HasValue) {

                  if (expandedNodeIds.Contains(rel.RelatedItemId.Value)) {
                    await AddNodeById(expandedNodeIds, projectNode, rel.RelatedItemId.Value, rel);
                  } else {

                    var item = await _appDataService.GetItemById(rel.RelatedItemId.Value);
                    if (item != null) {

                      ItemNode projectsChildNode = rel.ToItemNode(item);
                      _itemCache[item.Id] = projectsChildNode;
                      projectNode.Nodes.Add(projectsChildNode);


                      if (item.Relations.Count() > 0) {
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
              node.Expand();
            }
          }
          var target = tvKb.Nodes.Find(selectedItemId.Value.ToString(), true);
          if (target.Length > 0) {
            tvKb.SelectedNode = target[0];
            tvKb.SelectedNode.EnsureVisible();
          }
        }

      } catch (Exception ex) {
        MessageBox.Show($"Error loading root projects: {ex.Message}");
      }
    }


    private void ProjectTab_OnPostEvent() {
      if (_selectedNode != null && _selectedNode.Item != null && _selectedNode.Item.Properties != null) {
        _itemPropertiesTab.ItemProps = _selectedNode.Item.Properties.ToList();
        _itemPropertiesTab.SetEditingMode(false);
        _itemPropertiesTab.SetLabelRight(Cx.intPropertyLabelLeft);
      }
    }

    private async Task<ItemNode?> AddNodeById(List<int> toLoad, ItemNode parent, int? itemId, RelationDto? relation = null) {
      if (itemId == null) return null;

      if (toLoad.Contains(itemId.Value)) {
        var bItem = await _appDataService.GetItemById(itemId.Value);
        if (bItem != null) {
          ItemNode newNode = relation == null ? bItem.ToItemNode() : relation.ToItemNode(bItem);
          _itemCache[bItem.Id] = newNode;
          parent.Nodes.Add(newNode);
          if (bItem.Relations.Count() > 0) {
            foreach (var rel in bItem.Relations) {
              var relatedItem = await AddNodeById(toLoad, newNode, rel.RelatedItemId, rel);
            }
          }
          return newNode;
        }
      } else {
        if (relation == null || !relation.RelatedItemId.HasValue) { return null; }
        var aItem = await _appDataService.GetItemById(relation.RelatedItemId.Value);
        if (aItem != null) {
          ItemNode projectsChildNode = relation.ToItemNode(aItem);
          _itemCache[aItem.Id] = projectsChildNode;
          parent.Nodes.Add(projectsChildNode);
          if (aItem.Relations.Count() > 0) {
            projectsChildNode.Nodes.Add(new ItemNode());
          }
        }
      }
      return null;
    }

    private async void tvKb_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
      if (e.Node is not ItemNode node) return;
      if (node.Nodes.Count != 1 || node.IsLoaded) return;

      e.Cancel = true;  // prevent expand until data is ready
      node.Nodes.Clear();
      if (node.Item == null) return;
      var item = await _appDataService.GetItemById(node.Item.Id);
      if (item != null) {
        foreach (var rel in item.Relations) {
          if (rel.RelatedItemId.HasValue) {
            var itemChild = await _appDataService.GetItemById(rel.RelatedItemId.Value);
            if (itemChild != null) {
              ItemNode itemsChildNode = rel.ToItemNode(itemChild);
              _itemCache[itemChild.Id] = itemsChildNode;
              node.Nodes.Add(itemsChildNode);
              if (itemChild.Relations.Count() > 0) {
                itemsChildNode.Nodes.Add(new ItemNode());
              }
            }

          }
        }
        node.Expand();
      }
    }

    private void tvKb_AfterCollapse(object sender, TreeViewEventArgs e) {
      // if (e.Node is ItemNode node) {
      //   if (node.Nodes.Count > 0) {
      //     foreach (ItemNode child in node.Nodes) {
      //       child.Nodes.Clear();
      //       child.Nodes.Add(new ItemNode());
      //     }
      //   }
      // }
    }

    private async Task LoadItemTypesCache() {
      try {
        _inSetupTpItems = true;
        var itemTypes = await _appDataService.GetAllItemTypesAsync();
        _itemTypeCache = itemTypes.ToDictionary(t => t.Id, t => t);

        edItemType.DataSource = _itemTypeCache.Values.ToList();
        edItemType.DisplayMember = "Name";
        edItemType.ValueMember = "Id";

      } catch (Exception ex) {
        MessageBox.Show($"An error occurred while loading project types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      try {
        var RelationTypes = await _appDataService.GetRelationTypesAsync();
        cbRelRelation.DataSource = RelationTypes;
        cbRelRelation.DisplayMember = "Name";
        cbRelRelation.ValueMember = "Id";
      } catch (Exception ex) {
        MessageBox.Show($"An error occurred while loading relation types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      _inSetupTpItems = false;

    }

    #endregion

    #region Tree View Selection tracking


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

    private bool _ItemTabDirty = false;

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

    private bool _RelationTabDirty = false;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool RelationTabDirty {
      get { return _RelationTabDirty; }
      set {
        _RelationTabDirty = value;
        if (!_inSetupTpRelations) {
          btnCancelRelation.Visible = value;
          btnUpdateRelation.Visible = value;
        }
      }
    }


    private ItemDto? _CurrentItemBackup = null;
    private RelationDto? _CurrentRelationBackup = null;
    private bool _inSetupTpItems = false;
    private bool _inSetupTpRelations = false;
    private void SetupTpItems() {
      if (_selectedNode != null && _selectedNode.Item != null) {
        _inSetupTpItems = true;
        var item = _selectedNode.Item;

        _CurrentItemBackup = _selectedNode.Item.Clone();
        lbItemId.Text = "ItemId: " + _selectedNode.Item.Id.ToString();
        edItemType.DataBindings.Clear();
        edItemType.DataBindings.Add("SelectedValue", _selectedNode.Item, "ItemTypeId", true, DataSourceUpdateMode.OnPropertyChanged);
        edItemName.DataBindings.Clear();
        edItemName.DataBindings.Add("Text", _selectedNode.Item, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
        edItemDesc.DataBindings.Clear();
        edItemDesc.DataBindings.Add("Text", _selectedNode.Item, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
        edItemData.DataBindings.Clear();
        edItemData.DataBindings.Add("Text", _selectedNode.Item, "Data", true, DataSourceUpdateMode.OnPropertyChanged);

        _inSetupTpItems = false;
        ItemTabDirty = false;
      }
    }

    private void SetupTpRelations() {
      _inSetupTpRelations = true;
      if (_selectedNode != null && _selectedNode.Relation != null) {

        _CurrentRelationBackup = _selectedNode.Relation.Clone();
        lbRelationId.Text = "RelationId: " + _selectedNode.Relation.Id.ToString();

        var rank = _selectedNode.Relation.Rank.HasValue ? _selectedNode.Relation.Rank.Value : 0;
        edRank.Value = rank;

        lbRelItemName.DataBindings.Clear();
        lbRelItemName.DataBindings.Add("Text", _selectedNode.Relation, "ItemName", true, DataSourceUpdateMode.OnPropertyChanged);

        if (!cbRelRelation.Enabled) cbRelRelation.Enabled = true;
        cbRelRelation.DataBindings.Clear();
        cbRelRelation.DataBindings.Add("SelectedValue", _selectedNode.Relation, "RelationTypeId", true, DataSourceUpdateMode.OnPropertyChanged);

      } else {
        lbRelationId.Text = "RelationId: N/A";
        lbRelItemName.DataBindings.Clear();

        cbRelRelation.DataBindings.Clear();
        cbRelRelation.Text = "";
        if (cbRelRelation.Enabled) cbRelRelation.Enabled = false;
      }
      _inSetupTpRelations = false;
      RelationTabDirty = false;
    }

    private void edRank_ValueChanged(object sender, EventArgs e) {
      if (!_inSetupTpRelations) {
        RelationTabDirty = true;
      }
    }
    private void cbRelRelation_SelectedValueChanged(object sender, EventArgs e) {
      if (!_inSetupTpRelations) {
        RelationTabDirty = true;
      }
    }

    private void edItemName_TextChanged(object sender, EventArgs e) {
      if (!_inSetupTpItems) {
        ItemTabDirty = true;
      }
    }


    private async void btnUpdateRelation_Click(object sender, EventArgs e) {
      if (_selectedNode != null && _selectedNode.Relation != null) {
        await _appDataService.UpdateRelationAsync(_selectedNode.Relation);
        SetupTpRelations();
      }
    }

    private void btnCancelRelation_Click(object sender, EventArgs e) {
      if (_selectedNode != null && _selectedNode.Relation != null && _CurrentRelationBackup != null) {
        _selectedNode.Relation.RelatedItemId = _CurrentRelationBackup.RelatedItemId;
        _selectedNode.Relation.Rank = _CurrentRelationBackup.Rank;
        _selectedNode.Relation.RelationTypeId = _CurrentRelationBackup.RelationTypeId;
        SetupTpRelations();
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
        _selectedNode.Text = _selectedNode.Item.Name;

        var selectedItemTypeId = _selectedNode.Item.ItemTypeId;
        if (selectedItemTypeId == (int)WeItemType.ProjectFolderModel) { 
          string newPath = Path.Combine(edAppDefaultFolder.Text, edItemName.Text );
          var rootFolderProp = _selectedNode.Item.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder);
          if (rootFolderProp != null) {
            var oldPath = rootFolderProp.Value;
            if (oldPath != null && oldPath.StartsWith(edAppDefaultFolder.Text)) {
              rootFolderProp.Value = newPath;
              await _appDataService.AddUpdateItemPropertyAsync(rootFolderProp);            
            }
          }
        } else if (selectedItemTypeId == (int)WeItemType.RelativeFolderModel) {
          var parentNode = (ItemNode?)_selectedNode.Parent;
          if (parentNode != null) {
            string newPath = parentNode.Item.ResolveParentFolderPath(edAppDefaultFolder.Text);
            await UpdateFolderPathIfNeededAsync(_selectedNode.Item, newPath);
          }
        }
        SetupPropertiesTabForItem(_selectedNode.Item);
      }
      SetupTpItems();      
    }

    private async Task UpdateFolderPathIfNeededAsync(ItemDto item, string basePath) {
      string? propKey = item.ItemTypeId switch {
        (int)WeItemType.ProjectFolderModel => Cx.ItRootFolder,
        (int)WeItemType.RelativeFolderModel => Cx.ItRelativeFolder,
        _ => null
      };
      if (propKey == null) return;

      var folderProp = item.Properties.FirstOrDefault(p => p.Name == propKey);
      if (folderProp == null) return;     

      if (!string.IsNullOrEmpty(folderProp.Value) && folderProp.Value.StartsWith(basePath)) {
        folderProp.Value = Path.Combine(basePath, item.Name.UrlSafe());
        var updated = await _appDataService.AddUpdateItemPropertyAsync(folderProp);
        if (updated != null) item.AddOrUpdateProperty(updated);
      }
    }

    private void btnAbortItem_Click(object sender, EventArgs e) {
      if (_selectedNode != null && _selectedNode.Item != null && _CurrentItemBackup != null) {
        _selectedNode.Item.Name = _CurrentItemBackup.Name;
        _selectedNode.Item.Description = _CurrentItemBackup.Description;
        _selectedNode.Item.Data = _CurrentItemBackup.Data;
        _selectedNode.Item.ItemTypeId = _CurrentItemBackup.ItemTypeId;
        SetupTpItems();
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
        miAddProjectRoot.Visible = true;
        miAddSubProject.Visible = false;
        miDeleteItem.Enabled = false;
      } else {
        var itemType = (WeItemType)_selectedNode.Item.ItemTypeId;
        miAddProjectRoot.Visible = true;
        miAddSubProject.Visible = itemType == WeItemType.ProjectFolderModel || itemType == WeItemType.RelativeFolderModel;
        miDeleteItem.Enabled = true;
      }
    }

    private async void miReloadTree_Click(object sender, EventArgs e) {
      await LoadRootProjects();
    }



    private void AddNodeToRoot(ItemNode node) {
      var idx = tvKb.Nodes.Add(node);
      tvKb.SelectedNode = tvKb.Nodes[idx];
    }

    private void AddNodeToSelected(ItemNode parentNode, ItemNode childNode) {
      var idx = parentNode.Nodes.Add(childNode);
      parentNode.Expand();
      tvKb.SelectedNode = parentNode.Nodes[idx];
    }

    private async void miAddProjectRoot_Click(object sender, EventArgs e) {
      try { 
        await AddProjectRootAsync(); 
      } catch (Exception ex) {
        DoLogMessage( "Failed to add project root - error:"+ex.Message);
        MessageBox.Show($"Error adding project: {ex.Message}", "Add Project Failed");
      }
    }

    private async Task AddProjectRootAsync() {
      var nextRank = await _appDataService.GetNextItemRank() + 1;
      var newItem = await _appDataService.CreateItemAsync(new ItemDto {
        Name = $"Project {nextRank}",
        Description = "",
        Data = "{}",
        ItemTypeId = (int)WeItemType.ProjectFolderModel
      });
      if (newItem == null) return;

      var rootFolderProperty = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        var defaultFolder = string.IsNullOrEmpty(edAppDefaultFolder.Text) 
          ? WeaverExt.AppProjectsPath : edAppDefaultFolder.Text;

        rootFolderProperty.Value = Path.Combine(defaultFolder, newItem.Name.UrlSafe());
        var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(rootFolderProperty);
        if (updatedProp != null) {
          newItem.AddOrUpdateProperty(updatedProp);
        }
      }

      AddNodeToRoot(newItem.ToItemNode());      
    }

    private async void miAddSubProject_Click(object sender, EventArgs e) {
      try { 
        await AddSubFolderAsync(); 
      } catch (Exception ex) {
        DoLogMessage("Failed to add subfolder - error:" + ex.Message);
        MessageBox.Show($"Error adding folder: {ex.Message}", "Add Folder Failed");
      }
    }

    private async Task AddSubFolderAsync() {
      var itemNode = _selectedNode;
      if (itemNode?.Item == null) return;
      ItemDto item = itemNode.Item;
      if (!item.IsValidFolderParent()) return;
      var nextRank = await _appDataService.GetNextItemRank(item.Id) + 1;
      var newSubItem = await _appDataService.CreateRelatedItemAsync(
        item.Id, 
        (int)WeRelationTypes.Contains,
        (int)WeItemType.RelativeFolderModel, 
        $"Folder {nextRank}", string.Empty, "{}");

      if (newSubItem == null) {
        // add error logging.
        return;
      }
   
      var newParentRelation = newSubItem.IncomingRelations
        .FirstOrDefault(r => r.ItemId == item.Id 
        && r.RelationTypeId == (int)WeRelationTypes.Contains);
      if (newParentRelation == null) {
        // add error logging.
        return;
      }      

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = item.ResolveParentFolderPath(edAppDefaultFolder.Text);
        var fullPath = Path.Combine(parentFolderPath, newSubItem.Name.UrlSafe());
        rootFolderProperty.Value = fullPath;
        var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(rootFolderProperty);
        if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
      }

      AddNodeToSelected(itemNode, newParentRelation.ToItemNode(newSubItem));      
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

    delegate void LogMessageDelegate(string message);
    private void DoLogMessage(string message) {
      if (this.InvokeRequired) {
        this.Invoke(new LogMessageDelegate(DoLogMessage), new object[] { message });
      } else {
        if (splitContainer3.Panel2Collapsed) {
          splitContainer3.Panel2Collapsed = false;
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

    #endregion



  }
}
