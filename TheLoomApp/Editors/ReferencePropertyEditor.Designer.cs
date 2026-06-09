namespace TheLoomApp.Editors {
  partial class ReferencePropertyEditor {
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
      cbValues = new ComboBox();
      lbName = new Label();
      cbType = new ComboBox();
      lbType = new Label();
      SuspendLayout();
      // 
      // cbValues
      // 
      cbValues.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      cbValues.FormattingEnabled = true;
      cbValues.Location = new Point(53, 38);
      cbValues.Name = "cbValues";
      cbValues.Size = new Size(371, 23);
      cbValues.TabIndex = 0;
      cbValues.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
      // 
      // lbName
      // 
      lbName.AutoSize = true;
      lbName.Location = new Point(9, 42);
      lbName.Name = "lbName";
      lbName.Size = new Size(38, 15);
      lbName.TabIndex = 1;
      lbName.Text = "label1";
      // 
      // cbType
      // 
      cbType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      cbType.FormattingEnabled = true;
      cbType.Location = new Point(53, 9);
      cbType.Name = "cbType";
      cbType.Size = new Size(371, 23);
      cbType.TabIndex = 2;
      cbType.SelectedIndexChanged += cbType_SelectedIndexChanged;
      // 
      // lbType
      // 
      lbType.AutoSize = true;
      lbType.Location = new Point(15, 12);
      lbType.Name = "lbType";
      lbType.Size = new Size(32, 15);
      lbType.TabIndex = 3;
      lbType.Text = "Type";
      // 
      // ReferencePropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(lbType);
      Controls.Add(cbType);
      Controls.Add(lbName);
      Controls.Add(cbValues);
      Name = "ReferencePropertyEditor";
      Size = new Size(435, 71);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private ComboBox cbValues;
    private Label lbName;
    private ComboBox cbType;
    private Label lbType;
  }
}
