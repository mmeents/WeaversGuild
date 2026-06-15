namespace TheLoomApp.Editors {
  partial class CmdPickerPropertyEditor {
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
      edCmdChkList = new ListView();
      SuspendLayout();
      // 
      // lbName
      // 
      lbName.AutoSize = true;
      lbName.Location = new Point(3, 6);
      lbName.Name = "lbName";
      lbName.Size = new Size(38, 15);
      lbName.TabIndex = 1;
      lbName.Text = "label1";
      // 
      // edCmdChkList
      // 
      edCmdChkList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edCmdChkList.CheckBoxes = true;
      edCmdChkList.Location = new Point(102, 6);
      edCmdChkList.Name = "edCmdChkList";
      edCmdChkList.Size = new Size(330, 110);
      edCmdChkList.TabIndex = 3;
      edCmdChkList.UseCompatibleStateImageBehavior = false;
      edCmdChkList.View = View.List;
      edCmdChkList.ItemChecked += edCmdChkList_ItemChecked;
      // 
      // CmdPickerPropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(edCmdChkList);
      Controls.Add(lbName);
      Name = "CmdPickerPropertyEditor";
      Size = new Size(435, 122);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private Label lbName;
    private ListView edCmdChkList;
  }
}
