namespace TheLoomApp.Editors {
  partial class PasswordPropertyEditor  {
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
      textBox1 = new TextBox();
      lbName = new Label();
      cbView = new CheckBox();
      SuspendLayout();
      // 
      // textBox1
      // 
      textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      textBox1.Location = new Point(85, 3);
      textBox1.Name = "textBox1";
      textBox1.Size = new Size(266, 23);
      textBox1.TabIndex = 0;
      textBox1.TextChanged += textBox1_TextChanged;
      textBox1.Enter += textBox1_Enter;
      // 
      // lbName
      // 
      lbName.AutoSize = true;
      lbName.Location = new Point(9, 8);
      lbName.Name = "lbName";
      lbName.Size = new Size(38, 15);
      lbName.TabIndex = 1;
      lbName.Text = "label1";
      // 
      // cbView
      // 
      cbView.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      cbView.AutoSize = true;
      cbView.Location = new Point(358, 4);
      cbView.Name = "cbView";
      cbView.Size = new Size(51, 19);
      cbView.TabIndex = 2;
      cbView.Text = "View";
      cbView.UseVisualStyleBackColor = true;
      cbView.CheckedChanged += cbView_CheckedChanged;
      // 
      // PasswordPropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(cbView);
      Controls.Add(lbName);
      Controls.Add(textBox1);
      Name = "PasswordPropertyEditor";
      Size = new Size(414, 30);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private TextBox textBox1;
    private Label lbName;
    private CheckBox cbView;
  }
}
