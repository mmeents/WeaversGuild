namespace TheLoomApp.Editors {
  partial class MemoPropertyEditor {
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
      lbName = new Label();
      textBox1 = new TextBox();
      button1 = new Button();
      button3 = new Button();
      button2 = new Button();
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
      // textBox1
      // 
      textBox1.AcceptsReturn = true;
      textBox1.AcceptsTab = true;
      textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      textBox1.Location = new Point(119, 4);
      textBox1.Multiline = true;
      textBox1.Name = "textBox1";
      textBox1.ScrollBars = ScrollBars.Both;
      textBox1.Size = new Size(298, 89);
      textBox1.TabIndex = 1;
      textBox1.TextChanged += TextBox1_TextChanged;
      // 
      // button1
      // 
      button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      button1.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
      button1.Location = new Point(5, 75);
      button1.Name = "button1";
      button1.Size = new Size(41, 21);
      button1.TabIndex = 3;
      button1.Text = "more";
      button1.UseVisualStyleBackColor = true;
      button1.Click += button1_Click;
      // 
      // button3
      // 
      button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      button3.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
      button3.Location = new Point(6, 52);
      button3.Name = "button3";
      button3.Size = new Size(43, 21);
      button3.TabIndex = 6;
      button3.Text = "paste";
      button3.UseVisualStyleBackColor = true;
      button3.Click += button3_Click;
      // 
      // button2
      // 
      button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      button2.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
      button2.Location = new Point(6, 28);
      button2.Name = "button2";
      button2.Size = new Size(43, 21);
      button2.TabIndex = 5;
      button2.Text = "copy";
      button2.UseVisualStyleBackColor = true;
      button2.Click += button2_Click;
      // 
      // MemoPropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(button3);
      Controls.Add(button2);
      Controls.Add(button1);
      Controls.Add(textBox1);
      Controls.Add(lbName);
      Name = "MemoPropertyEditor";
      Size = new Size(420, 100);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label lbName;
    private TextBox textBox1;
    private Button button1;
    private Button button3;
    private Button button2;
  }
}
