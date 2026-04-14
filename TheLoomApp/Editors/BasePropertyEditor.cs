using System.ComponentModel;
using Weavers.Core.Interfaces;
using Weavers.Core.Models;

namespace TheLoomApp.Editors {
  public partial class BasePropertyEditor : UserControl, IAmAFieldViewer {
    public BasePropertyEditor() {
      InitializeComponent();
    }

    private bool _modified = false;
    private ItemPropertyDto? _field;


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual ItemPropertyDto? Field {
      get { return _field; }
      set {
        _field = value;
        ResetToField();        
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual int LabelRight {
      get {
        return lbName.Left + lbName.Width;
      }
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        lbValue.Left = value + 3;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual string PropertyName {
      get { return lbName.Text; }
      set { lbName.Text = value; }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Modified {
      get { return _modified; }
      set { _modified = value; }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new virtual bool Enabled {
      get {
        return base.Enabled;
      }
      set {
        base.Enabled = value;
        lbName.Enabled = value;
        lbValue.Enabled = value;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PropertyValue { 
      get { return lbValue.Text; } 
      set { lbValue.Text = value; }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public void ResetToField() {
      if (Field != null) {
        PropertyName = Field?.Name ?? string.Empty;
        PropertyValue = Field?.Value ?? string.Empty;
      } else {
        PropertyName = string.Empty;
        PropertyValue = string.Empty;
      }
    }
  }
}
