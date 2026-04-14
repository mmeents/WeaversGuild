namespace TheLoomApp {
  partial class Form1 {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      splitContainer1 = new SplitContainer();
      splitContainer2 = new SplitContainer();
      tvKb = new TreeView();
      cmsTreeMenus = new ContextMenuStrip(components);
      miReloadTree = new ToolStripMenuItem();
      toolStripSeparator1 = new ToolStripSeparator();
      miAddProjectRoot = new ToolStripMenuItem();
      miAddSubProject = new ToolStripMenuItem();
      toolStripSeparator2 = new ToolStripSeparator();
      miDeleteItem = new ToolStripMenuItem();
      ilTreeImages = new ImageList(components);
      splitContainer3 = new SplitContainer();
      tabControl1 = new TabControl();
      tpSettings = new TabPage();
      btnShowErrors = new Button();
      btnCancelAppDefaultF = new Button();
      btnSaveDefaultFolder = new Button();
      btnAppDefaultFolderBrowse = new Button();
      label2 = new Label();
      edAppDefaultFolder = new TextBox();
      tpItem = new TabPage();
      tabControl2 = new TabControl();
      tpItemDesc = new TabPage();
      edItemDesc = new TextBox();
      tpData = new TabPage();
      edItemData = new TextBox();
      btnArchive = new Button();
      btnCancelRelation = new Button();
      btnUpdateRelation = new Button();
      label7 = new Label();
      edRank = new NumericUpDown();
      label4 = new Label();
      cbRelRelation = new ComboBox();
      lbRelationId = new Label();
      lbRelItemName = new Label();
      label1 = new Label();
      lbItemId = new Label();
      btnAbortItem = new Button();
      btnUpdateItem = new Button();
      lbType = new Label();
      lbItemName = new Label();
      edItemType = new ComboBox();
      edItemName = new TextBox();
      tbErrorOut = new TextBox();
      tsErrorPopup = new ToolStrip();
      toolStripLabel1 = new ToolStripLabel();
      tsBtnDismiss = new ToolStripButton();
      splitter1 = new Splitter();
      ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
      splitContainer2.Panel2.SuspendLayout();
      splitContainer2.SuspendLayout();
      cmsTreeMenus.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
      splitContainer3.Panel1.SuspendLayout();
      splitContainer3.Panel2.SuspendLayout();
      splitContainer3.SuspendLayout();
      tabControl1.SuspendLayout();
      tpSettings.SuspendLayout();
      tpItem.SuspendLayout();
      tabControl2.SuspendLayout();
      tpItemDesc.SuspendLayout();
      tpData.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)edRank).BeginInit();
      tsErrorPopup.SuspendLayout();
      SuspendLayout();
      // 
      // splitContainer1
      // 
      splitContainer1.Dock = DockStyle.Fill;
      splitContainer1.Location = new Point(0, 0);
      splitContainer1.Margin = new Padding(3, 2, 3, 2);
      splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(splitContainer2);
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(splitContainer3);
      splitContainer1.Panel2.Controls.Add(splitter1);
      splitContainer1.Size = new Size(896, 632);
      splitContainer1.SplitterDistance = 265;
      splitContainer1.TabIndex = 0;
      // 
      // splitContainer2
      // 
      splitContainer2.Dock = DockStyle.Fill;
      splitContainer2.Location = new Point(0, 0);
      splitContainer2.Margin = new Padding(3, 2, 3, 2);
      splitContainer2.Name = "splitContainer2";
      splitContainer2.Orientation = Orientation.Horizontal;
      // 
      // splitContainer2.Panel2
      // 
      splitContainer2.Panel2.Controls.Add(tvKb);
      splitContainer2.Size = new Size(265, 632);
      splitContainer2.SplitterDistance = 60;
      splitContainer2.SplitterWidth = 3;
      splitContainer2.TabIndex = 0;
      // 
      // tvKb
      // 
      tvKb.ContextMenuStrip = cmsTreeMenus;
      tvKb.Dock = DockStyle.Fill;
      tvKb.ImageIndex = 0;
      tvKb.ImageList = ilTreeImages;
      tvKb.Location = new Point(0, 0);
      tvKb.Margin = new Padding(3, 2, 3, 2);
      tvKb.Name = "tvKb";
      tvKb.SelectedImageIndex = 0;
      tvKb.Size = new Size(265, 569);
      tvKb.TabIndex = 0;
      tvKb.AfterCollapse += tvKb_AfterCollapse;
      tvKb.BeforeExpand += tvKb_BeforeExpand;
      tvKb.AfterSelect += tvKb_AfterSelect;
      // 
      // cmsTreeMenus
      // 
      cmsTreeMenus.ImageScalingSize = new Size(20, 20);
      cmsTreeMenus.Items.AddRange(new ToolStripItem[] { miReloadTree, toolStripSeparator1, miAddProjectRoot, miAddSubProject, toolStripSeparator2, miDeleteItem });
      cmsTreeMenus.Name = "cmsTreeMenus";
      cmsTreeMenus.Size = new Size(165, 104);
      cmsTreeMenus.Opening += cmsTreeMenus_Opening;
      // 
      // miReloadTree
      // 
      miReloadTree.Name = "miReloadTree";
      miReloadTree.Size = new Size(164, 22);
      miReloadTree.Text = "Reload Projects";
      miReloadTree.Click += miReloadTree_Click;
      // 
      // toolStripSeparator1
      // 
      toolStripSeparator1.Name = "toolStripSeparator1";
      toolStripSeparator1.Size = new Size(161, 6);
      // 
      // miAddProjectRoot
      // 
      miAddProjectRoot.Name = "miAddProjectRoot";
      miAddProjectRoot.Size = new Size(164, 22);
      miAddProjectRoot.Text = "Add Project Root";
      miAddProjectRoot.Click += miAddProjectRoot_Click;
      // 
      // miAddSubProject
      // 
      miAddSubProject.Name = "miAddSubProject";
      miAddSubProject.Size = new Size(164, 22);
      miAddSubProject.Text = "Add Project";
      miAddSubProject.Click += miAddSubProject_Click;
      // 
      // toolStripSeparator2
      // 
      toolStripSeparator2.Name = "toolStripSeparator2";
      toolStripSeparator2.Size = new Size(161, 6);
      // 
      // miDeleteItem
      // 
      miDeleteItem.Name = "miDeleteItem";
      miDeleteItem.Size = new Size(164, 22);
      miDeleteItem.Text = "Delete Selected";
      miDeleteItem.Click += miDeleteItem_Click;
      // 
      // ilTreeImages
      // 
      ilTreeImages.ColorDepth = ColorDepth.Depth32Bit;
      ilTreeImages.ImageStream = (ImageListStreamer)resources.GetObject("ilTreeImages.ImageStream");
      ilTreeImages.TransparentColor = Color.Transparent;
      ilTreeImages.Images.SetKeyName(0, "transparent.png");
      ilTreeImages.Images.SetKeyName(1, "folder.png");
      // 
      // splitContainer3
      // 
      splitContainer3.Dock = DockStyle.Fill;
      splitContainer3.Location = new Point(4, 0);
      splitContainer3.Margin = new Padding(3, 2, 3, 2);
      splitContainer3.Name = "splitContainer3";
      splitContainer3.Orientation = Orientation.Horizontal;
      // 
      // splitContainer3.Panel1
      // 
      splitContainer3.Panel1.Controls.Add(tabControl1);
      // 
      // splitContainer3.Panel2
      // 
      splitContainer3.Panel2.Controls.Add(tbErrorOut);
      splitContainer3.Panel2.Controls.Add(tsErrorPopup);
      splitContainer3.Size = new Size(623, 632);
      splitContainer3.SplitterDistance = 513;
      splitContainer3.SplitterWidth = 3;
      splitContainer3.TabIndex = 1;
      // 
      // tabControl1
      // 
      tabControl1.Controls.Add(tpSettings);
      tabControl1.Controls.Add(tpItem);
      tabControl1.Dock = DockStyle.Fill;
      tabControl1.Location = new Point(0, 0);
      tabControl1.Margin = new Padding(3, 2, 3, 2);
      tabControl1.Name = "tabControl1";
      tabControl1.SelectedIndex = 0;
      tabControl1.Size = new Size(623, 513);
      tabControl1.TabIndex = 0;
      // 
      // tpSettings
      // 
      tpSettings.Controls.Add(btnShowErrors);
      tpSettings.Controls.Add(btnCancelAppDefaultF);
      tpSettings.Controls.Add(btnSaveDefaultFolder);
      tpSettings.Controls.Add(btnAppDefaultFolderBrowse);
      tpSettings.Controls.Add(label2);
      tpSettings.Controls.Add(edAppDefaultFolder);
      tpSettings.Location = new Point(4, 24);
      tpSettings.Margin = new Padding(3, 2, 3, 2);
      tpSettings.Name = "tpSettings";
      tpSettings.Padding = new Padding(3, 2, 3, 2);
      tpSettings.Size = new Size(615, 485);
      tpSettings.TabIndex = 0;
      tpSettings.Text = "Settings";
      tpSettings.UseVisualStyleBackColor = true;
      // 
      // btnShowErrors
      // 
      btnShowErrors.Location = new Point(25, 53);
      btnShowErrors.Name = "btnShowErrors";
      btnShowErrors.Size = new Size(80, 23);
      btnShowErrors.TabIndex = 5;
      btnShowErrors.Text = "Show Errors";
      btnShowErrors.UseVisualStyleBackColor = true;
      btnShowErrors.Click += btnShowErrors_Click;
      // 
      // btnCancelAppDefaultF
      // 
      btnCancelAppDefaultF.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnCancelAppDefaultF.Location = new Point(571, 19);
      btnCancelAppDefaultF.Name = "btnCancelAppDefaultF";
      btnCancelAppDefaultF.Size = new Size(36, 23);
      btnCancelAppDefaultF.TabIndex = 4;
      btnCancelAppDefaultF.Text = "✕";
      btnCancelAppDefaultF.UseVisualStyleBackColor = true;
      btnCancelAppDefaultF.Click += btnCancelAppDefaultF_Click;
      // 
      // btnSaveDefaultFolder
      // 
      btnSaveDefaultFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnSaveDefaultFolder.Location = new Point(534, 19);
      btnSaveDefaultFolder.Name = "btnSaveDefaultFolder";
      btnSaveDefaultFolder.Size = new Size(36, 23);
      btnSaveDefaultFolder.TabIndex = 3;
      btnSaveDefaultFolder.Text = "✓";
      btnSaveDefaultFolder.UseVisualStyleBackColor = true;
      btnSaveDefaultFolder.Click += btnSaveDefaultFolder_Click;
      // 
      // btnAppDefaultFolderBrowse
      // 
      btnAppDefaultFolderBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnAppDefaultFolderBrowse.Location = new Point(498, 19);
      btnAppDefaultFolderBrowse.Name = "btnAppDefaultFolderBrowse";
      btnAppDefaultFolderBrowse.Size = new Size(36, 23);
      btnAppDefaultFolderBrowse.TabIndex = 2;
      btnAppDefaultFolderBrowse.Text = "↗";
      btnAppDefaultFolderBrowse.UseVisualStyleBackColor = true;
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(22, 21);
      label2.Name = "label2";
      label2.Size = new Size(106, 15);
      label2.TabIndex = 1;
      label2.Text = "App Default Folder";
      // 
      // edAppDefaultFolder
      // 
      edAppDefaultFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edAppDefaultFolder.Location = new Point(146, 19);
      edAppDefaultFolder.Margin = new Padding(3, 2, 3, 2);
      edAppDefaultFolder.Name = "edAppDefaultFolder";
      edAppDefaultFolder.Size = new Size(348, 23);
      edAppDefaultFolder.TabIndex = 0;
      edAppDefaultFolder.TextChanged += edAppDefaultFolder_TextChanged;
      // 
      // tpItem
      // 
      tpItem.Controls.Add(tabControl2);
      tpItem.Controls.Add(btnArchive);
      tpItem.Controls.Add(btnCancelRelation);
      tpItem.Controls.Add(btnUpdateRelation);
      tpItem.Controls.Add(label7);
      tpItem.Controls.Add(edRank);
      tpItem.Controls.Add(label4);
      tpItem.Controls.Add(cbRelRelation);
      tpItem.Controls.Add(lbRelationId);
      tpItem.Controls.Add(lbRelItemName);
      tpItem.Controls.Add(label1);
      tpItem.Controls.Add(lbItemId);
      tpItem.Controls.Add(btnAbortItem);
      tpItem.Controls.Add(btnUpdateItem);
      tpItem.Controls.Add(lbType);
      tpItem.Controls.Add(lbItemName);
      tpItem.Controls.Add(edItemType);
      tpItem.Controls.Add(edItemName);
      tpItem.Location = new Point(4, 24);
      tpItem.Margin = new Padding(3, 2, 3, 2);
      tpItem.Name = "tpItem";
      tpItem.Padding = new Padding(3, 2, 3, 2);
      tpItem.Size = new Size(615, 485);
      tpItem.TabIndex = 1;
      tpItem.Text = "Details";
      tpItem.UseVisualStyleBackColor = true;
      // 
      // tabControl2
      // 
      tabControl2.Controls.Add(tpItemDesc);
      tabControl2.Controls.Add(tpData);
      tabControl2.Dock = DockStyle.Bottom;
      tabControl2.Location = new Point(3, 226);
      tabControl2.Margin = new Padding(3, 2, 3, 2);
      tabControl2.Name = "tabControl2";
      tabControl2.SelectedIndex = 0;
      tabControl2.Size = new Size(609, 257);
      tabControl2.TabIndex = 45;
      // 
      // tpItemDesc
      // 
      tpItemDesc.Controls.Add(edItemDesc);
      tpItemDesc.Location = new Point(4, 24);
      tpItemDesc.Margin = new Padding(3, 2, 3, 2);
      tpItemDesc.Name = "tpItemDesc";
      tpItemDesc.Padding = new Padding(3, 2, 3, 2);
      tpItemDesc.Size = new Size(601, 229);
      tpItemDesc.TabIndex = 0;
      tpItemDesc.Text = "Description";
      tpItemDesc.UseVisualStyleBackColor = true;
      // 
      // edItemDesc
      // 
      edItemDesc.Dock = DockStyle.Fill;
      edItemDesc.Location = new Point(3, 2);
      edItemDesc.Margin = new Padding(3, 2, 3, 2);
      edItemDesc.Multiline = true;
      edItemDesc.Name = "edItemDesc";
      edItemDesc.ScrollBars = ScrollBars.Both;
      edItemDesc.Size = new Size(595, 225);
      edItemDesc.TabIndex = 28;
      edItemDesc.TextChanged += edItemName_TextChanged;
      // 
      // tpData
      // 
      tpData.Controls.Add(edItemData);
      tpData.Location = new Point(4, 24);
      tpData.Margin = new Padding(3, 2, 3, 2);
      tpData.Name = "tpData";
      tpData.Padding = new Padding(3, 2, 3, 2);
      tpData.Size = new Size(601, 229);
      tpData.TabIndex = 1;
      tpData.Text = "Json Data";
      tpData.UseVisualStyleBackColor = true;
      // 
      // edItemData
      // 
      edItemData.Dock = DockStyle.Fill;
      edItemData.Location = new Point(3, 2);
      edItemData.Margin = new Padding(3, 2, 3, 2);
      edItemData.Multiline = true;
      edItemData.Name = "edItemData";
      edItemData.ScrollBars = ScrollBars.Both;
      edItemData.Size = new Size(595, 225);
      edItemData.TabIndex = 27;
      edItemData.TextChanged += edItemName_TextChanged;
      // 
      // btnArchive
      // 
      btnArchive.Anchor = AnchorStyles.Top;
      btnArchive.Location = new Point(545, 148);
      btnArchive.Margin = new Padding(3, 2, 3, 2);
      btnArchive.Name = "btnArchive";
      btnArchive.Size = new Size(66, 21);
      btnArchive.TabIndex = 44;
      btnArchive.Text = "Archive";
      btnArchive.UseVisualStyleBackColor = true;
      btnArchive.Click += btnArchive_Click;
      // 
      // btnCancelRelation
      // 
      btnCancelRelation.Location = new Point(291, 74);
      btnCancelRelation.Margin = new Padding(3, 2, 3, 2);
      btnCancelRelation.Name = "btnCancelRelation";
      btnCancelRelation.Size = new Size(66, 21);
      btnCancelRelation.TabIndex = 43;
      btnCancelRelation.Text = "Cancel";
      btnCancelRelation.UseVisualStyleBackColor = true;
      btnCancelRelation.Click += btnCancelRelation_Click;
      // 
      // btnUpdateRelation
      // 
      btnUpdateRelation.Location = new Point(220, 73);
      btnUpdateRelation.Margin = new Padding(3, 2, 3, 2);
      btnUpdateRelation.Name = "btnUpdateRelation";
      btnUpdateRelation.Size = new Size(66, 21);
      btnUpdateRelation.TabIndex = 42;
      btnUpdateRelation.Text = "Update";
      btnUpdateRelation.UseVisualStyleBackColor = true;
      btnUpdateRelation.Click += btnUpdateRelation_Click;
      // 
      // label7
      // 
      label7.AutoSize = true;
      label7.Location = new Point(30, 76);
      label7.Name = "label7";
      label7.Size = new Size(66, 15);
      label7.TabIndex = 41;
      label7.Text = "Rank Order";
      // 
      // edRank
      // 
      edRank.Location = new Point(108, 74);
      edRank.Margin = new Padding(3, 2, 3, 2);
      edRank.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
      edRank.Name = "edRank";
      edRank.Size = new Size(88, 23);
      edRank.TabIndex = 40;
      edRank.ValueChanged += edRank_ValueChanged;
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new Point(43, 100);
      label4.Name = "label4";
      label4.Size = new Size(50, 15);
      label4.TabIndex = 39;
      label4.Text = "Relation";
      // 
      // cbRelRelation
      // 
      cbRelRelation.DisplayMember = "Name";
      cbRelRelation.FormattingEnabled = true;
      cbRelRelation.Location = new Point(108, 98);
      cbRelRelation.Margin = new Padding(3, 2, 3, 2);
      cbRelRelation.MinimumSize = new Size(176, 0);
      cbRelRelation.Name = "cbRelRelation";
      cbRelRelation.Size = new Size(179, 23);
      cbRelRelation.TabIndex = 38;
      cbRelRelation.ValueMember = "Id";
      cbRelRelation.SelectedValueChanged += cbRelRelation_SelectedValueChanged;
      // 
      // lbRelationId
      // 
      lbRelationId.AutoSize = true;
      lbRelationId.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lbRelationId.Location = new Point(21, 23);
      lbRelationId.Name = "lbRelationId";
      lbRelationId.Size = new Size(94, 21);
      lbRelationId.TabIndex = 37;
      lbRelationId.Text = "RelationId: x";
      // 
      // lbRelItemName
      // 
      lbRelItemName.AutoSize = true;
      lbRelItemName.Location = new Point(108, 54);
      lbRelItemName.Name = "lbRelItemName";
      lbRelItemName.Size = new Size(66, 15);
      lbRelItemName.TabIndex = 36;
      lbRelItemName.Text = "Item Name";
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(52, 54);
      label1.Name = "label1";
      label1.Size = new Size(41, 15);
      label1.TabIndex = 35;
      label1.Text = "Parent";
      // 
      // lbItemId
      // 
      lbItemId.AutoSize = true;
      lbItemId.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lbItemId.Location = new Point(49, 142);
      lbItemId.Name = "lbItemId";
      lbItemId.Size = new Size(68, 21);
      lbItemId.TabIndex = 34;
      lbItemId.Text = "ItemId: x";
      // 
      // btnAbortItem
      // 
      btnAbortItem.Anchor = AnchorStyles.Top;
      btnAbortItem.Location = new Point(548, 196);
      btnAbortItem.Margin = new Padding(3, 2, 3, 2);
      btnAbortItem.Name = "btnAbortItem";
      btnAbortItem.Size = new Size(66, 21);
      btnAbortItem.TabIndex = 33;
      btnAbortItem.Text = "Abort";
      btnAbortItem.UseVisualStyleBackColor = true;
      btnAbortItem.Click += btnAbortItem_Click;
      // 
      // btnUpdateItem
      // 
      btnUpdateItem.Anchor = AnchorStyles.Top;
      btnUpdateItem.Location = new Point(478, 196);
      btnUpdateItem.Margin = new Padding(3, 2, 3, 2);
      btnUpdateItem.Name = "btnUpdateItem";
      btnUpdateItem.Size = new Size(66, 21);
      btnUpdateItem.TabIndex = 32;
      btnUpdateItem.Text = "Update";
      btnUpdateItem.UseVisualStyleBackColor = true;
      btnUpdateItem.Click += btnUpdateItem_Click;
      // 
      // lbType
      // 
      lbType.AutoSize = true;
      lbType.Location = new Point(65, 197);
      lbType.Name = "lbType";
      lbType.Size = new Size(31, 15);
      lbType.TabIndex = 29;
      lbType.Text = "Type";
      // 
      // lbItemName
      // 
      lbItemName.AutoSize = true;
      lbItemName.Location = new Point(58, 176);
      lbItemName.Name = "lbItemName";
      lbItemName.Size = new Size(39, 15);
      lbItemName.TabIndex = 28;
      lbItemName.Text = "Name";
      // 
      // edItemType
      // 
      edItemType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edItemType.DisplayMember = "Name";
      edItemType.FormattingEnabled = true;
      edItemType.Location = new Point(108, 195);
      edItemType.Margin = new Padding(3, 2, 3, 2);
      edItemType.MinimumSize = new Size(176, 0);
      edItemType.Name = "edItemType";
      edItemType.Size = new Size(366, 23);
      edItemType.TabIndex = 25;
      edItemType.ValueMember = "Id";
      edItemType.SelectedValueChanged += edItemName_TextChanged;
      // 
      // edItemName
      // 
      edItemName.Location = new Point(108, 173);
      edItemName.Margin = new Padding(3, 2, 3, 2);
      edItemName.Name = "edItemName";
      edItemName.Size = new Size(490, 23);
      edItemName.TabIndex = 24;
      edItemName.TextChanged += edItemName_TextChanged;
      // 
      // tbErrorOut
      // 
      tbErrorOut.Dock = DockStyle.Fill;
      tbErrorOut.Location = new Point(0, 25);
      tbErrorOut.Margin = new Padding(3, 2, 3, 2);
      tbErrorOut.Multiline = true;
      tbErrorOut.Name = "tbErrorOut";
      tbErrorOut.ScrollBars = ScrollBars.Both;
      tbErrorOut.Size = new Size(623, 91);
      tbErrorOut.TabIndex = 1;
      tbErrorOut.WordWrap = false;
      // 
      // tsErrorPopup
      // 
      tsErrorPopup.ImageScalingSize = new Size(20, 20);
      tsErrorPopup.Items.AddRange(new ToolStripItem[] { toolStripLabel1, tsBtnDismiss });
      tsErrorPopup.Location = new Point(0, 0);
      tsErrorPopup.Name = "tsErrorPopup";
      tsErrorPopup.Size = new Size(623, 25);
      tsErrorPopup.TabIndex = 0;
      tsErrorPopup.Text = "Hide";
      // 
      // toolStripLabel1
      // 
      toolStripLabel1.Name = "toolStripLabel1";
      toolStripLabel1.Size = new Size(86, 22);
      toolStripLabel1.Text = "Error Reported ";
      // 
      // tsBtnDismiss
      // 
      tsBtnDismiss.DisplayStyle = ToolStripItemDisplayStyle.Text;
      tsBtnDismiss.ImageTransparentColor = Color.Magenta;
      tsBtnDismiss.Name = "tsBtnDismiss";
      tsBtnDismiss.Size = new Size(23, 22);
      tsBtnDismiss.Text = "✕";
      tsBtnDismiss.TextImageRelation = TextImageRelation.Overlay;
      tsBtnDismiss.Click += tsBtnDismiss_Click;
      // 
      // splitter1
      // 
      splitter1.Location = new Point(0, 0);
      splitter1.Margin = new Padding(3, 2, 3, 2);
      splitter1.Name = "splitter1";
      splitter1.Size = new Size(4, 632);
      splitter1.TabIndex = 0;
      splitter1.TabStop = false;
      // 
      // Form1
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(896, 632);
      Controls.Add(splitContainer1);
      Margin = new Padding(3, 2, 3, 2);
      Name = "Form1";
      Text = "TheLoom on WeaversGuild";
      Shown += Form1_Shown;
      Resize += Form1_Resize;
      splitContainer1.Panel1.ResumeLayout(false);
      splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
      splitContainer1.ResumeLayout(false);
      splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
      splitContainer2.ResumeLayout(false);
      cmsTreeMenus.ResumeLayout(false);
      splitContainer3.Panel1.ResumeLayout(false);
      splitContainer3.Panel2.ResumeLayout(false);
      splitContainer3.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
      splitContainer3.ResumeLayout(false);
      tabControl1.ResumeLayout(false);
      tpSettings.ResumeLayout(false);
      tpSettings.PerformLayout();
      tpItem.ResumeLayout(false);
      tpItem.PerformLayout();
      tabControl2.ResumeLayout(false);
      tpItemDesc.ResumeLayout(false);
      tpItemDesc.PerformLayout();
      tpData.ResumeLayout(false);
      tpData.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)edRank).EndInit();
      tsErrorPopup.ResumeLayout(false);
      tsErrorPopup.PerformLayout();
      ResumeLayout(false);
    }

    #endregion

    private SplitContainer splitContainer1;
    private SplitContainer splitContainer2;
    private TreeView tvKb;
    private SplitContainer splitContainer3;
    private TabControl tabControl1;
    private TabPage tpSettings;
    private TabPage tpItem;
    private Splitter splitter1;
    private ContextMenuStrip cmsTreeMenus;
    private ToolStripMenuItem miReloadTree;
    private ToolStripMenuItem miAddProjectRoot;
    private ImageList ilTreeImages;
    private ToolStripMenuItem miAddSubProject;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStrip tsErrorPopup;
    private ToolStripLabel toolStripLabel1;
    private ToolStripButton tsBtnDismiss;
    private TextBox tbErrorOut;
    private Button btnArchive;
    private Button btnCancelRelation;
    private Button btnUpdateRelation;
    private Label label7;
    private NumericUpDown edRank;
    private Label label4;
    private ComboBox cbRelRelation;
    private Label lbRelationId;
    private Label lbRelItemName;
    private Label label1;
    private Label lbItemId;
    private Button btnAbortItem;
    private Button btnUpdateItem;
    private Label lbType;
    private Label lbItemName;
    private ComboBox edItemType;
    private TextBox edItemName;
    private TabControl tabControl2;
    private TabPage tpItemDesc;
    private TabPage tpData;
    private TextBox edItemDesc;
    private TextBox edItemData;
    private Label label2;
    private TextBox edAppDefaultFolder;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripMenuItem miDeleteItem;
    private Button btnAppDefaultFolderBrowse;
    private Button btnCancelAppDefaultF;
    private Button btnSaveDefaultFolder;
    private Button btnShowErrors;
  }
}
