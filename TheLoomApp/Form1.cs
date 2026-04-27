using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using TheLoomApp.Components;
using TheLoomApp.Extensions;
using TheLoomApp.Models;
using Weavers.Core.Constants;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;
using Weavers.Core.Service;

namespace TheLoomApp {
  public partial class Form1 : Form {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IAppDataService _appDataService;
    private readonly IAppItemTemplateService _itemTemplateService;
    private ItemNode? _selectedNode = null;
    private Dictionary<int, ItemNode> _itemCache = new Dictionary<int, ItemNode>();
    private Dictionary<int, ItemTypeDto> _itemTypeCache = new Dictionary<int, ItemTypeDto>();
    private PropertiesTab _itemPropertiesTab;
    private IAppSettingService _settings;

    #region Initialization and Setup
    public Form1(IServiceScopeFactory serviceScopeFactory) {
      InitializeComponent();
      _serviceScopeFactory = serviceScopeFactory;
      using var scope = _serviceScopeFactory.CreateScope();
      _appDataService = scope.ServiceProvider.GetRequiredService<IAppDataService>();
      _itemTemplateService = scope.ServiceProvider.GetRequiredService<IAppItemTemplateService>();
      _itemPropertiesTab = new PropertiesTab(_serviceScopeFactory);
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

    private bool _inResize = false;
    private void Form1_Resize(object sender, EventArgs e) {
      if (_inResize) return;
      _inResize = true;
      var horizonalSpace = splitContainer1.Panel2.Width - splitContainer1.SplitterWidth;
      var verticalSpace = splitContainer1.Panel2.Height - (splitContainer3.Panel2Collapsed ? 0 : splitContainer3.Panel2.Height + 4);
      btnArchive.Left = horizonalSpace - btnArchive.Width - 10;
      btnGenerateDesc.Left = btnArchive.Left - btnGenerateDesc.Width - 4;
      btnAbortItem.Left = horizonalSpace - btnAbortItem.Width - 10;
      btnUpdateItem.Left = btnAbortItem.Left - btnUpdateItem.Width - 4;
      edItemName.Width = horizonalSpace - edItemName.Left - 10;
      edItemType.Width = horizonalSpace - edItemType.Left - 14 - (btnUpdateItem.Width * 2);

      tabControl2.Height = verticalSpace - (lbType.Top + (lbType.Height * 4));
      _inResize = false;
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

    #region Tree View Loading and Events
    private async void Form1_Shown(object sender, EventArgs e) {
      await LoadRootProjects();
      await LoadItemTypesCache();
      Form1_Resize(sender, e);
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

    private bool _isExpanding = false;
    private async void tvKb_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
      if (e.Node is not ItemNode node) return;
      if (_isExpanding) return;
      _isExpanding = true;
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
      _isExpanding = false;
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

    private HashSet<WeItemType> _generatableTypes = WeItemTypeExtensions.GetGenerativeTypes();
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
        btnGenerateDesc.Visible = _generatableTypes.Contains((WeItemType)item.ItemTypeId);
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


    private void edItemDesc2_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
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

    private async void ProjectTab_OnPostEvent() {
      if (_selectedNode != null && _selectedNode.Item != null && _selectedNode.Item.Properties != null) {
        _itemPropertiesTab.ItemProps = _selectedNode.Item.Properties.ToList();
        _itemPropertiesTab.SetEditingMode(false);
        _itemPropertiesTab.SetLabelRight(Cx.intPropertyLabelLeft);


        if (_selectedNode.Item.ItemTypeId == (int)WeItemType.ProjectFolderModel
          || _selectedNode.Item.ItemTypeId == (int)WeItemType.RelativeFolderModel
          || _selectedNode.Item.ItemTypeId == (int)WeItemType.FileModel
          || _selectedNode.Item.ItemTypeId == (int)WeItemType.LibraryModel
        ) {
          var folderProp = _selectedNode.Item.Properties.FirstOrDefault(p => p.Name == Cx.ItRootFolder || p.Name == Cx.ItRelativeFolder || p.Name == Cx.ItFilePath);
          if (folderProp != null) {
            string basePath = folderProp.Value ?? "";
            await UpdateFolderPathIfNeededAsync(_selectedNode.Item, basePath);
          }
        } else if (_selectedNode.Item.ItemTypeId == (int)WeItemType.DependencyInjectionModel) {
          var hasDbContext = _selectedNode.Item.Properties.Any(p => p.Name == Cx.ItHasDbContext && p.Value.AsBoolean());
          await _appDataService.AddRemoveDbContextToLibDi(_selectedNode.Item.Id, hasDbContext);
          if (!hasDbContext && _selectedNode.Nodes.Count > 0) {
            foreach(ItemNode node in _selectedNode.Nodes) { 
              if (node.Item != null && node.Item.ItemTypeId == (int)WeItemType.DbContextModel) {
                node.Remove();
              }
            }          
          } else if (hasDbContext && _selectedNode.Nodes.Count == 0) {
            var ee = new TreeViewCancelEventArgs(_selectedNode, false, TreeViewAction.Expand);
            tvKb_BeforeExpand(null, ee);
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

        }

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

    private readonly HashSet<WeItemType> _parentFolderTypes = WeItemTypeExtensions.GetParentFileFolderDependTypes();
    private readonly HashSet<WeItemType> _parentNamespaceTypes = WeItemTypeExtensions.GetParentNamespaceDependTypes();

    private async Task UpdateItemAsync() {
      if (_selectedNode != null && _selectedNode.Item != null) {
        await _appDataService.UpdateItemAsync(_selectedNode.Item);
        _selectedNode.Text = _selectedNode.Item.Name;

        var selectedItemTypeId = _selectedNode.Item.ItemTypeId;
        var selectedItemType = (WeItemType)selectedItemTypeId;
        if (selectedItemType == WeItemType.ProjectFolderModel) {
          string newPath = Path.Combine(edAppDefaultFolder.Text, edItemName.Text);
          await UpdateFolderPathIfNeededAsync(_selectedNode.Item, newPath);
        } else {
          var parentNode = (ItemNode?)_selectedNode.Parent;
          if (parentNode != null) {
            if (_parentFolderTypes.Contains(selectedItemType)) {
              string newPath = parentNode.Item.ResolveParentFolderPath(edAppDefaultFolder.Text);
              await UpdateFolderPathIfNeededAsync(_selectedNode.Item, newPath);
            }
            
            if (parentNode.Item != null && _parentNamespaceTypes.Contains(selectedItemType)) {
              string newNamespace = parentNode.Item.ResolveParentNamespace(_selectedNode.Item.Name); // library case previous no namespace.
              await UpdateNamespacePathIfNeededAsync(_selectedNode.Item, newNamespace);            //  name is the library though.
            }
          }
          
          if (selectedItemTypeId == (int)WeItemType.DependencyInjectionModel) {
            var hasDbContext = _selectedNode.Item.Properties.Any(p => p.Name == Cx.ItHasDbContext && p.Value.AsBoolean());
            var hasMediatR = _selectedNode.Item.Properties.Any(p => p.Name == Cx.ItHasMediator && p.Value.AsBoolean());

            var diCode = await _itemTemplateService.GetDependencyInjectionTemplate(_selectedNode.Item.Id);
            if (diCode != null) {
              _selectedNode.Item.Description = diCode;
              await _appDataService.UpdateItemAsync(_selectedNode.Item);
            }                        
          }
        }

        await Task.Delay(100);
        await LoadRootProjects();
      }

    }

    private async Task UpdateFolderPathIfNeededAsync(ItemDto item, string basePath) {
      string propKey = item.ItemTypeId.GetFolderPropertyName();
      if (propKey == "") return;

      var folderProp = item.Properties.FirstOrDefault(p => p.Name == propKey);
      if (folderProp == null) return;

      var fullPath = "";
      var fileName = "";  // namespaces are folders within a file and get add to the path the file is in.
      var fileBasePath = basePath;
      if (propKey == Cx.ItFilePath) {
        if (fileBasePath.Contains(".csproj")) {
          fileBasePath = Path.GetDirectoryName(fileBasePath) ?? basePath;
        }
        fileName = item.GetFileName();
        fullPath = Path.Combine(fileBasePath, fileName);
      } else if (item.ItemTypeId == (int)WeItemType.RelativeFolderModel) {
        fullPath = Path.Combine(basePath, item.Name.UrlSafe());
      } else {
        fullPath = basePath;
      }

      folderProp.Value = fullPath;
      var updated = await _appDataService.AddUpdateItemPropertyAsync(folderProp);
      if (updated != null) item.AddOrUpdateProperty(updated);
      await _appDataService.UpdateItemPropertyPathRecursive(item.Id, basePath, fullPath);

    }

    private async Task UpdateNamespacePathIfNeededAsync(ItemDto item, string basePath) {
      var namespaceProp = item.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespace || p.Name == Cx.ItNamespaceRoot);
      if (namespaceProp == null) return;
      var newNamespace = "";
      var folderPropKey = item.ItemTypeId.GetFolderPropertyName();
      if (folderPropKey == Cx.ItFilePath && (item.ItemTypeId != (int)WeItemType.NamespaceModel)) {
        newNamespace = basePath.NameSafe(); // files use the parents namespace.
      } else {  // vs a folder, we include items name.
        newNamespace = basePath.NameSafe() + "." + item.Name.NameSafe();
      }

      if (namespaceProp.Value != newNamespace) {
        namespaceProp.Value = newNamespace;
        await _appDataService.UpdateItemPropertyNamespaceRecursive(item.Id, basePath, newNamespace);
        await _appDataService.AddUpdateItemPropertyAsync(namespaceProp);
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
        miAddProjectRoot.Visible = itemType == WeItemType.ProjectFolderModel || itemType == WeItemType.RelativeFolderModel;
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
        miGenerate.Visible = true;
        toolStripSeparator2.Visible = true;
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
      _isExpanding = true;
      parentNode.Expand();
      _isExpanding = false;
      if (parentNode.Nodes.Count > 0 && idx >= 0 && idx < parentNode.Nodes.Count) {
        tvKb.SelectedNode = parentNode.Nodes[idx];
      }
    }

    private async void miAddProjectRoot_Click(object sender, EventArgs e) {
      try {
        await AddProjectRootAsync();
      } catch (Exception ex) {
        DoLogMessage("Failed to add project root - error:" + ex.Message);
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
      if (_selectedNode == null || itemNode?.Item == null) return;
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
        .FirstOrDefault(r => r.ItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
      if (newParentRelation == null) {
        // add error logging.
        return;
      }
      var newNode = newParentRelation.ToItemNode(newSubItem);
      AddNodeToSelected(_selectedNode, newNode);

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = item.ResolveParentFolderPath(edAppDefaultFolder.Text);
        var fullPath = Path.Combine(parentFolderPath, newSubItem.Name.UrlSafe());
        rootFolderProperty.Value = fullPath;
        var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(rootFolderProperty);
        if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
      }

      //await LoadRootProjects(newSubItem.Id);

    }

    private async void miAddSolution_Click(object sender, EventArgs e) {
      try {
        await AddSolutionAsync();
      } catch (Exception ex) {
        DoLogMessage("Failed to add solution - error: " + ex.Message);
        MessageBox.Show($"Error adding solution: {ex.Message}", "Add Solution failed");
      }
    }
    private async Task AddSolutionAsync() {
      var itemNode = _selectedNode;
      if (_selectedNode == null || itemNode?.Item == null) return;
      ItemDto item = itemNode.Item;
      if (!item.IsValidFolderParent()) return;
      var nextRank = await _appDataService.GetNextItemRank(item.Id) + 1;
      var newSubItem = await _appDataService.CreateRelatedItemAsync(
        item.Id,
        (int)WeRelationTypes.Contains,
        (int)WeItemType.SolutionModel,
        $"{item.Name}{nextRank}",
        string.Empty,
        "{}");

      if (newSubItem == null) {
        // add error logging.
        return;
      }

      var newParentRelation = newSubItem.IncomingRelations
        .FirstOrDefault(r => r.ItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
      if (newParentRelation == null) {
        // add error logging.
        return;
      }

      var newNode = newParentRelation.ToItemNode(newSubItem);
      AddNodeToSelected(_selectedNode, newNode);

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = item.ResolveParentFolderPath(edAppDefaultFolder.Text);
        var fileName = newSubItem.Name.UrlSafe() + ".sln";
        var fullPath = Path.Combine(parentFolderPath, fileName);
        rootFolderProperty.Value = fullPath;
        var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(rootFolderProperty);
        if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
      }

      var slnGuidProp = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItSolutionGuid);
      if (slnGuidProp != null) {
        string? val = slnGuidProp?.Value ?? "";
        if (slnGuidProp != null && string.IsNullOrEmpty(val)) {
          slnGuidProp.Value = Guid.NewGuid().ToString("B").ToUpper();
          var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(slnGuidProp);
          if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
        }
      }

      //await LoadRootProjects(newSubItem.Id);

    }
    private async void miAddSolutionImport_Click(object sender, EventArgs e) {
      try {
        await AddSolutionImportAsync();
      } catch (Exception ex) {
        DoLogMessage("Failed to add solution import - error: " + ex.Message);
        MessageBox.Show($"Error adding solution import: {ex.Message}", "Add Solution failed");
      }
    }

    private async Task AddSolutionImportAsync() {
      var itemNode = _selectedNode;
      if (_selectedNode == null || itemNode?.Item == null) return;
      ItemDto item = itemNode.Item;
      var itemType = (WeItemType)item.ItemTypeId;
      if (itemType == WeItemType.SolutionModel) {
        var nextRank = await _appDataService.GetNextItemRank(item.Id) + 1;
        var newSubItem = await _appDataService.CreateRelatedItemAsync(item.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.SolutionImportModel, $"Import{nextRank}", string.Empty, "{}");

        var newParentRelation = newSubItem.IncomingRelations
          .FirstOrDefault(r => r.ItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
        if (newParentRelation == null) {
          // add error logging.
          return;
        }

        var projGuidProp = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItProjectGuid);
        if (projGuidProp != null) {
          string? val = projGuidProp?.Value ?? "";
          if (projGuidProp != null && string.IsNullOrEmpty(val)) {
            projGuidProp.Value = Guid.NewGuid().ToString("B").ToUpper();
            var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(projGuidProp);
            if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
          }
        }

        var newNode = newParentRelation.ToItemNode(newSubItem);
        AddNodeToSelected(_selectedNode, newNode);
        //if (newSubItem != null) await LoadRootProjects(newSubItem.Id);
      }
    }

    private async void miAddFile_Click(object sender, EventArgs e) {
      try {
        await AddFileAsync();
      } catch (Exception ex) {
        DoLogMessage("Failed to add file - error:" + ex.Message);
        MessageBox.Show($"Error adding file: {ex.Message}", "Add File Failed");
      }
    }

    private async Task AddFileAsync() {

      var itemNode = _selectedNode;
      if (_selectedNode == null || itemNode?.Item == null) return;
      ItemDto item = itemNode.Item;
      if (!item.IsValidFolderParent()) return;
      var nextRank = await _appDataService.GetNextItemRank(item.Id) + 1;
      var newSubItem = await _appDataService.CreateRelatedItemAsync(
        item.Id,
        (int)WeRelationTypes.Contains,
        (int)WeItemType.FileModel,
        $"File {nextRank}", string.Empty, "{}");

      if (newSubItem == null) {
        // add error logging.
        return;
      }

      var newParentRelation = newSubItem.IncomingRelations
        .FirstOrDefault(r => r.ItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
      if (newParentRelation == null) {
        // add error logging.
        return;
      }

      var newNode = newParentRelation.ToItemNode(newSubItem);
      AddNodeToSelected(_selectedNode, newNode);

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = item.ResolveParentFolderPath(edAppDefaultFolder.Text);
        var fileName = newSubItem.Name.Contains('.') ? newSubItem.Name : newSubItem.Name.UrlSafe() + ".md";
        var fullPath = Path.Combine(parentFolderPath, fileName);
        rootFolderProperty.Value = fullPath;
        var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(rootFolderProperty);
        if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
      }

      //await LoadRootProjects(newSubItem.Id);

    }

    private async void miAddLibrary_Click(object sender, EventArgs e) {
      try {
        await AddLibraryAsync();
      } catch (Exception ex) {
        DoLogMessage("Failed to add library - error:" + ex.Message);
        MessageBox.Show($"Error adding library: {ex.Message}", "Add Library Failed");
      }
    }
    private async Task AddLibraryAsync() {

      var itemNode = _selectedNode;
      if (_selectedNode == null || itemNode?.Item == null) return;
      ItemDto item = itemNode.Item;
      if (!item.IsValidFolderParent()) return;
      var nextRank = await _appDataService.GetNextItemRank(item.Id) + 1;
      var newSubItem = await _appDataService.CreateRelatedItemAsync(
        item.Id,
        (int)WeRelationTypes.Contains,
        (int)WeItemType.LibraryModel,
        $"Library{nextRank}",
        ItemDefaultsByTypeExt.GetTypeTemplate(itemNode.Item, WeItemType.LibraryModel),
        "{}");

      if (newSubItem == null) {
        // add error logging.
        return;
      }

      var newParentRelation = newSubItem.IncomingRelations
        .FirstOrDefault(r => r.ItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
      if (newParentRelation == null) {
        // add error logging.
        return;
      }

      var newNode = newParentRelation.ToItemNode(newSubItem);
      AddNodeToSelected(_selectedNode, newNode);

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = item.ResolveParentFolderPath(edAppDefaultFolder.Text);
        var fileName = newSubItem.Name.Contains('.') ? newSubItem.Name : newSubItem.Name.UrlSafe() + ".csproj";
        var fullPath = Path.Combine(parentFolderPath, fileName);
        rootFolderProperty.Value = fullPath;
        var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(rootFolderProperty);
        if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
      }

      var rootNamespaceProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespaceRoot);
      if (rootNamespaceProperty != null && string.IsNullOrEmpty(rootNamespaceProperty.Value)) {
        rootNamespaceProperty.Value = newSubItem.Name.NameSafe();
        var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(rootNamespaceProperty);
        if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
      }

      // await LoadRootProjects(newSubItem.Id);

    }


    private async void miAddDiModel_Click(object sender, EventArgs e) {
      try {
        await AddDiModel();
      } catch (Exception ex) {
        DoLogMessage("Failed to add DI model - error:" + ex.Message);
        MessageBox.Show($"Error adding DI model: {ex.Message}", "Add DI Model Failed");
      }
    }

    private async Task AddDiModel() {
      var itemNode = _selectedNode;
      if (_selectedNode == null || itemNode?.Item == null) return;
      ItemDto item = itemNode.Item;
      if (item.ItemTypeId != (int)WeItemType.LibraryModel) return;
      var nextRank = await _appDataService.GetNextItemRank(item.Id) + 1;
      var newSubItem = await _appDataService.CreateRelatedItemAsync(
        item.Id,
        (int)WeRelationTypes.Contains,
        (int)WeItemType.DependencyInjectionModel,
        $"DI Model {nextRank}",
        ItemDefaultsByTypeExt.GetTypeTemplate(itemNode.Item, WeItemType.DependencyInjectionModel),
        "{}");

      if (newSubItem == null) {
        // add error logging.
        return;
      }

      var newParentRelation = newSubItem.IncomingRelations
        .FirstOrDefault(r => r.ItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
      if (newParentRelation == null) {
        // add error logging.
        return;
      }

      var newNode = newParentRelation.ToItemNode(newSubItem);
      AddNodeToSelected(_selectedNode, newNode);

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = item.ResolveParentFolderPath(edAppDefaultFolder.Text);
        var fileName = "DependencyInjection.cs";
        var fullPath = Path.Combine(parentFolderPath, fileName);
        rootFolderProperty.Value = fullPath;
        var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(rootFolderProperty);
        if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
      }

      var rootNamespaceProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespaceRoot);
      if (rootNamespaceProperty != null && string.IsNullOrEmpty(rootNamespaceProperty.Value)) {
        rootNamespaceProperty.Value = newSubItem.Name;
        var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(rootNamespaceProperty);
        if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
      }

      //await LoadRootProjects(newSubItem.Id);
    }



    private async void miAddNamespace_Click(object sender, EventArgs e) {
      try {
        await AddNamespaceModel();
      } catch (Exception ex) {
        DoLogMessage("Failed to add namespace model - error:" + ex.Message);
        MessageBox.Show($"Error adding namespace model: {ex.Message}", "Add Namespace Model Failed");
      }
    }

    private async Task AddNamespaceModel() {
      var itemNode = _selectedNode;
      if (_selectedNode == null || itemNode?.Item == null) return;
      ItemDto item = itemNode.Item;
      if (item.ItemTypeId != (int)WeItemType.LibraryModel && item.ItemTypeId != (int)WeItemType.NamespaceModel) return;
      var nextRank = await _appDataService.GetNextItemRank(item.Id) + 1;
      var newSubItem = await _appDataService.CreateRelatedItemAsync(
        item.Id,
        (int)WeRelationTypes.Contains,
        (int)WeItemType.NamespaceModel,
        $"Namespace{nextRank}",
        ItemDefaultsByTypeExt.GetTypeTemplate(itemNode.Item, WeItemType.NamespaceModel),
        "{}");
      if (newSubItem == null) {
        // add error logging.
        return;
      }

      var newParentRelation = newSubItem.IncomingRelations
        .FirstOrDefault(r => r.ItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
      if (newParentRelation == null) {
        // add error logging.
        return;
      }

      var newNode = newParentRelation.ToItemNode(newSubItem);
      AddNodeToSelected(_selectedNode, newNode);

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (rootFolderProperty != null) {
        string parentFolderPath = item.ResolveParentFolderPath(edAppDefaultFolder.Text);
        var newFolderName = newSubItem.Name.UrlSafe();
        var fullPath = Path.Combine(parentFolderPath, newFolderName);
        if (rootFolderProperty.Value != fullPath) {
          rootFolderProperty.Value = fullPath;
          var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(rootFolderProperty);
          if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
        }
      }

      var rootNamespaceProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespace);
      if (rootNamespaceProperty != null && _selectedNode != null) {
        ItemNode parent = (ItemNode)_selectedNode;
        string newNamespace = parent.Item.ResolveParentNamespace("NoParentNamespace");
        if (_selectedNode.Item != null) {
          await UpdateNamespacePathIfNeededAsync(newSubItem, newNamespace);
        }
        rootNamespaceProperty.Value = newNamespace + "." + newSubItem.Name;
        var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(rootNamespaceProperty);
        if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
      }

      //await LoadRootProjects(newSubItem.Id);
    }

    private async void miAddClass_Click(object sender, EventArgs e) {
      try {
        await AddClassAsync();
      } catch (Exception ex) {
        DoLogMessage("Failed to add class model - error:" + ex.Message);
        MessageBox.Show($"Error adding class model: {ex.Message}", "Add Class Model Failed");
      }
    }

    private async Task AddClassAsync() {
      var itemNode = _selectedNode;
      if (_selectedNode == null || itemNode?.Item == null) return;
      ItemDto item = itemNode.Item;
      if (item.ItemTypeId != (int)WeItemType.LibraryModel && item.ItemTypeId != (int)WeItemType.NamespaceModel) return;
      var nextRank = await _appDataService.GetNextItemRank(item.Id) + 1;
      var newSubItem = await _appDataService.CreateRelatedItemAsync(
        item.Id,
        (int)WeRelationTypes.Contains,
        (int)WeItemType.ClassModel,
        $"Class{nextRank}",
        ItemDefaultsByTypeExt.GetTypeTemplate(itemNode.Item, WeItemType.ClassModel),
        "{}");
      if (newSubItem == null) {
        // add error logging.
        return;
      }

      var newParentRelation = newSubItem.IncomingRelations
        .FirstOrDefault(r => r.ItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
      if (newParentRelation == null) {
        // add error logging.
        return;
      }

      var newNode = newParentRelation.ToItemNode(newSubItem);
      AddNodeToSelected(_selectedNode, newNode);

      var classFilePathProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      var fileExtProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFileExt);
      if (classFilePathProperty != null && fileExtProperty != null) {
        string parentFolderPath = item.ResolveParentFolderPath(edAppDefaultFolder.Text);
        var newFileExt = fileExtProperty.Value ?? ".cs";
        var newFolderName = newSubItem.Name.UrlSafe();
        var fullPath = Path.Combine(parentFolderPath, newFolderName + newFileExt);
        if (classFilePathProperty.Value != fullPath) {
          classFilePathProperty.Value = fullPath;
          var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(classFilePathProperty);
          if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
        }
      }

      var rootNamespaceProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItNamespace);
      if (rootNamespaceProperty != null && _selectedNode != null) {
        ItemNode parent = (ItemNode)_selectedNode;
        string newNamespace = parent.Item.ResolveParentNamespace("NoParentNamespace");
        if (_selectedNode.Item != null) {
          await UpdateNamespacePathIfNeededAsync(newSubItem, newNamespace);
        }
        rootNamespaceProperty.Value = newNamespace;
        var updatedProp = await _appDataService.AddUpdateItemPropertyAsync(rootNamespaceProperty);
        if (updatedProp != null) newSubItem.AddOrUpdateProperty(updatedProp);
      }

      //await LoadRootProjects(newSubItem.Id);
    }

    private async void miAddClassImport_Click(object sender, EventArgs e) {
      try {
        await AddClassImport();
      } catch (Exception ex) {
        DoLogMessage("Failed to add class Import - error:" + ex.Message);
        MessageBox.Show($"Error adding class import: {ex.Message}", "Add Class Import Failed");
      }
    }
    private async Task AddClassImport() {
      var itemNode = _selectedNode;
      if (_selectedNode == null || itemNode?.Item == null) return;
      ItemDto item = itemNode.Item;
      var itemType = (WeItemType)item.ItemTypeId;
      if (itemType == WeItemType.ClassModel) {
        var nextRank = await _appDataService.GetNextItemRank(item.Id) + 1;
        var newSubItem = await _appDataService.CreateRelatedItemAsync(item.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.ClassImportModel, $"Import{nextRank}", string.Empty, "{}");

        var newParentRelation = newSubItem.IncomingRelations
          .FirstOrDefault(r => r.ItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
        if (newParentRelation == null) {
          // add error logging.
          return;
        }
        var newNode = newParentRelation.ToItemNode(newSubItem);
        AddNodeToSelected(_selectedNode, newNode);
        //if (newSubItem != null) await LoadRootProjects(newSubItem.Id);
      }
    }
    private async void miAddClassProp_Click(object sender, EventArgs e) {
      try {
        await AddClassProp();
      } catch (Exception ex) {
        DoLogMessage("Failed to add class property - error:" + ex.Message);
        MessageBox.Show($"Error adding class property: {ex.Message}", "Add Class Property Failed");
      }
    }
    private async Task AddClassProp() {
      var itemNode = _selectedNode;
      if (_selectedNode == null || itemNode?.Item == null) return;
      ItemDto item = itemNode.Item;
      var itemType = (WeItemType)item.ItemTypeId;
      if (itemType == WeItemType.ClassModel) {
        var nextRank = await _appDataService.GetNextItemRank(item.Id) + 1;
        var newSubItem = await _appDataService.CreateRelatedItemAsync(item.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.ClassPropertyModel, $"Prop{nextRank}", string.Empty, "{}");
        var newParentRelation = newSubItem.IncomingRelations
          .FirstOrDefault(r => r.ItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
        if (newParentRelation == null) {
          // add error logging.
          return;
        }
        var newNode = newParentRelation.ToItemNode(newSubItem);
        AddNodeToSelected(_selectedNode, newNode);
        //if (newSubItem != null) await LoadRootProjects(newSubItem.Id);
      }
    }
    private async void miAddClassMethod_Click(object sender, EventArgs e) {
      try {
        await AddClassMethod();
      } catch (Exception ex) {
        DoLogMessage("Failed to add class method - error:" + ex.Message);
        MessageBox.Show($"Error adding class method: {ex.Message}", "Add Class Method Failed");
      }
    }

    private async Task AddClassMethod() {
      var itemNode = _selectedNode;
      if (itemNode?.Item == null) return;
      ItemDto item = itemNode.Item;
      var itemType = (WeItemType)item.ItemTypeId;
      if (itemType == WeItemType.ClassModel) {
        var nextRank = await _appDataService.GetNextItemRank(item.Id) + 1;
        var newSubItem = await _appDataService.CreateRelatedItemAsync(item.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.ClassMethodModel, $"Method{nextRank}", string.Empty, "{}");

        var newParentRelation = newSubItem.IncomingRelations
          .FirstOrDefault(r => r.ItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
        if (newParentRelation == null) {
          // add error logging.
          return;
        }

        var newNode = newParentRelation.ToItemNode(newSubItem);
        AddNodeToSelected(_selectedNode, newNode);
        //if (newSubItem != null) await LoadRootProjects(newSubItem.Id);
      }
    }

    private async void miAddClassMethodParam_Click(object sender, EventArgs e) {
      try {
        await AddClassMethodParam();
      } catch (Exception ex) {
        DoLogMessage("Failed to add class method parameter - error:" + ex.Message);
        MessageBox.Show($"Error adding class method parameter: {ex.Message}", "Add Class Method Parameter Failed");
      }
    }

    private async Task AddClassMethodParam() {
      var itemNode = _selectedNode;
      if (_selectedNode == null || itemNode?.Item == null) return;
      ItemDto item = itemNode.Item;
      var itemType = (WeItemType)item.ItemTypeId;
      if (itemType == WeItemType.ClassMethodModel) {
        var nextRank = await _appDataService.GetNextItemRank(item.Id) + 1;
        var newSubItem = await _appDataService.CreateRelatedItemAsync(item.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.ClassMethodParameterModel, $"Param{nextRank}", string.Empty, "{}");
        var newParentRelation = newSubItem.IncomingRelations
          .FirstOrDefault(r => r.ItemId == item.Id && r.RelationTypeId == (int)WeRelationTypes.Contains);
        if (newParentRelation == null) {
          // add error logging.
          return;
        }
        var newNode = newParentRelation.ToItemNode(newSubItem);
        AddNodeToSelected(_selectedNode, newNode);
        //if (newSubItem != null) await LoadRootProjects(newSubItem.Id);
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
    #endregion

    #region On Generate Context Menu
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
            case WeItemType.DependencyInjectionModel:
              var diCode = await _itemTemplateService.GetDependencyInjectionTemplate(item.Id);
              if (diCode != null) {
                item.Description = diCode;
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

    #endregion


  }
}
