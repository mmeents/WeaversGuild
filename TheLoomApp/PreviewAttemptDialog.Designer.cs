namespace TheLoomApp {
  partial class PreviewAttemptDialog {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewAttemptDialog));
      edSystemPrompt = new FastColoredTextBoxNS.FastColoredTextBox();
      edUserPrompt = new FastColoredTextBoxNS.FastColoredTextBox();
      label1 = new Label();
      label2 = new Label();
      button1 = new Button();
      button2 = new Button();
      edHarness = new TextBox();
      label3 = new Label();
      label4 = new Label();
      edOperator = new TextBox();
      ((System.ComponentModel.ISupportInitialize)edSystemPrompt).BeginInit();
      ((System.ComponentModel.ISupportInitialize)edUserPrompt).BeginInit();
      SuspendLayout();
      // 
      // edSystemPrompt
      // 
      edSystemPrompt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edSystemPrompt.AutoCompleteBracketsList = new char[]
  {
    '(',
    ')',
    '{',
    '}',
    '[',
    ']',
    '"',
    '"',
    '\'',
    '\''
  };
      edSystemPrompt.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
      edSystemPrompt.AutoScrollMinSize = new Size(0, 16);
      edSystemPrompt.BackBrush = null;
      edSystemPrompt.BackColor = SystemColors.Menu;
      edSystemPrompt.BorderStyle = BorderStyle.FixedSingle;
      edSystemPrompt.CharHeight = 16;
      edSystemPrompt.CharWidth = 9;
      edSystemPrompt.DefaultMarkerSize = 8;
      edSystemPrompt.DisabledColor = Color.FromArgb(100, 180, 180, 180);
      edSystemPrompt.FindForm = null;
      edSystemPrompt.Font = new Font("Courier New", 10.8F);
      edSystemPrompt.GoToForm = null;
      edSystemPrompt.Hotkeys = resources.GetString("edSystemPrompt.Hotkeys");
      edSystemPrompt.IsReplaceMode = false;
      edSystemPrompt.Location = new Point(120, 90);
      edSystemPrompt.Margin = new Padding(3, 4, 3, 4);
      edSystemPrompt.Name = "edSystemPrompt";
      edSystemPrompt.Paddings = new Padding(0);
      edSystemPrompt.ReplaceForm = null;
      edSystemPrompt.SelectionColor = Color.FromArgb(60, 0, 0, 255);
      edSystemPrompt.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("edSystemPrompt.ServiceColors");
      edSystemPrompt.Size = new Size(574, 174);
      edSystemPrompt.TabIndex = 0;
      edSystemPrompt.WordWrap = true;
      edSystemPrompt.Zoom = 100;
      // 
      // edUserPrompt
      // 
      edUserPrompt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edUserPrompt.AutoCompleteBracketsList = new char[]
  {
    '(',
    ')',
    '{',
    '}',
    '[',
    ']',
    '"',
    '"',
    '\'',
    '\''
  };
      edUserPrompt.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
      edUserPrompt.AutoScrollMinSize = new Size(0, 16);
      edUserPrompt.BackBrush = null;
      edUserPrompt.BackColor = SystemColors.Menu;
      edUserPrompt.BorderStyle = BorderStyle.FixedSingle;
      edUserPrompt.CharHeight = 16;
      edUserPrompt.CharWidth = 9;
      edUserPrompt.DefaultMarkerSize = 8;
      edUserPrompt.DisabledColor = Color.FromArgb(100, 180, 180, 180);
      edUserPrompt.FindForm = null;
      edUserPrompt.Font = new Font("Courier New", 10.8F);
      edUserPrompt.GoToForm = null;
      edUserPrompt.Hotkeys = resources.GetString("edUserPrompt.Hotkeys");
      edUserPrompt.IsReplaceMode = false;
      edUserPrompt.Location = new Point(120, 292);
      edUserPrompt.Margin = new Padding(3, 4, 3, 4);
      edUserPrompt.Name = "edUserPrompt";
      edUserPrompt.Paddings = new Padding(0);
      edUserPrompt.ReplaceForm = null;
      edUserPrompt.SelectionColor = Color.FromArgb(60, 0, 0, 255);
      edUserPrompt.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("edUserPrompt.ServiceColors");
      edUserPrompt.Size = new Size(574, 178);
      edUserPrompt.TabIndex = 1;
      edUserPrompt.WordWrap = true;
      edUserPrompt.Zoom = 100;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(2, 90);
      label1.Name = "label1";
      label1.Size = new Size(106, 19);
      label1.TabIndex = 2;
      label1.Text = "System Prompt:";
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(20, 292);
      label2.Name = "label2";
      label2.Size = new Size(90, 19);
      label2.TabIndex = 3;
      label2.Text = "User Prompt:";
      // 
      // button1
      // 
      button1.DialogResult = DialogResult.Cancel;
      button1.Location = new Point(413, 496);
      button1.Name = "button1";
      button1.Size = new Size(79, 31);
      button1.TabIndex = 4;
      button1.Text = "Cancel";
      button1.UseVisualStyleBackColor = true;
      // 
      // button2
      // 
      button2.DialogResult = DialogResult.OK;
      button2.Location = new Point(282, 496);
      button2.Name = "button2";
      button2.Size = new Size(118, 31);
      button2.TabIndex = 5;
      button2.Text = "Send Attempt";
      button2.UseVisualStyleBackColor = true;
      // 
      // edHarness
      // 
      edHarness.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edHarness.BackColor = SystemColors.Menu;
      edHarness.BorderStyle = BorderStyle.FixedSingle;
      edHarness.Location = new Point(120, 19);
      edHarness.Name = "edHarness";
      edHarness.Size = new Size(576, 26);
      edHarness.TabIndex = 6;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(53, 21);
      label3.Name = "label3";
      label3.Size = new Size(61, 19);
      label3.TabIndex = 7;
      label3.Text = "Harness:";
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new Point(46, 59);
      label4.Name = "label4";
      label4.Size = new Size(68, 19);
      label4.TabIndex = 8;
      label4.Text = "Operator:";
      // 
      // edOperator
      // 
      edOperator.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edOperator.BackColor = SystemColors.Menu;
      edOperator.BorderStyle = BorderStyle.FixedSingle;
      edOperator.Location = new Point(119, 57);
      edOperator.Name = "edOperator";
      edOperator.Size = new Size(576, 26);
      edOperator.TabIndex = 9;
      // 
      // PreviewAttemptDialog
      // 
      AutoScaleDimensions = new SizeF(8F, 19F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(708, 542);
      Controls.Add(edOperator);
      Controls.Add(label4);
      Controls.Add(label3);
      Controls.Add(edHarness);
      Controls.Add(button2);
      Controls.Add(button1);
      Controls.Add(label2);
      Controls.Add(label1);
      Controls.Add(edUserPrompt);
      Controls.Add(edSystemPrompt);
      Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
      Margin = new Padding(3, 4, 3, 4);
      Name = "PreviewAttemptDialog";
      StartPosition = FormStartPosition.CenterParent;
      Text = "Preview Todo Attempt Prompt";
      ((System.ComponentModel.ISupportInitialize)edSystemPrompt).EndInit();
      ((System.ComponentModel.ISupportInitialize)edUserPrompt).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private FastColoredTextBoxNS.FastColoredTextBox edSystemPrompt;
    private FastColoredTextBoxNS.FastColoredTextBox edUserPrompt;
    private Label label1;
    private Label label2;
    private Button button1;
    private Button button2;
    private TextBox edHarness;
    private Label label3;
    private Label label4;
    private TextBox edOperator;
  }
}