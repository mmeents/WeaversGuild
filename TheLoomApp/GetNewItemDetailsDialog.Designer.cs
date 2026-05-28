namespace TheLoomApp {
  partial class GetNewItemDetailsDialog {
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
      lbAddTarget = new Label();
      edName = new TextBox();
      lbNewItemName = new Label();
      btnCancel = new Button();
      btnOk = new Button();
      cbNewFileType = new ComboBox();
      lbItemType = new Label();
      cbDataType = new ComboBox();
      cbItemLookup = new ComboBox();
      lbWhich = new Label();
      cbIsAsync = new CheckBox();
      edDbTableName = new TextBox();
      lbDbTableName = new Label();
      SuspendLayout();
      // 
      // lbAddTarget
      // 
      lbAddTarget.AutoSize = true;
      lbAddTarget.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lbAddTarget.Location = new Point(21, 19);
      lbAddTarget.Name = "lbAddTarget";
      lbAddTarget.Size = new Size(93, 21);
      lbAddTarget.TabIndex = 0;
      lbAddTarget.Text = "lbAddTarget";
      // 
      // edName
      // 
      edName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edName.BorderStyle = BorderStyle.FixedSingle;
      edName.Font = new Font("Segoe UI", 10.2F);
      edName.Location = new Point(141, 85);
      edName.Margin = new Padding(3, 4, 3, 4);
      edName.Name = "edName";
      edName.Size = new Size(469, 26);
      edName.TabIndex = 1;
      // 
      // lbNewItemName
      // 
      lbNewItemName.AutoSize = true;
      lbNewItemName.Font = new Font("Segoe UI", 10.2F);
      lbNewItemName.Location = new Point(79, 89);
      lbNewItemName.Name = "lbNewItemName";
      lbNewItemName.Size = new Size(48, 19);
      lbNewItemName.TabIndex = 2;
      lbNewItemName.Text = "Name:";
      // 
      // btnCancel
      // 
      btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      btnCancel.DialogResult = DialogResult.Cancel;
      btnCancel.Location = new Point(354, 221);
      btnCancel.Margin = new Padding(3, 4, 3, 4);
      btnCancel.Name = "btnCancel";
      btnCancel.Size = new Size(86, 29);
      btnCancel.TabIndex = 3;
      btnCancel.Text = "Cancel";
      btnCancel.UseVisualStyleBackColor = true;
      // 
      // btnOk
      // 
      btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      btnOk.DialogResult = DialogResult.OK;
      btnOk.Location = new Point(235, 221);
      btnOk.Margin = new Padding(3, 4, 3, 4);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(86, 29);
      btnOk.TabIndex = 4;
      btnOk.Text = "Create";
      btnOk.UseVisualStyleBackColor = true;
      // 
      // cbNewFileType
      // 
      cbNewFileType.FormattingEnabled = true;
      cbNewFileType.Items.AddRange(new object[] { ".md file", ".html file", ".json file" });
      cbNewFileType.Location = new Point(339, 119);
      cbNewFileType.Margin = new Padding(3, 4, 3, 4);
      cbNewFileType.Name = "cbNewFileType";
      cbNewFileType.Size = new Size(138, 27);
      cbNewFileType.TabIndex = 5;
      cbNewFileType.Text = ".md file";
      cbNewFileType.SelectedIndexChanged += cbNewFileType_SelectedIndexChanged;
      // 
      // lbItemType
      // 
      lbItemType.AutoSize = true;
      lbItemType.Location = new Point(82, 122);
      lbItemType.Name = "lbItemType";
      lbItemType.Size = new Size(40, 19);
      lbItemType.TabIndex = 6;
      lbItemType.Text = "Type:";
      // 
      // cbDataType
      // 
      cbDataType.FormattingEnabled = true;
      cbDataType.Location = new Point(141, 119);
      cbDataType.Name = "cbDataType";
      cbDataType.Size = new Size(138, 27);
      cbDataType.TabIndex = 7;
      cbDataType.SelectedIndexChanged += cbDataType_SelectedIndexChanged;
      // 
      // cbItemLookup
      // 
      cbItemLookup.FormattingEnabled = true;
      cbItemLookup.Location = new Point(141, 152);
      cbItemLookup.Name = "cbItemLookup";
      cbItemLookup.Size = new Size(211, 27);
      cbItemLookup.TabIndex = 8;
      // 
      // lbWhich
      // 
      lbWhich.AutoSize = true;
      lbWhich.Location = new Point(79, 155);
      lbWhich.Name = "lbWhich";
      lbWhich.Size = new Size(47, 19);
      lbWhich.TabIndex = 9;
      lbWhich.Text = "Which";
      // 
      // cbIsAsync
      // 
      cbIsAsync.AutoSize = true;
      cbIsAsync.Location = new Point(82, 55);
      cbIsAsync.Name = "cbIsAsync";
      cbIsAsync.Size = new Size(130, 23);
      cbIsAsync.TabIndex = 10;
      cbIsAsync.Text = "Method is Async";
      cbIsAsync.UseVisualStyleBackColor = true;
      cbIsAsync.CheckedChanged += cbIsAsync_CheckedChanged;
      // 
      // edDbTableName
      // 
      edDbTableName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edDbTableName.BorderStyle = BorderStyle.FixedSingle;
      edDbTableName.Location = new Point(141, 188);
      edDbTableName.Name = "edDbTableName";
      edDbTableName.Size = new Size(469, 26);
      edDbTableName.TabIndex = 11;
      // 
      // lbDbTableName
      // 
      lbDbTableName.AutoSize = true;
      lbDbTableName.Location = new Point(21, 190);
      lbDbTableName.Name = "lbDbTableName";
      lbDbTableName.Size = new Size(101, 19);
      lbDbTableName.TabIndex = 12;
      lbDbTableName.Text = "Db Table Name";
      // 
      // GetNewItemDetailsDialog
      // 
      AutoScaleDimensions = new SizeF(8F, 19F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(640, 267);
      Controls.Add(lbDbTableName);
      Controls.Add(edDbTableName);
      Controls.Add(cbIsAsync);
      Controls.Add(lbWhich);
      Controls.Add(cbItemLookup);
      Controls.Add(cbDataType);
      Controls.Add(lbItemType);
      Controls.Add(cbNewFileType);
      Controls.Add(btnOk);
      Controls.Add(btnCancel);
      Controls.Add(lbNewItemName);
      Controls.Add(edName);
      Controls.Add(lbAddTarget);
      Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
      Margin = new Padding(3, 4, 3, 4);
      Name = "GetNewItemDetailsDialog";
      StartPosition = FormStartPosition.CenterParent;
      Text = "Adding";
      TopMost = true;
      Load += GetNewItemDetailsDialog_Load;
      Shown += GetNewItemDetailsDialog_Shown;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label lbAddTarget;
    private TextBox edName;
    private Label lbNewItemName;
    private Button btnCancel;
    private Button btnOk;
    private ComboBox cbNewFileType;
    private Label lbItemType;
    private ComboBox cbDataType;
    private ComboBox cbItemLookup;
    private Label lbWhich;
    private CheckBox cbIsAsync;
    private TextBox edDbTableName;
    private Label lbDbTableName;
  }
}