namespace TheLoomApp {
  partial class ImportOrgDocsDialog {
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
      textBox1 = new TextBox();
      label1 = new Label();
      button1 = new Button();
      lbItemToImport = new CheckedListBox();
      label2 = new Label();
      button2 = new Button();
      button3 = new Button();
      button4 = new Button();
      button5 = new Button();
      SuspendLayout();
      // 
      // textBox1
      // 
      textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      textBox1.Location = new Point(46, 49);
      textBox1.Margin = new Padding(3, 4, 3, 4);
      textBox1.Name = "textBox1";
      textBox1.Size = new Size(496, 26);
      textBox1.TabIndex = 0;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(23, 26);
      label1.Name = "label1";
      label1.Size = new Size(250, 19);
      label1.TabIndex = 1;
      label1.Text = "Choose the Organization.md to import:";
      // 
      // button1
      // 
      button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      button1.Location = new Point(548, 49);
      button1.Name = "button1";
      button1.Size = new Size(75, 27);
      button1.TabIndex = 2;
      button1.Text = "Browse";
      button1.UseVisualStyleBackColor = true;
      button1.Click += button1_Click;
      // 
      // lbItemToImport
      // 
      lbItemToImport.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      lbItemToImport.FormattingEnabled = true;
      lbItemToImport.Location = new Point(129, 122);
      lbItemToImport.MinimumSize = new Size(396, 96);
      lbItemToImport.Name = "lbItemToImport";
      lbItemToImport.Size = new Size(503, 88);
      lbItemToImport.TabIndex = 3;
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(21, 122);
      label2.Name = "label2";
      label2.Size = new Size(102, 19);
      label2.TabIndex = 4;
      label2.Text = "Docs to Import";
      // 
      // button2
      // 
      button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      button2.DialogResult = DialogResult.Cancel;
      button2.Location = new Point(335, 254);
      button2.Name = "button2";
      button2.Size = new Size(80, 31);
      button2.TabIndex = 5;
      button2.Text = "Close";
      button2.UseVisualStyleBackColor = true;
      // 
      // button3
      // 
      button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      button3.DialogResult = DialogResult.OK;
      button3.Location = new Point(252, 253);
      button3.Name = "button3";
      button3.Size = new Size(77, 31);
      button3.TabIndex = 6;
      button3.Text = "Import";
      button3.UseVisualStyleBackColor = true;
      button3.Click += button3_Click;
      // 
      // button4
      // 
      button4.Location = new Point(526, 85);
      button4.Name = "button4";
      button4.Size = new Size(106, 31);
      button4.TabIndex = 7;
      button4.Text = "Scan for docs";
      button4.UseVisualStyleBackColor = true;
      button4.Click += button4_Click;
      // 
      // button5
      // 
      button5.Location = new Point(17, 155);
      button5.Name = "button5";
      button5.Size = new Size(106, 31);
      button5.TabIndex = 8;
      button5.Text = "Select All";
      button5.UseVisualStyleBackColor = true;
      button5.Click += button5_Click;
      // 
      // ImportOrgDocsDialog
      // 
      AutoScaleDimensions = new SizeF(8F, 19F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(646, 313);
      Controls.Add(button5);
      Controls.Add(button4);
      Controls.Add(button3);
      Controls.Add(button2);
      Controls.Add(label2);
      Controls.Add(lbItemToImport);
      Controls.Add(button1);
      Controls.Add(label1);
      Controls.Add(textBox1);
      Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
      Margin = new Padding(3, 4, 3, 4);
      MinimumSize = new Size(556, 269);
      Name = "ImportOrgDocsDialog";
      Text = "ImportOrgDocsDialog";
      Shown += ImportOrgDocsDialog_Shown;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private TextBox textBox1;
    private Label label1;
    private Button button1;
    private CheckedListBox lbItemToImport;
    private Label label2;
    private Button button2;
    private Button button3;
    private Button button4;
    private Button button5;
  }
}