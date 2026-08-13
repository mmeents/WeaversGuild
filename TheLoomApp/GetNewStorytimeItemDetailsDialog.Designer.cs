namespace TheLoomApp {
  partial class GetNewStorytimeItemDetailsDialog {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetNewStorytimeItemDetailsDialog));
      lbNewItemName = new Label();
      edName = new TextBox();
      lbAddTarget = new Label();
      edDescription = new FastColoredTextBoxNS.FastColoredTextBox();
      lbDescription = new Label();
      btnOk = new Button();
      btnCancel = new Button();
      lbTargetSceneCount = new Label();
      edTargetSceneCount = new NumericUpDown();
      lbPov = new Label();
      cbPov = new ComboBox();
      lbEntry = new Label();
      edEntryState = new TextBox();
      lbExit = new Label();
      edExitState = new TextBox();
      ((System.ComponentModel.ISupportInitialize)edDescription).BeginInit();
      ((System.ComponentModel.ISupportInitialize)edTargetSceneCount).BeginInit();
      SuspendLayout();
      // 
      // lbNewItemName
      // 
      lbNewItemName.AutoSize = true;
      lbNewItemName.Font = new Font("Segoe UI", 10.2F);
      lbNewItemName.Location = new Point(83, 58);
      lbNewItemName.Name = "lbNewItemName";
      lbNewItemName.Size = new Size(48, 19);
      lbNewItemName.TabIndex = 5;
      lbNewItemName.Text = "Name:";
      // 
      // edName
      // 
      edName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edName.BorderStyle = BorderStyle.FixedSingle;
      edName.Font = new Font("Segoe UI", 10.2F);
      edName.Location = new Point(139, 56);
      edName.Margin = new Padding(3, 4, 3, 4);
      edName.Name = "edName";
      edName.Size = new Size(469, 26);
      edName.TabIndex = 4;
      // 
      // lbAddTarget
      // 
      lbAddTarget.AutoSize = true;
      lbAddTarget.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lbAddTarget.Location = new Point(21, 19);
      lbAddTarget.Name = "lbAddTarget";
      lbAddTarget.Size = new Size(93, 21);
      lbAddTarget.TabIndex = 3;
      lbAddTarget.Text = "lbAddTarget";
      // 
      // edDescription
      // 
      edDescription.AutoCompleteBracketsList = new char[]
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
      edDescription.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
      edDescription.AutoScrollMinSize = new Size(27, 14);
      edDescription.BackBrush = null;
      edDescription.CharHeight = 14;
      edDescription.CharWidth = 8;
      edDescription.DefaultMarkerSize = 8;
      edDescription.DisabledColor = Color.FromArgb(100, 180, 180, 180);
      edDescription.FindForm = null;
      edDescription.GoToForm = null;
      edDescription.Hotkeys = resources.GetString("edDescription.Hotkeys");
      edDescription.IsReplaceMode = false;
      edDescription.Location = new Point(139, 233);
      edDescription.Name = "edDescription";
      edDescription.Paddings = new Padding(0);
      edDescription.ReplaceForm = null;
      edDescription.SelectionColor = Color.FromArgb(60, 0, 0, 255);
      edDescription.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("edDescription.ServiceColors");
      edDescription.Size = new Size(469, 96);
      edDescription.TabIndex = 6;
      edDescription.Zoom = 100;
      // 
      // lbDescription
      // 
      lbDescription.AutoSize = true;
      lbDescription.Font = new Font("Segoe UI", 10.2F);
      lbDescription.Location = new Point(46, 233);
      lbDescription.Name = "lbDescription";
      lbDescription.Size = new Size(81, 19);
      lbDescription.TabIndex = 7;
      lbDescription.Text = "Description:";
      // 
      // btnOk
      // 
      btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      btnOk.DialogResult = DialogResult.OK;
      btnOk.Location = new Point(259, 356);
      btnOk.Margin = new Padding(3, 4, 3, 4);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(86, 29);
      btnOk.TabIndex = 9;
      btnOk.Text = "Create";
      btnOk.UseVisualStyleBackColor = true;
      btnOk.Click += btnOk_Click;
      // 
      // btnCancel
      // 
      btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      btnCancel.DialogResult = DialogResult.Cancel;
      btnCancel.Location = new Point(378, 356);
      btnCancel.Margin = new Padding(3, 4, 3, 4);
      btnCancel.Name = "btnCancel";
      btnCancel.Size = new Size(86, 29);
      btnCancel.TabIndex = 8;
      btnCancel.Text = "Cancel";
      btnCancel.UseVisualStyleBackColor = true;
      // 
      // lbTargetSceneCount
      // 
      lbTargetSceneCount.AutoSize = true;
      lbTargetSceneCount.Font = new Font("Segoe UI", 10.2F);
      lbTargetSceneCount.Location = new Point(355, 94);
      lbTargetSceneCount.Name = "lbTargetSceneCount";
      lbTargetSceneCount.Size = new Size(127, 19);
      lbTargetSceneCount.TabIndex = 12;
      lbTargetSceneCount.Text = "Target Scene Count";
      // 
      // edTargetSceneCount
      // 
      edTargetSceneCount.Font = new Font("Segoe UI", 10.2F);
      edTargetSceneCount.Location = new Point(488, 92);
      edTargetSceneCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
      edTargetSceneCount.Name = "edTargetSceneCount";
      edTargetSceneCount.Size = new Size(120, 26);
      edTargetSceneCount.TabIndex = 13;
      edTargetSceneCount.Value = new decimal(new int[] { 5, 0, 0, 0 });
      // 
      // lbPov
      // 
      lbPov.AutoSize = true;
      lbPov.Font = new Font("Segoe UI", 10.2F);
      lbPov.Location = new Point(51, 97);
      lbPov.Name = "lbPov";
      lbPov.Size = new Size(82, 19);
      lbPov.TabIndex = 15;
      lbPov.Text = "Pov Default:";
      // 
      // cbPov
      // 
      cbPov.Font = new Font("Segoe UI", 10.2F);
      cbPov.FormattingEnabled = true;
      cbPov.Location = new Point(139, 94);
      cbPov.Name = "cbPov";
      cbPov.Size = new Size(175, 27);
      cbPov.TabIndex = 14;
      // 
      // lbEntry
      // 
      lbEntry.AutoSize = true;
      lbEntry.Font = new Font("Segoe UI", 10.2F);
      lbEntry.Location = new Point(51, 130);
      lbEntry.Name = "lbEntry";
      lbEntry.Size = new Size(79, 19);
      lbEntry.TabIndex = 17;
      lbEntry.Text = "Entry State:";
      // 
      // edEntryState
      // 
      edEntryState.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edEntryState.BorderStyle = BorderStyle.FixedSingle;
      edEntryState.Font = new Font("Segoe UI", 10.2F);
      edEntryState.Location = new Point(139, 128);
      edEntryState.Margin = new Padding(3, 4, 3, 4);
      edEntryState.Multiline = true;
      edEntryState.Name = "edEntryState";
      edEntryState.Size = new Size(469, 45);
      edEntryState.TabIndex = 16;
      // 
      // lbExit
      // 
      lbExit.AutoSize = true;
      lbExit.Font = new Font("Segoe UI", 10.2F);
      lbExit.Location = new Point(59, 183);
      lbExit.Name = "lbExit";
      lbExit.Size = new Size(68, 19);
      lbExit.TabIndex = 19;
      lbExit.Text = "Exit State:";
      // 
      // edExitState
      // 
      edExitState.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edExitState.BorderStyle = BorderStyle.FixedSingle;
      edExitState.Font = new Font("Segoe UI", 10.2F);
      edExitState.Location = new Point(139, 181);
      edExitState.Margin = new Padding(3, 4, 3, 4);
      edExitState.Multiline = true;
      edExitState.Name = "edExitState";
      edExitState.Size = new Size(469, 45);
      edExitState.TabIndex = 18;
      // 
      // GetNewStorytimeItemDetailsDialog
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(729, 398);
      Controls.Add(lbExit);
      Controls.Add(edExitState);
      Controls.Add(lbEntry);
      Controls.Add(edEntryState);
      Controls.Add(lbPov);
      Controls.Add(cbPov);
      Controls.Add(edTargetSceneCount);
      Controls.Add(lbTargetSceneCount);
      Controls.Add(btnOk);
      Controls.Add(btnCancel);
      Controls.Add(lbDescription);
      Controls.Add(edDescription);
      Controls.Add(lbNewItemName);
      Controls.Add(edName);
      Controls.Add(lbAddTarget);
      Name = "GetNewStorytimeItemDetailsDialog";
      StartPosition = FormStartPosition.CenterParent;
      Text = "GetNewStorytimeItemDetailsDialog";
      ((System.ComponentModel.ISupportInitialize)edDescription).EndInit();
      ((System.ComponentModel.ISupportInitialize)edTargetSceneCount).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label lbNewItemName;
    private TextBox edName;
    private Label lbAddTarget;
    private FastColoredTextBoxNS.FastColoredTextBox edDescription;
    private Label lbDescription;
    private Button btnOk;
    private Button btnCancel;
    private Label lbTargetSceneCount;
    private NumericUpDown edTargetSceneCount;
    private Label lbPov;
    private ComboBox cbPov;
    private Label lbEntry;
    private TextBox edEntryState;
    private Label lbExit;
    private TextBox edExitState;
  }
}