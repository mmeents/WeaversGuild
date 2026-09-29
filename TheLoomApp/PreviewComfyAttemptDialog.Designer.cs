namespace TheLoomApp {
  partial class PreviewComfyAttemptDialog {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewComfyAttemptDialog));
      edOperator = new TextBox();
      label4 = new Label();
      label3 = new Label();
      edHarness = new TextBox();
      button2 = new Button();
      button1 = new Button();
      label1 = new Label();
      edSystemPrompt = new FastColoredTextBoxNS.FastColoredTextBox();
      ((System.ComponentModel.ISupportInitialize)edSystemPrompt).BeginInit();
      SuspendLayout();
      // 
      // edOperator
      // 
      edOperator.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edOperator.BackColor = SystemColors.Menu;
      edOperator.BorderStyle = BorderStyle.FixedSingle;
      edOperator.Location = new Point(145, 78);
      edOperator.Name = "edOperator";
      edOperator.Size = new Size(576, 23);
      edOperator.TabIndex = 17;
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new Point(72, 80);
      label4.Name = "label4";
      label4.Size = new Size(53, 15);
      label4.TabIndex = 16;
      label4.Text = "Op Todo";
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(79, 42);
      label3.Name = "label3";
      label3.Size = new Size(52, 15);
      label3.TabIndex = 15;
      label3.Text = "Harness:";
      // 
      // edHarness
      // 
      edHarness.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edHarness.BackColor = SystemColors.Menu;
      edHarness.BorderStyle = BorderStyle.FixedSingle;
      edHarness.Location = new Point(146, 40);
      edHarness.Name = "edHarness";
      edHarness.Size = new Size(576, 23);
      edHarness.TabIndex = 14;
      // 
      // button2
      // 
      button2.DialogResult = DialogResult.OK;
      button2.Location = new Point(308, 517);
      button2.Name = "button2";
      button2.Size = new Size(118, 31);
      button2.TabIndex = 13;
      button2.Text = "Send Attempt";
      button2.UseVisualStyleBackColor = true;
      // 
      // button1
      // 
      button1.DialogResult = DialogResult.Cancel;
      button1.Location = new Point(439, 517);
      button1.Name = "button1";
      button1.Size = new Size(79, 31);
      button1.TabIndex = 12;
      button1.Text = "Cancel";
      button1.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(49, 111);
      label1.Name = "label1";
      label1.Size = new Size(80, 15);
      label1.TabIndex = 11;
      label1.Text = "Request JSON";
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
      edSystemPrompt.Location = new Point(146, 111);
      edSystemPrompt.Margin = new Padding(3, 4, 3, 4);
      edSystemPrompt.Name = "edSystemPrompt";
      edSystemPrompt.Paddings = new Padding(0);
      edSystemPrompt.ReplaceForm = null;
      edSystemPrompt.SelectionColor = Color.FromArgb(60, 0, 0, 255);
      edSystemPrompt.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("edSystemPrompt.ServiceColors");
      edSystemPrompt.Size = new Size(574, 358);
      edSystemPrompt.TabIndex = 10;
      edSystemPrompt.WordWrap = true;
      edSystemPrompt.Zoom = 100;
      // 
      // PreviewComfyAttemptDialog
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(750, 589);
      Controls.Add(edOperator);
      Controls.Add(label4);
      Controls.Add(label3);
      Controls.Add(edHarness);
      Controls.Add(button2);
      Controls.Add(button1);
      Controls.Add(label1);
      Controls.Add(edSystemPrompt);
      Name = "PreviewComfyAttemptDialog";
      StartPosition = FormStartPosition.CenterParent;
      Text = "PreviewComfyAttemptDialog";
      ((System.ComponentModel.ISupportInitialize)edSystemPrompt).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private TextBox edOperator;
    private Label label4;
    private Label label3;
    private TextBox edHarness;
    private Button button2;
    private Button button1;
    private Label label1;
    private FastColoredTextBoxNS.FastColoredTextBox edSystemPrompt;
  }
}