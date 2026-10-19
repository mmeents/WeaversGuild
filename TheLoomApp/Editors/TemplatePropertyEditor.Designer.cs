namespace TheLoomApp.Editors {
  partial class TemplatePropertyEditor {
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplatePropertyEditor));
      lbName = new Label();
      tabControl1 = new TabControl();
      tabTemplate = new TabPage();
      textBox1 = new FastColoredTextBoxNS.FastColoredTextBox();
      tabPreview = new TabPage();
      txtPreview = new FastColoredTextBoxNS.FastColoredTextBox();
      button1 = new Button();
      button2 = new Button();
      button3 = new Button();
      tabControl1.SuspendLayout();
      tabTemplate.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)textBox1).BeginInit();
      tabPreview.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)txtPreview).BeginInit();
      SuspendLayout();
      // 
      // lbName
      // 
      lbName.AutoSize = true;
      lbName.Location = new Point(12, 7);
      lbName.Name = "lbName";
      lbName.Size = new Size(38, 15);
      lbName.TabIndex = 0;
      lbName.Text = "label1";
      // 
      // tabControl1
      // 
      tabControl1.Alignment = TabAlignment.Bottom;
      tabControl1.Controls.Add(tabTemplate);
      tabControl1.Controls.Add(tabPreview);
      tabControl1.Dock = DockStyle.Right;
      tabControl1.Location = new Point(105, 0);
      tabControl1.Multiline = true;
      tabControl1.Name = "tabControl1";
      tabControl1.SelectedIndex = 0;
      tabControl1.Size = new Size(315, 131);
      tabControl1.TabIndex = 1;
      tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
      // 
      // tabTemplate
      // 
      tabTemplate.Controls.Add(textBox1);
      tabTemplate.Location = new Point(4, 4);
      tabTemplate.Name = "tabTemplate";
      tabTemplate.Padding = new Padding(3);
      tabTemplate.Size = new Size(307, 103);
      tabTemplate.TabIndex = 0;
      tabTemplate.Text = "Template";
      tabTemplate.UseVisualStyleBackColor = true;
      // 
      // textBox1
      // 
      textBox1.AutoCompleteBracketsList = new char[]
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
      textBox1.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
      textBox1.AutoScrollMinSize = new Size(0, 14);
      textBox1.BackBrush = null;
      textBox1.CharHeight = 14;
      textBox1.CharWidth = 8;
      textBox1.DefaultMarkerSize = 8;
      textBox1.DisabledColor = Color.FromArgb(100, 180, 180, 180);
      textBox1.Dock = DockStyle.Fill;
      textBox1.FindForm = null;
      textBox1.Font = new Font("Courier New", 9.75F);
      textBox1.GoToForm = null;
      textBox1.Hotkeys = resources.GetString("textBox1.Hotkeys");
      textBox1.IsReplaceMode = false;
      textBox1.Location = new Point(3, 3);
      textBox1.Name = "textBox1";
      textBox1.Paddings = new Padding(0);
      textBox1.ReplaceForm = null;
      textBox1.SelectionColor = Color.FromArgb(60, 192, 255, 255);
      textBox1.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("textBox1.ServiceColors");
      textBox1.Size = new Size(301, 97);
      textBox1.TabIndex = 0;
      textBox1.WordWrap = true;
      textBox1.Zoom = 100;
      textBox1.TextChanged += textBox1_TextChanged;
      // 
      // tabPreview
      // 
      tabPreview.Controls.Add(txtPreview);
      tabPreview.Location = new Point(4, 4);
      tabPreview.Name = "tabPreview";
      tabPreview.Padding = new Padding(3);
      tabPreview.Size = new Size(307, 103);
      tabPreview.TabIndex = 1;
      tabPreview.Text = "Preview";
      tabPreview.UseVisualStyleBackColor = true;
      // 
      // txtPreview
      // 
      txtPreview.AutoCompleteBracketsList = new char[]
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
      txtPreview.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
      txtPreview.AutoScrollMinSize = new Size(0, 14);
      txtPreview.BackBrush = null;
      txtPreview.CharHeight = 14;
      txtPreview.CharWidth = 8;
      txtPreview.DefaultMarkerSize = 8;
      txtPreview.DisabledColor = Color.FromArgb(100, 180, 180, 180);
      txtPreview.Dock = DockStyle.Fill;
      txtPreview.FindForm = null;
      txtPreview.Font = new Font("Courier New", 9.75F);
      txtPreview.GoToForm = null;
      txtPreview.Hotkeys = resources.GetString("txtPreview.Hotkeys");
      txtPreview.IsReplaceMode = false;
      txtPreview.Location = new Point(3, 3);
      txtPreview.Name = "txtPreview";
      txtPreview.Paddings = new Padding(0);
      txtPreview.ReadOnly = true;
      txtPreview.ReplaceForm = null;
      txtPreview.SelectionColor = Color.FromArgb(60, 192, 255, 255);
      txtPreview.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("txtPreview.ServiceColors");
      txtPreview.Size = new Size(301, 97);
      txtPreview.TabIndex = 0;
      txtPreview.Text = "fastColoredTextBox1";
      txtPreview.WordWrap = true;
      txtPreview.Zoom = 100;
      // 
      // button1
      // 
      button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      button1.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
      button1.Location = new Point(3, 107);
      button1.Name = "button1";
      button1.Size = new Size(43, 21);
      button1.TabIndex = 2;
      button1.Text = "more";
      button1.UseVisualStyleBackColor = true;
      button1.Click += button1_Click;
      // 
      // button2
      // 
      button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      button2.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
      button2.Location = new Point(3, 50);
      button2.Name = "button2";
      button2.Size = new Size(43, 21);
      button2.TabIndex = 3;
      button2.Text = "copy";
      button2.UseVisualStyleBackColor = true;
      button2.Click += button2_Click;
      // 
      // button3
      // 
      button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      button3.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
      button3.Location = new Point(3, 78);
      button3.Name = "button3";
      button3.Size = new Size(43, 21);
      button3.TabIndex = 4;
      button3.Text = "paste";
      button3.UseVisualStyleBackColor = true;
      button3.Click += button3_Click;
      // 
      // TemplatePropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(button3);
      Controls.Add(button2);
      Controls.Add(button1);
      Controls.Add(tabControl1);
      Controls.Add(lbName);
      Name = "TemplatePropertyEditor";
      Size = new Size(420, 131);
      Resize += TemplatePropertyEditor_Resize;
      tabControl1.ResumeLayout(false);
      tabTemplate.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)textBox1).EndInit();
      tabPreview.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)txtPreview).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label lbName;
    private TabControl tabControl1;
    private TabPage tabTemplate;
    private TabPage tabPreview;
    private FastColoredTextBoxNS.FastColoredTextBox textBox1;
    private FastColoredTextBoxNS.FastColoredTextBox txtPreview;
    private Button button1;
    private Button button2;
    private Button button3;
  }
}
