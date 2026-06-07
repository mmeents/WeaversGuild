using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheLoomApp {
  public partial class PreviewAttemptDialog : Form {
    public PreviewAttemptDialog() {
      InitializeComponent();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string SystemPrompt {
      get => edSystemPrompt.Text;
      set => edSystemPrompt.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string UserPrompt {
      get => edUserPrompt.Text;
      set => edUserPrompt.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Operator {
      get => edOperator.Text;
      set => edOperator.Text = value; 
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Harness {
      get => edHarness.Text;
      set => edHarness.Text = value;
    }



  }
}
