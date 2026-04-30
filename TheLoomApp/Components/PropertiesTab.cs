using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Weavers.Core.Interfaces;
using Weavers.Core.Models;
using TheLoomApp.Editors;
using Weavers.Core.Service;
using Microsoft.Extensions.DependencyInjection;

namespace TheLoomApp.Components {
  public class PropertiesTab : TabPage {
    private string _titleLabel = "Properties";
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IAppDataService _appDataService;
    private readonly IItemTypeLookupComboProvider _itemTypeLookupComboProvider;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PageName {
      get { return this.Text; }
      set { this.Text = value; }
    }
    public PropertiesTab(IServiceScopeFactory scopeFactory) {
      _scopeFactory = scopeFactory;    
      var scope = _scopeFactory.CreateScope();
      _appDataService = scope.ServiceProvider.GetRequiredService<IAppDataService>();
      _itemTypeLookupComboProvider = scope.ServiceProvider.GetRequiredService<IItemTypeLookupComboProvider>();
      BasePanel = new Panel {
        Dock = DockStyle.Fill,
        AutoScroll = true,
        BackColor = System.Drawing.Color.WhiteSmoke
      };
      this.Controls.Add(BasePanel);
      PageName = "Properties";
      _titleLabel = "Properties";
    }

    private Button? OkButton { get; set; }
    private Button? CancelButton { get; set; }

    private bool _isEditing = false;
    private Panel BasePanel { get; set; }
    private Panel? TitlePanel { get; set; }
    private Panel? MenuPanel { get; set; }
    private ConcurrentDictionary<int, IAmAFieldEditor> _propertyEditors = new ConcurrentDictionary<int, IAmAFieldEditor>();
 
    private Label? TitleLabel { get; set; }

    private List<ItemPropertyDto>? _tableRow;


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public List<ItemPropertyDto>? ItemProps {
      get { return _tableRow; }
      set {
        _tableRow = value;
        if (value != null) {
          ResetToRow();
        } else {
          if (TitleLabel != null) TitleLabel.Text = "";
        }
      }
    }

  //  public MessageLogTab? LogTab { get; set; }

    //private void LogMessage(string message) {
    //  if (LogTab != null) {
    //    LogTab.LogMsg(message);
    //  } else {
    //    MessageBox.Show(message, "Log Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
    //  }
    //}

    private bool _disabled = false;
    private int _labelRight = 50; // Default label right position
      

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]

    public virtual bool Disabled {
      get { return _disabled; }
      set {
        _disabled = value;
        SetDisabled(value);
      }
    }

    private void SetDisabled(bool disabled) {
      _disabled = disabled;
      if (_disabled) {
        BasePanel.Enabled = false;
        if (TitleLabel != null) TitleLabel.Text = $"{_titleLabel} (Disabled)";
        if (_propertyEditors.Keys.Count > 0) {
          foreach (var editor in _propertyEditors.Values) {
            editor.Enabled = false;
          }
        }
      } else {
        BasePanel.Enabled = true;
        if (TitleLabel != null) TitleLabel.Text = $"{_titleLabel}";
      }
    }

    public event Action OnPostEvent = delegate { };

    private void DoOnPostEvent() {
      if (OnPostEvent != null) {
        OnPostEvent();
      }
    }

    private async void OkButton_Click(object? sender, EventArgs e) {
      if (ItemProps != null) {
        try {

          foreach (var field in ItemProps) {
            if (!_propertyEditors.ContainsKey(field.Id)) continue; // hidden fields have no editor.
            var editor = _propertyEditors[field.Id];
            if (editor.Modified) {
              editor.CommitToField(); // Commit changes to the field
              editor.Modified = false; // Reset modified state
              await _appDataService.AddUpdateItemPropertyAsync(field);              
            }
          }

          SetEditingMode(false);
          DoOnPostEvent();

        } catch (Exception ex) {
          MessageBox.Show($"Error updating item properties: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void CancelButton_Click(object? sender, EventArgs e) {
      if (ItemProps != null) {
        ResetToRow(); 
      }
    }

    /// <summary>
    /// Builds all the editors for the current row.
    /// </summary>
    public void ResetToRow() {
      if (ItemProps == null) {
        if (TitleLabel != null) TitleLabel.Text = "";
        return;
      }
      BasePanel.Controls.Clear();
      _propertyEditors.Clear();     
      MenuPanel = new Panel {
        Dock = DockStyle.Top,
        Height = 32
      };
      BasePanel.Controls.Add(MenuPanel);

      
      foreach (var field in ItemProps) {
        var propertyEditor = PropertyEditorFactory.CreateEditor(field, _itemTypeLookupComboProvider);
        if (propertyEditor != null) {
          propertyEditor.Field = field;
          ((UserControl)propertyEditor).Dock = DockStyle.Top;
          propertyEditor.LabelRight = 80; // Set a default label right position
          propertyEditor.ValueChanged += PropertyEditor_ValueChanged;
          _propertyEditors[field.Id] = propertyEditor;
          BasePanel.Controls.Add(((UserControl)propertyEditor));
        }
      }
      if (ItemProps.Count > 0) {
        _titleLabel = " "+ (ItemProps[0].Item?.Name ?? "No item selected.");
      } else {
        _titleLabel = " Item has no properties.";
      }
      TitlePanel = new Panel {
        Dock = DockStyle.Top,
        Height = 30,
        BackColor = System.Drawing.Color.Silver
      };
      // Initialize the properties tab with a label
      TitleLabel = new Label {
        Text = _titleLabel,
        TextAlign = System.Drawing.ContentAlignment.MiddleLeft
      };

      TitlePanel.Controls.Add(TitleLabel);
      BasePanel.Controls.Add(TitlePanel);
      CreateMenuButtons();
      SetEditingMode(false);

    }

        
    /// <summary>
    /// Gets editor height based on field and schema configuration
    /// </summary>
    //private int GetEditorHeight(FieldModel field, TableUISchema? schema) {
    //  if (schema != null) {
    //    var column = field.OwnerRow?.Owner?.Columns.Values.FirstOrDefault(c => c.Id == field.ColumnId);
    //    if (column != null) {
    //      var config = schema.GetConfig(column.ColumnName);
    //      if (config.WeEditorType.HasValue) {
    //        return PropertyEditorFactory.GetRecommendedHeight(config.WeEditorType.Value);
    //      }
    //    }
    //  }

      // Default height
    //  return 30;
    //}

    private void PropertyEditor_ValueChanged(object? sender, EventArgs e) {
      if (!_isEditing) {
        SetEditingMode(true); // Enable editing mode if a property changes
      }
    }

    private void CreateMenuButtons() {
      if (MenuPanel == null) return;

      MenuPanel.Controls.Clear();
      int left = _labelRight;
      // OK Button
      OkButton = new Button {
        Text = "OK",
        Size = new Size(100, 28),
        Location = new Point(left + 2, 2),
        Enabled = false, // Start disabled
      };
      OkButton.Click += OkButton_Click;

      // Cancel Button  
      CancelButton = new Button {
        Text = "Cancel",
        Size = new Size(100, 28),
        Location = new Point(left + OkButton.Width + 7, 2),
        Enabled = false, // Start disabled
      };
      CancelButton.Click += CancelButton_Click;

      MenuPanel.Controls.Add(OkButton);
      MenuPanel.Controls.Add(CancelButton);
      if (MenuPanel.Visible) MenuPanel.Visible = false;
    }

    /// <summary>
    /// Sets the right alignment position for labels and adjusts related UI elements accordingly.
    /// </summary>
    /// <remarks>This method updates the right position of labels for all property editors. If the menu panel
    /// and buttons are present, it also adjusts the positions of the OK and Cancel buttons based on the specified right
    /// alignment.</remarks>
    /// <param name="right">The right alignment position for the labels.</param>
    public void SetLabelRight(int right) {
      _labelRight = right;
      if (_propertyEditors.Count > 0) {
        foreach (var editor in _propertyEditors.Values) {
          editor.LabelRight = right;
        }
      }
      if (MenuPanel != null && OkButton != null && CancelButton != null) {
        OkButton.Left = _labelRight + 2;
        CancelButton.Left = OkButton.Left + OkButton.Width + 5;
      }
    }

    public void SetEditingMode(bool editing) {
      _isEditing = editing;
      if (MenuPanel == null) {
        return;
      }
      if (!MenuPanel.Visible) MenuPanel.Visible = true;
      if (OkButton != null) OkButton.Enabled = editing;
      if (CancelButton != null) CancelButton.Enabled = editing;
      if (MenuPanel.Visible != editing) MenuPanel.Visible = editing;

      // Update visual state of all editors
      foreach (var editor in _propertyEditors.Values) {
        if (editor is IEditStateAware editAware) {
          editAware.SetEditingState(editing);
        }
      }

      // Update title to show state
      if (TitleLabel != null) {
        TitleLabel.Text = editing ? $"{_titleLabel} (Editing)" : _titleLabel;
      }
    }

    /// <summary>
    /// Validates all fields before allowing commit
    /// </summary>
    public bool ValidateAllFields() {
      var validationErrors = new List<string>();

      foreach (var editor in _propertyEditors.Values) {
        try {
          if (editor.Modified) {
            // Test commit without actually committing
            int? tempField = null; // editor.Field;
            if (tempField != null) {
              // This would validate the editor's current value
              editor.CommitToField();
              // If we get here, it's valid, so reset modified state temporarily
              // Real commit happens in OkButton_Click
            }
          }
        } catch (ValidationException vex) {
          validationErrors.Add($"{editor.PropertyName}: {vex.Message}");
        } catch (Exception ex) {
          validationErrors.Add($"{editor.PropertyName}: {ex.Message}");
        }
      }

      if (validationErrors.Any()) {
        var errorMessage = "Validation errors:\n" + string.Join("\n", validationErrors);
        MessageBox.Show(errorMessage, "Validation Failed",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
      }

      return true;
    }

    /// <summary>
    /// Gets the currently modified editors
    /// </summary>
    public IEnumerable<IAmAFieldEditor> GetModifiedEditors() {
      return _propertyEditors.Values.Where(e => e.Modified);
    }

    /// <summary>
    /// Resets all editors to their original field values
    /// </summary>
    public void ResetAllEditors() {
      foreach (var editor in _propertyEditors.Values) {
        editor.ResetToField();
        editor.Modified = false;
      }
      if (MenuPanel != null && MenuPanel.Visible) MenuPanel.Visible = false;
    }

  }

  public static class PropertiesTabColors {
    public static readonly Color StandardEditorWhite = Color.White;
    public static readonly Color NormalBackground = Color.FromKnownColor(KnownColor.Control);
    public static readonly Color EditingBackground = Color.FromArgb(244, 245, 220);

    public static readonly Color LabelBackground = Color.FromArgb(244, 245, 220);
    public static readonly Color EditingLabelBackground = Color.FromArgb(244, 245, 220);
    public static readonly Color DisabledBackground = Color.LightGray;
    public static readonly Color ValidationError = Color.FromArgb(255, 230, 230);
  }
}
