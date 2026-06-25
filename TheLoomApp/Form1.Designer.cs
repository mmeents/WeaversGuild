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
      miAddOrgRole = new ToolStripMenuItem();
      miAddDigitalOperator = new ToolStripMenuItem();
      miAddOrgDesk = new ToolStripMenuItem();
      miAddDeskTodo = new ToolStripMenuItem();
      miAddForeachTodo = new ToolStripMenuItem();
      miAddOrgFolder = new ToolStripMenuItem();
      miAddOrgFile = new ToolStripMenuItem();
      miAddProjectRoot = new ToolStripMenuItem();
      miAddSubProject = new ToolStripMenuItem();
      miAddSolution = new ToolStripMenuItem();
      miAddSolutionImport = new ToolStripMenuItem();
      miAddFile = new ToolStripMenuItem();
      miAddLibrary = new ToolStripMenuItem();
      miAddDiModel = new ToolStripMenuItem();
      miAddNamespace = new ToolStripMenuItem();
      miAddClass = new ToolStripMenuItem();
      miAddClassImport = new ToolStripMenuItem();
      miAddClassProp = new ToolStripMenuItem();
      miAddClassMethod = new ToolStripMenuItem();
      miAddClassMethodParam = new ToolStripMenuItem();
      miAddEntity = new ToolStripMenuItem();
      miAddEntityProperty = new ToolStripMenuItem();
      toolStripSeparator3 = new ToolStripSeparator();
      miGenerate = new ToolStripMenuItem();
      toolStripSeparator2 = new ToolStripSeparator();
      miDeleteItem = new ToolStripMenuItem();
      ilTreeImages = new ImageList(components);
      splitContainer3 = new SplitContainer();
      tabControl1 = new TabControl();
      tpSettings = new TabPage();
      lbClaudeLaunch = new LinkLabel();
      btnImportOrgDocs = new Button();
      cbShowSessions = new CheckBox();
      button1 = new Button();
      cbShowPkgInLib = new CheckBox();
      btnShowErrors = new Button();
      btnCancelAppDefaultF = new Button();
      btnSaveDefaultFolder = new Button();
      btnAppDefaultFolderBrowse = new Button();
      label2 = new Label();
      edAppDefaultFolder = new TextBox();
      tpItem = new TabPage();
      btnAttemptTodo = new Button();
      btnWriteFile = new Button();
      btnGenerateDesc = new Button();
      tabControl2 = new TabControl();
      tpItemDesc = new TabPage();
      edItemDesc = new FastColoredTextBoxNS.FastColoredTextBox();
      tpData = new TabPage();
      edItemData = new TextBox();
      tpHtml = new TabPage();
      wvDescription = new Microsoft.Web.WebView2.WinForms.WebView2();
      btnArchive = new Button();
      lbRelationId = new Label();
      lbItemId = new Label();
      btnAbortItem = new Button();
      btnUpdateItem = new Button();
      lbType = new Label();
      lbItemName = new Label();
      edItemType = new ComboBox();
      edItemName = new TextBox();
      tpReview = new TabPage();
      cbDeleteNotReady = new CheckBox();
      btnAbortReadUpdate = new Button();
      btnUpdateReady = new Button();
      cbSetReadyfromReview = new CheckBox();
      edReadyRefItem = new TextBox();
      edReadyPrompt = new TextBox();
      edReadyTodoName = new TextBox();
      labelNotReady = new Label();
      lbNotReady = new ListBox();
      tpSchedule = new TabPage();
      cbHarness = new ComboBox();
      btnAbortWorking = new Button();
      btnUpdateWorking = new Button();
      cbReadyWorking = new CheckBox();
      edWorkingRefItem = new TextBox();
      edWorkingPrompt = new TextBox();
      edWorkingName = new TextBox();
      lbWorkingStatus = new Label();
      btnStartStop = new Button();
      lbReady = new ListBox();
      tpResults = new TabPage();
      cbDeleteResult = new CheckBox();
      btnCancelUpdateResultStatus = new Button();
      btnUpdateResultStatus = new Button();
      cbArchiveResult = new CheckBox();
      edResultTodoDetails = new TextBox();
      cbTodoResultType = new ComboBox();
      label1 = new Label();
      lbTodoResults = new ListBox();
      tbErrorOut = new TextBox();
      tsErrorPopup = new ToolStrip();
      toolStripLabel1 = new ToolStripLabel();
      tsBtnDismiss = new ToolStripButton();
      splitter1 = new Splitter();
      tRun = new System.Windows.Forms.Timer(components);
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
      ((System.ComponentModel.ISupportInitialize)edItemDesc).BeginInit();
      tpData.SuspendLayout();
      tpHtml.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)wvDescription).BeginInit();
      tpReview.SuspendLayout();
      tpSchedule.SuspendLayout();
      tpResults.SuspendLayout();
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
      splitContainer1.Size = new Size(845, 632);
      splitContainer1.SplitterDistance = 249;
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
      splitContainer2.Size = new Size(249, 632);
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
      tvKb.Size = new Size(249, 569);
      tvKb.TabIndex = 0;
      tvKb.BeforeExpand += TvKb_BeforeExpand;
      tvKb.AfterSelect += tvKb_AfterSelect;
      // 
      // cmsTreeMenus
      // 
      cmsTreeMenus.ImageScalingSize = new Size(20, 20);
      cmsTreeMenus.Items.AddRange(new ToolStripItem[] { miReloadTree, toolStripSeparator1, miAddOrgRole, miAddDigitalOperator, miAddOrgDesk, miAddDeskTodo, miAddForeachTodo, miAddOrgFolder, miAddOrgFile, miAddProjectRoot, miAddSubProject, miAddSolution, miAddSolutionImport, miAddFile, miAddLibrary, miAddDiModel, miAddNamespace, miAddClass, miAddClassImport, miAddClassProp, miAddClassMethod, miAddClassMethodParam, miAddEntity, miAddEntityProperty, toolStripSeparator3, miGenerate, toolStripSeparator2, miDeleteItem });
      cmsTreeMenus.Name = "cmsTreeMenus";
      cmsTreeMenus.Size = new Size(209, 572);
      cmsTreeMenus.Opening += cmsTreeMenus_Opening;
      // 
      // miReloadTree
      // 
      miReloadTree.Name = "miReloadTree";
      miReloadTree.Size = new Size(208, 22);
      miReloadTree.Text = "Reload Projects";
      miReloadTree.Click += miReloadTree_Click;
      // 
      // toolStripSeparator1
      // 
      toolStripSeparator1.Name = "toolStripSeparator1";
      toolStripSeparator1.Size = new Size(205, 6);
      // 
      // miAddOrgRole
      // 
      miAddOrgRole.Name = "miAddOrgRole";
      miAddOrgRole.Size = new Size(208, 22);
      miAddOrgRole.Text = "Add Org DeskRole";
      miAddOrgRole.Click += miAddOrgRole_Click;
      // 
      // miAddDigitalOperator
      // 
      miAddDigitalOperator.Name = "miAddDigitalOperator";
      miAddDigitalOperator.Size = new Size(208, 22);
      miAddDigitalOperator.Text = "Add Digital Operator";
      miAddDigitalOperator.Click += miAddDigitalOperator_Click;
      // 
      // miAddOrgDesk
      // 
      miAddOrgDesk.Name = "miAddOrgDesk";
      miAddOrgDesk.Size = new Size(208, 22);
      miAddOrgDesk.Text = "Add Desk";
      miAddOrgDesk.Click += miAddOrgDesk_Click;
      // 
      // miAddDeskTodo
      // 
      miAddDeskTodo.Name = "miAddDeskTodo";
      miAddDeskTodo.Size = new Size(208, 22);
      miAddDeskTodo.Text = "Add Desk Todo";
      miAddDeskTodo.Click += miAddDeskTodo_Click;
      // 
      // miAddForeachTodo
      // 
      miAddForeachTodo.Name = "miAddForeachTodo";
      miAddForeachTodo.Size = new Size(208, 22);
      miAddForeachTodo.Text = "Add Foreach Todo";
      miAddForeachTodo.Click += miAddForeachTodo_Click;
      // 
      // miAddOrgFolder
      // 
      miAddOrgFolder.Name = "miAddOrgFolder";
      miAddOrgFolder.Size = new Size(208, 22);
      miAddOrgFolder.Text = "Add Org Folder";
      miAddOrgFolder.Click += miAddOrgFolder_Click;
      // 
      // miAddOrgFile
      // 
      miAddOrgFile.Name = "miAddOrgFile";
      miAddOrgFile.Size = new Size(208, 22);
      miAddOrgFile.Text = "Add Org File";
      miAddOrgFile.Click += miAddOrgFile_Click;
      // 
      // miAddProjectRoot
      // 
      miAddProjectRoot.Name = "miAddProjectRoot";
      miAddProjectRoot.Size = new Size(208, 22);
      miAddProjectRoot.Text = "Add Project Root";
      miAddProjectRoot.Click += miAddProjectRoot_Click;
      // 
      // miAddSubProject
      // 
      miAddSubProject.Name = "miAddSubProject";
      miAddSubProject.Size = new Size(208, 22);
      miAddSubProject.Text = "Add Folder";
      miAddSubProject.Click += miAddSubProject_Click;
      // 
      // miAddSolution
      // 
      miAddSolution.Name = "miAddSolution";
      miAddSolution.Size = new Size(208, 22);
      miAddSolution.Text = "Add Solution";
      miAddSolution.Click += miAddSolution_Click;
      // 
      // miAddSolutionImport
      // 
      miAddSolutionImport.Name = "miAddSolutionImport";
      miAddSolutionImport.Size = new Size(208, 22);
      miAddSolutionImport.Text = "Add Solution Import";
      miAddSolutionImport.Click += miAddSolutionImport_Click;
      // 
      // miAddFile
      // 
      miAddFile.Name = "miAddFile";
      miAddFile.Size = new Size(208, 22);
      miAddFile.Text = "Add File";
      miAddFile.Click += miAddFile_Click;
      // 
      // miAddLibrary
      // 
      miAddLibrary.Name = "miAddLibrary";
      miAddLibrary.Size = new Size(208, 22);
      miAddLibrary.Text = "Add Library";
      miAddLibrary.Click += miAddLibrary_Click;
      // 
      // miAddDiModel
      // 
      miAddDiModel.Name = "miAddDiModel";
      miAddDiModel.Size = new Size(208, 22);
      miAddDiModel.Text = "Add DI Model";
      miAddDiModel.Click += miAddDiModel_Click;
      // 
      // miAddNamespace
      // 
      miAddNamespace.Name = "miAddNamespace";
      miAddNamespace.Size = new Size(208, 22);
      miAddNamespace.Text = "Add Namespace";
      miAddNamespace.Click += miAddNamespace_Click;
      // 
      // miAddClass
      // 
      miAddClass.Name = "miAddClass";
      miAddClass.Size = new Size(208, 22);
      miAddClass.Text = "Add Class";
      miAddClass.Click += miAddClass_Click;
      // 
      // miAddClassImport
      // 
      miAddClassImport.Name = "miAddClassImport";
      miAddClassImport.Size = new Size(208, 22);
      miAddClassImport.Text = "Add Class Import";
      miAddClassImport.Click += miAddClassImport_Click;
      // 
      // miAddClassProp
      // 
      miAddClassProp.Name = "miAddClassProp";
      miAddClassProp.Size = new Size(208, 22);
      miAddClassProp.Text = "Add Class Property";
      miAddClassProp.Click += miAddClassProp_Click;
      // 
      // miAddClassMethod
      // 
      miAddClassMethod.Name = "miAddClassMethod";
      miAddClassMethod.Size = new Size(208, 22);
      miAddClassMethod.Text = "Add Class Method";
      miAddClassMethod.Click += miAddClassMethod_Click;
      // 
      // miAddClassMethodParam
      // 
      miAddClassMethodParam.Name = "miAddClassMethodParam";
      miAddClassMethodParam.Size = new Size(208, 22);
      miAddClassMethodParam.Text = "Add Class Method Param";
      miAddClassMethodParam.Click += miAddClassMethodParam_Click;
      // 
      // miAddEntity
      // 
      miAddEntity.Name = "miAddEntity";
      miAddEntity.Size = new Size(208, 22);
      miAddEntity.Text = "Add Entity";
      miAddEntity.Click += miAddEntity_Click;
      // 
      // miAddEntityProperty
      // 
      miAddEntityProperty.Name = "miAddEntityProperty";
      miAddEntityProperty.Size = new Size(208, 22);
      miAddEntityProperty.Text = "Add Entity Property";
      miAddEntityProperty.Click += miAddEntityProperty_Click;
      // 
      // toolStripSeparator3
      // 
      toolStripSeparator3.Name = "toolStripSeparator3";
      toolStripSeparator3.Size = new Size(205, 6);
      // 
      // miGenerate
      // 
      miGenerate.Name = "miGenerate";
      miGenerate.Size = new Size(208, 22);
      miGenerate.Text = "Generate";
      miGenerate.Click += miGenerate_Click;
      // 
      // toolStripSeparator2
      // 
      toolStripSeparator2.Name = "toolStripSeparator2";
      toolStripSeparator2.Size = new Size(205, 6);
      // 
      // miDeleteItem
      // 
      miDeleteItem.Name = "miDeleteItem";
      miDeleteItem.Size = new Size(208, 22);
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
      ilTreeImages.Images.SetKeyName(2, "callsheet.png");
      ilTreeImages.Images.SetKeyName(3, "filemodel.png");
      ilTreeImages.Images.SetKeyName(4, "DiModel.png");
      ilTreeImages.Images.SetKeyName(5, "InterfaceModel.png");
      ilTreeImages.Images.SetKeyName(6, "propertyModel.png");
      ilTreeImages.Images.SetKeyName(7, "MethodModel.png");
      ilTreeImages.Images.SetKeyName(8, "paramModel.png");
      ilTreeImages.Images.SetKeyName(9, "ClassModel.png");
      ilTreeImages.Images.SetKeyName(10, "HandlerModel.png");
      ilTreeImages.Images.SetKeyName(11, "DblCabnet.png");
      ilTreeImages.Images.SetKeyName(12, "deliverable.png");
      ilTreeImages.Images.SetKeyName(13, "story.png");
      ilTreeImages.Images.SetKeyName(14, "EntityProperty.png");
      ilTreeImages.Images.SetKeyName(15, "importBrick.png");
      ilTreeImages.Images.SetKeyName(16, "globe.png");
      ilTreeImages.Images.SetKeyName(17, "scroll.png");
      ilTreeImages.Images.SetKeyName(18, "scene.png");
      ilTreeImages.Images.SetKeyName(19, "character.png");
      ilTreeImages.Images.SetKeyName(20, "Galactic-Senate--Streamline-Font-Awesome.png");
      ilTreeImages.Images.SetKeyName(21, "Ai-Settings-Cog-Spark--Streamline-Micro.png");
      ilTreeImages.Images.SetKeyName(22, "Mail-Send--Streamline-Micro.png");
      ilTreeImages.Images.SetKeyName(23, "Inbox-Open--Streamline-Micro.png");
      ilTreeImages.Images.SetKeyName(24, "narration.png");
      ilTreeImages.Images.SetKeyName(25, "GatwayWeave.png");
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
      splitContainer3.Size = new Size(588, 632);
      splitContainer3.SplitterDistance = 513;
      splitContainer3.SplitterWidth = 3;
      splitContainer3.TabIndex = 1;
      splitContainer3.SplitterMoved += splitContainer3_SplitterMoved;
      // 
      // tabControl1
      // 
      tabControl1.Controls.Add(tpSettings);
      tabControl1.Controls.Add(tpItem);
      tabControl1.Controls.Add(tpReview);
      tabControl1.Controls.Add(tpSchedule);
      tabControl1.Controls.Add(tpResults);
      tabControl1.Dock = DockStyle.Fill;
      tabControl1.Location = new Point(0, 0);
      tabControl1.Margin = new Padding(3, 2, 3, 2);
      tabControl1.Name = "tabControl1";
      tabControl1.SelectedIndex = 0;
      tabControl1.Size = new Size(588, 513);
      tabControl1.TabIndex = 0;
      tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
      // 
      // tpSettings
      // 
      tpSettings.Controls.Add(lbClaudeLaunch);
      tpSettings.Controls.Add(btnImportOrgDocs);
      tpSettings.Controls.Add(cbShowSessions);
      tpSettings.Controls.Add(button1);
      tpSettings.Controls.Add(cbShowPkgInLib);
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
      tpSettings.Size = new Size(580, 485);
      tpSettings.TabIndex = 0;
      tpSettings.Text = "Settings";
      tpSettings.UseVisualStyleBackColor = true;
      // 
      // lbClaudeLaunch
      // 
      lbClaudeLaunch.AutoSize = true;
      lbClaudeLaunch.Location = new Point(35, 159);
      lbClaudeLaunch.Name = "lbClaudeLaunch";
      lbClaudeLaunch.Size = new Size(93, 15);
      lbClaudeLaunch.TabIndex = 10;
      lbClaudeLaunch.TabStop = true;
      lbClaudeLaunch.Text = "lbClaudeLaunch";
      lbClaudeLaunch.LinkClicked += lbClaudeLaunch_LinkClicked;
      // 
      // btnImportOrgDocs
      // 
      btnImportOrgDocs.Location = new Point(22, 117);
      btnImportOrgDocs.Name = "btnImportOrgDocs";
      btnImportOrgDocs.Size = new Size(112, 23);
      btnImportOrgDocs.TabIndex = 9;
      btnImportOrgDocs.Text = "Import OrgDocs";
      btnImportOrgDocs.UseVisualStyleBackColor = true;
      btnImportOrgDocs.Click += btnImportOrgDocs_Click;
      // 
      // cbShowSessions
      // 
      cbShowSessions.AutoSize = true;
      cbShowSessions.Location = new Point(22, 83);
      cbShowSessions.Name = "cbShowSessions";
      cbShowSessions.Size = new Size(102, 19);
      cbShowSessions.TabIndex = 8;
      cbShowSessions.Text = "Show Sessions";
      cbShowSessions.UseVisualStyleBackColor = true;
      cbShowSessions.CheckedChanged += cbShowSessions_CheckedChanged;
      // 
      // button1
      // 
      button1.Location = new Point(438, 84);
      button1.Name = "button1";
      button1.Size = new Size(125, 23);
      button1.TabIndex = 7;
      button1.Text = "Test Create dialog";
      button1.UseVisualStyleBackColor = true;
      button1.Visible = false;
      button1.Click += button1_Click;
      // 
      // cbShowPkgInLib
      // 
      cbShowPkgInLib.AutoSize = true;
      cbShowPkgInLib.Location = new Point(22, 58);
      cbShowPkgInLib.Name = "cbShowPkgInLib";
      cbShowPkgInLib.Size = new Size(182, 19);
      cbShowPkgInLib.TabIndex = 6;
      cbShowPkgInLib.Text = "Show Library Packages in tree";
      cbShowPkgInLib.UseVisualStyleBackColor = true;
      cbShowPkgInLib.CheckedChanged += cbShowPkgInLib_CheckedChanged;
      // 
      // btnShowErrors
      // 
      btnShowErrors.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnShowErrors.Location = new Point(483, 55);
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
      btnCancelAppDefaultF.Location = new Point(536, 19);
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
      btnSaveDefaultFolder.Location = new Point(499, 19);
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
      btnAppDefaultFolderBrowse.Location = new Point(463, 19);
      btnAppDefaultFolderBrowse.Name = "btnAppDefaultFolderBrowse";
      btnAppDefaultFolderBrowse.Size = new Size(36, 23);
      btnAppDefaultFolderBrowse.TabIndex = 2;
      btnAppDefaultFolderBrowse.Text = "↗";
      btnAppDefaultFolderBrowse.UseVisualStyleBackColor = true;
      btnAppDefaultFolderBrowse.Click += btnAppDefaultFolderBrowse_Click;
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
      edAppDefaultFolder.Size = new Size(313, 23);
      edAppDefaultFolder.TabIndex = 0;
      edAppDefaultFolder.TextChanged += edAppDefaultFolder_TextChanged;
      // 
      // tpItem
      // 
      tpItem.Controls.Add(btnAttemptTodo);
      tpItem.Controls.Add(btnWriteFile);
      tpItem.Controls.Add(btnGenerateDesc);
      tpItem.Controls.Add(tabControl2);
      tpItem.Controls.Add(btnArchive);
      tpItem.Controls.Add(lbRelationId);
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
      tpItem.Size = new Size(580, 485);
      tpItem.TabIndex = 1;
      tpItem.Text = "Details";
      tpItem.UseVisualStyleBackColor = true;
      // 
      // btnAttemptTodo
      // 
      btnAttemptTodo.Location = new Point(225, 33);
      btnAttemptTodo.Name = "btnAttemptTodo";
      btnAttemptTodo.Size = new Size(101, 23);
      btnAttemptTodo.TabIndex = 48;
      btnAttemptTodo.Text = "Attempt Todo";
      btnAttemptTodo.UseVisualStyleBackColor = true;
      btnAttemptTodo.Click += btnAttemptTodo_Click;
      // 
      // btnWriteFile
      // 
      btnWriteFile.Anchor = AnchorStyles.Top;
      btnWriteFile.Location = new Point(332, 35);
      btnWriteFile.Margin = new Padding(3, 2, 3, 2);
      btnWriteFile.Name = "btnWriteFile";
      btnWriteFile.Size = new Size(92, 21);
      btnWriteFile.TabIndex = 47;
      btnWriteFile.Text = "Write updated";
      btnWriteFile.UseVisualStyleBackColor = true;
      btnWriteFile.Click += btnWriteFile_Click;
      // 
      // btnGenerateDesc
      // 
      btnGenerateDesc.Anchor = AnchorStyles.Top;
      btnGenerateDesc.Location = new Point(430, 35);
      btnGenerateDesc.Margin = new Padding(3, 2, 3, 2);
      btnGenerateDesc.Name = "btnGenerateDesc";
      btnGenerateDesc.Size = new Size(92, 21);
      btnGenerateDesc.TabIndex = 46;
      btnGenerateDesc.Text = "Generate Desc";
      btnGenerateDesc.UseVisualStyleBackColor = true;
      btnGenerateDesc.Click += btnGenerateDesc_Click;
      // 
      // tabControl2
      // 
      tabControl2.Controls.Add(tpItemDesc);
      tabControl2.Controls.Add(tpData);
      tabControl2.Controls.Add(tpHtml);
      tabControl2.Dock = DockStyle.Bottom;
      tabControl2.Location = new Point(3, 247);
      tabControl2.Margin = new Padding(3, 2, 3, 2);
      tabControl2.Name = "tabControl2";
      tabControl2.SelectedIndex = 0;
      tabControl2.Size = new Size(574, 236);
      tabControl2.TabIndex = 45;
      // 
      // tpItemDesc
      // 
      tpItemDesc.Controls.Add(edItemDesc);
      tpItemDesc.Location = new Point(4, 24);
      tpItemDesc.Margin = new Padding(3, 2, 3, 2);
      tpItemDesc.Name = "tpItemDesc";
      tpItemDesc.Padding = new Padding(3, 2, 3, 2);
      tpItemDesc.Size = new Size(566, 208);
      tpItemDesc.TabIndex = 0;
      tpItemDesc.Text = "Description";
      tpItemDesc.UseVisualStyleBackColor = true;
      // 
      // edItemDesc
      // 
      edItemDesc.AutoCompleteBracketsList = new char[]
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
      edItemDesc.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
      edItemDesc.AutoScrollMinSize = new Size(154, 14);
      edItemDesc.BackBrush = null;
      edItemDesc.CharHeight = 14;
      edItemDesc.CharWidth = 8;
      edItemDesc.DefaultMarkerSize = 8;
      edItemDesc.DisabledColor = Color.FromArgb(100, 180, 180, 180);
      edItemDesc.Dock = DockStyle.Fill;
      edItemDesc.FindForm = null;
      edItemDesc.GoToForm = null;
      edItemDesc.Hotkeys = resources.GetString("edItemDesc.Hotkeys");
      edItemDesc.IsReplaceMode = false;
      edItemDesc.Location = new Point(3, 2);
      edItemDesc.Name = "edItemDesc";
      edItemDesc.Paddings = new Padding(0);
      edItemDesc.ReplaceForm = null;
      edItemDesc.SelectionColor = Color.FromArgb(60, 0, 0, 255);
      edItemDesc.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("edItemDesc.ServiceColors");
      edItemDesc.Size = new Size(560, 204);
      edItemDesc.TabIndex = 0;
      edItemDesc.Text = "fastColoredTextBox1";
      edItemDesc.WordWrapAutoIndent = false;
      edItemDesc.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.CharWrapPreferredWidth;
      edItemDesc.Zoom = 100;
      edItemDesc.TextChanged += edItemDesc2_TextChanged;
      // 
      // tpData
      // 
      tpData.Controls.Add(edItemData);
      tpData.Location = new Point(4, 24);
      tpData.Margin = new Padding(3, 2, 3, 2);
      tpData.Name = "tpData";
      tpData.Padding = new Padding(3, 2, 3, 2);
      tpData.Size = new Size(566, 208);
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
      edItemData.Size = new Size(560, 204);
      edItemData.TabIndex = 27;
      edItemData.TextChanged += edItemName_TextChanged;
      // 
      // tpHtml
      // 
      tpHtml.Controls.Add(wvDescription);
      tpHtml.Location = new Point(4, 24);
      tpHtml.Name = "tpHtml";
      tpHtml.Padding = new Padding(3);
      tpHtml.Size = new Size(566, 208);
      tpHtml.TabIndex = 2;
      tpHtml.Text = "Html";
      tpHtml.UseVisualStyleBackColor = true;
      // 
      // wvDescription
      // 
      wvDescription.AllowExternalDrop = true;
      wvDescription.CreationProperties = null;
      wvDescription.DefaultBackgroundColor = Color.White;
      wvDescription.Dock = DockStyle.Fill;
      wvDescription.Location = new Point(3, 3);
      wvDescription.Name = "wvDescription";
      wvDescription.Size = new Size(560, 202);
      wvDescription.TabIndex = 0;
      wvDescription.ZoomFactor = 1D;
      // 
      // btnArchive
      // 
      btnArchive.Anchor = AnchorStyles.Top;
      btnArchive.Location = new Point(523, 35);
      btnArchive.Margin = new Padding(3, 2, 3, 2);
      btnArchive.Name = "btnArchive";
      btnArchive.Size = new Size(66, 21);
      btnArchive.TabIndex = 44;
      btnArchive.Text = "Archive";
      btnArchive.UseVisualStyleBackColor = true;
      btnArchive.Click += btnArchive_Click;
      // 
      // lbRelationId
      // 
      lbRelationId.AutoSize = true;
      lbRelationId.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lbRelationId.Location = new Point(6, 6);
      lbRelationId.Name = "lbRelationId";
      lbRelationId.Size = new Size(94, 21);
      lbRelationId.TabIndex = 37;
      lbRelationId.Text = "RelationId: x";
      // 
      // lbItemId
      // 
      lbItemId.AutoSize = true;
      lbItemId.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lbItemId.Location = new Point(6, 35);
      lbItemId.Name = "lbItemId";
      lbItemId.Size = new Size(68, 21);
      lbItemId.TabIndex = 34;
      lbItemId.Text = "ItemId: x";
      // 
      // btnAbortItem
      // 
      btnAbortItem.Anchor = AnchorStyles.Top;
      btnAbortItem.Location = new Point(488, 93);
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
      btnUpdateItem.Location = new Point(418, 93);
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
      lbType.Location = new Point(22, 94);
      lbType.Name = "lbType";
      lbType.Size = new Size(32, 15);
      lbType.TabIndex = 29;
      lbType.Text = "Type";
      // 
      // lbItemName
      // 
      lbItemName.AutoSize = true;
      lbItemName.Location = new Point(15, 69);
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
      edItemType.Location = new Point(65, 92);
      edItemType.Margin = new Padding(3, 2, 3, 2);
      edItemType.MinimumSize = new Size(176, 0);
      edItemType.Name = "edItemType";
      edItemType.Size = new Size(331, 23);
      edItemType.TabIndex = 25;
      edItemType.ValueMember = "Id";
      edItemType.SelectedValueChanged += edItemName_TextChanged;
      // 
      // edItemName
      // 
      edItemName.Location = new Point(65, 66);
      edItemName.Margin = new Padding(3, 2, 3, 2);
      edItemName.Name = "edItemName";
      edItemName.Size = new Size(490, 23);
      edItemName.TabIndex = 24;
      edItemName.TextChanged += edItemName_TextChanged;
      // 
      // tpReview
      // 
      tpReview.Controls.Add(cbDeleteNotReady);
      tpReview.Controls.Add(btnAbortReadUpdate);
      tpReview.Controls.Add(btnUpdateReady);
      tpReview.Controls.Add(cbSetReadyfromReview);
      tpReview.Controls.Add(edReadyRefItem);
      tpReview.Controls.Add(edReadyPrompt);
      tpReview.Controls.Add(edReadyTodoName);
      tpReview.Controls.Add(labelNotReady);
      tpReview.Controls.Add(lbNotReady);
      tpReview.Location = new Point(4, 24);
      tpReview.Name = "tpReview";
      tpReview.Size = new Size(580, 485);
      tpReview.TabIndex = 3;
      tpReview.Text = "Ready Review";
      tpReview.UseVisualStyleBackColor = true;
      // 
      // cbDeleteNotReady
      // 
      cbDeleteNotReady.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      cbDeleteNotReady.AutoSize = true;
      cbDeleteNotReady.Location = new Point(104, 457);
      cbDeleteNotReady.Name = "cbDeleteNotReady";
      cbDeleteNotReady.Size = new Size(59, 19);
      cbDeleteNotReady.TabIndex = 43;
      cbDeleteNotReady.Text = "Delete";
      cbDeleteNotReady.UseVisualStyleBackColor = true;
      cbDeleteNotReady.CheckedChanged += cbSetReadyfromReview_CheckedChanged;
      // 
      // btnAbortReadUpdate
      // 
      btnAbortReadUpdate.Anchor = AnchorStyles.Bottom;
      btnAbortReadUpdate.Location = new Point(248, 455);
      btnAbortReadUpdate.Margin = new Padding(3, 2, 3, 2);
      btnAbortReadUpdate.Name = "btnAbortReadUpdate";
      btnAbortReadUpdate.Size = new Size(66, 21);
      btnAbortReadUpdate.TabIndex = 35;
      btnAbortReadUpdate.Text = "Abort";
      btnAbortReadUpdate.UseVisualStyleBackColor = true;
      btnAbortReadUpdate.Click += btnAbortReadUpdate_Click;
      // 
      // btnUpdateReady
      // 
      btnUpdateReady.Anchor = AnchorStyles.Bottom;
      btnUpdateReady.Location = new Point(178, 455);
      btnUpdateReady.Margin = new Padding(3, 2, 3, 2);
      btnUpdateReady.Name = "btnUpdateReady";
      btnUpdateReady.Size = new Size(66, 21);
      btnUpdateReady.TabIndex = 34;
      btnUpdateReady.Text = "Update";
      btnUpdateReady.UseVisualStyleBackColor = true;
      btnUpdateReady.Click += btnUpdateReady_Click;
      // 
      // cbSetReadyfromReview
      // 
      cbSetReadyfromReview.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      cbSetReadyfromReview.AutoSize = true;
      cbSetReadyfromReview.Location = new Point(21, 457);
      cbSetReadyfromReview.Name = "cbSetReadyfromReview";
      cbSetReadyfromReview.Size = new Size(77, 19);
      cbSetReadyfromReview.TabIndex = 7;
      cbSetReadyfromReview.Text = "Set Ready";
      cbSetReadyfromReview.UseVisualStyleBackColor = true;
      cbSetReadyfromReview.CheckedChanged += cbSetReadyfromReview_CheckedChanged;
      // 
      // edReadyRefItem
      // 
      edReadyRefItem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edReadyRefItem.Location = new Point(21, 229);
      edReadyRefItem.Name = "edReadyRefItem";
      edReadyRefItem.Size = new Size(536, 23);
      edReadyRefItem.TabIndex = 6;
      // 
      // edReadyPrompt
      // 
      edReadyPrompt.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      edReadyPrompt.Location = new Point(21, 261);
      edReadyPrompt.Multiline = true;
      edReadyPrompt.Name = "edReadyPrompt";
      edReadyPrompt.ScrollBars = ScrollBars.Vertical;
      edReadyPrompt.Size = new Size(536, 190);
      edReadyPrompt.TabIndex = 5;
      // 
      // edReadyTodoName
      // 
      edReadyTodoName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edReadyTodoName.Location = new Point(21, 200);
      edReadyTodoName.Name = "edReadyTodoName";
      edReadyTodoName.Size = new Size(536, 23);
      edReadyTodoName.TabIndex = 4;
      // 
      // labelNotReady
      // 
      labelNotReady.AutoSize = true;
      labelNotReady.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      labelNotReady.Location = new Point(9, 8);
      labelNotReady.Name = "labelNotReady";
      labelNotReady.Size = new Size(127, 21);
      labelNotReady.TabIndex = 3;
      labelNotReady.Text = "Not Ready Todos";
      // 
      // lbNotReady
      // 
      lbNotReady.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      lbNotReady.FormattingEnabled = true;
      lbNotReady.Location = new Point(21, 39);
      lbNotReady.Name = "lbNotReady";
      lbNotReady.Size = new Size(536, 139);
      lbNotReady.TabIndex = 1;
      lbNotReady.SelectedIndexChanged += lbNotReady_SelectedIndexChanged;
      // 
      // tpSchedule
      // 
      tpSchedule.Controls.Add(cbHarness);
      tpSchedule.Controls.Add(btnAbortWorking);
      tpSchedule.Controls.Add(btnUpdateWorking);
      tpSchedule.Controls.Add(cbReadyWorking);
      tpSchedule.Controls.Add(edWorkingRefItem);
      tpSchedule.Controls.Add(edWorkingPrompt);
      tpSchedule.Controls.Add(edWorkingName);
      tpSchedule.Controls.Add(lbWorkingStatus);
      tpSchedule.Controls.Add(btnStartStop);
      tpSchedule.Controls.Add(lbReady);
      tpSchedule.Location = new Point(4, 24);
      tpSchedule.Name = "tpSchedule";
      tpSchedule.Padding = new Padding(3);
      tpSchedule.Size = new Size(580, 485);
      tpSchedule.TabIndex = 2;
      tpSchedule.Text = "Schedule";
      tpSchedule.UseVisualStyleBackColor = true;
      // 
      // cbHarness
      // 
      cbHarness.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      cbHarness.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
      cbHarness.FormattingEnabled = true;
      cbHarness.Location = new Point(365, 7);
      cbHarness.Name = "cbHarness";
      cbHarness.Size = new Size(190, 27);
      cbHarness.TabIndex = 39;
      cbHarness.SelectedIndexChanged += cbHarness_SelectedIndexChanged;
      // 
      // btnAbortWorking
      // 
      btnAbortWorking.Anchor = AnchorStyles.Bottom;
      btnAbortWorking.Location = new Point(196, 453);
      btnAbortWorking.Margin = new Padding(3, 2, 3, 2);
      btnAbortWorking.Name = "btnAbortWorking";
      btnAbortWorking.Size = new Size(66, 21);
      btnAbortWorking.TabIndex = 38;
      btnAbortWorking.Text = "Abort";
      btnAbortWorking.UseVisualStyleBackColor = true;
      btnAbortWorking.Click += btnAbortWorking_Click;
      // 
      // btnUpdateWorking
      // 
      btnUpdateWorking.Anchor = AnchorStyles.Bottom;
      btnUpdateWorking.Location = new Point(126, 453);
      btnUpdateWorking.Margin = new Padding(3, 2, 3, 2);
      btnUpdateWorking.Name = "btnUpdateWorking";
      btnUpdateWorking.Size = new Size(66, 21);
      btnUpdateWorking.TabIndex = 37;
      btnUpdateWorking.Text = "Update";
      btnUpdateWorking.UseVisualStyleBackColor = true;
      btnUpdateWorking.Click += btnUpdateWorking_Click;
      // 
      // cbReadyWorking
      // 
      cbReadyWorking.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      cbReadyWorking.AutoSize = true;
      cbReadyWorking.Location = new Point(27, 455);
      cbReadyWorking.Name = "cbReadyWorking";
      cbReadyWorking.Size = new Size(77, 19);
      cbReadyWorking.TabIndex = 36;
      cbReadyWorking.Text = "Set Ready";
      cbReadyWorking.UseVisualStyleBackColor = true;
      cbReadyWorking.CheckedChanged += cbReadyWorking_CheckedChanged;
      // 
      // edWorkingRefItem
      // 
      edWorkingRefItem.Location = new Point(21, 229);
      edWorkingRefItem.Name = "edWorkingRefItem";
      edWorkingRefItem.Size = new Size(534, 23);
      edWorkingRefItem.TabIndex = 21;
      // 
      // edWorkingPrompt
      // 
      edWorkingPrompt.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      edWorkingPrompt.Location = new Point(21, 258);
      edWorkingPrompt.Multiline = true;
      edWorkingPrompt.Name = "edWorkingPrompt";
      edWorkingPrompt.ScrollBars = ScrollBars.Vertical;
      edWorkingPrompt.Size = new Size(534, 190);
      edWorkingPrompt.TabIndex = 20;
      // 
      // edWorkingName
      // 
      edWorkingName.Location = new Point(21, 200);
      edWorkingName.Name = "edWorkingName";
      edWorkingName.Size = new Size(534, 23);
      edWorkingName.TabIndex = 19;
      // 
      // lbWorkingStatus
      // 
      lbWorkingStatus.AutoSize = true;
      lbWorkingStatus.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lbWorkingStatus.Location = new Point(75, 8);
      lbWorkingStatus.Name = "lbWorkingStatus";
      lbWorkingStatus.Size = new Size(182, 21);
      lbWorkingStatus.TabIndex = 18;
      lbWorkingStatus.Text = "Loom Operational Status";
      // 
      // btnStartStop
      // 
      btnStartStop.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      btnStartStop.Location = new Point(5, 3);
      btnStartStop.Name = "btnStartStop";
      btnStartStop.Size = new Size(63, 31);
      btnStartStop.TabIndex = 15;
      btnStartStop.Text = "Start";
      btnStartStop.UseVisualStyleBackColor = true;
      btnStartStop.Click += btnStartStop_Click;
      // 
      // lbReady
      // 
      lbReady.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      lbReady.FormattingEnabled = true;
      lbReady.Location = new Point(21, 39);
      lbReady.Name = "lbReady";
      lbReady.Size = new Size(536, 139);
      lbReady.TabIndex = 13;
      lbReady.SelectedIndexChanged += lbReady_SelectedIndexChanged;
      // 
      // tpResults
      // 
      tpResults.Controls.Add(cbDeleteResult);
      tpResults.Controls.Add(btnCancelUpdateResultStatus);
      tpResults.Controls.Add(btnUpdateResultStatus);
      tpResults.Controls.Add(cbArchiveResult);
      tpResults.Controls.Add(edResultTodoDetails);
      tpResults.Controls.Add(cbTodoResultType);
      tpResults.Controls.Add(label1);
      tpResults.Controls.Add(lbTodoResults);
      tpResults.Location = new Point(4, 24);
      tpResults.Name = "tpResults";
      tpResults.Size = new Size(580, 485);
      tpResults.TabIndex = 4;
      tpResults.Text = "Results";
      tpResults.UseVisualStyleBackColor = true;
      // 
      // cbDeleteResult
      // 
      cbDeleteResult.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      cbDeleteResult.AutoSize = true;
      cbDeleteResult.Location = new Point(99, 451);
      cbDeleteResult.Name = "cbDeleteResult";
      cbDeleteResult.Size = new Size(59, 19);
      cbDeleteResult.TabIndex = 42;
      cbDeleteResult.Text = "Delete";
      cbDeleteResult.UseVisualStyleBackColor = true;
      cbDeleteResult.CheckedChanged += cbDeleteResult_CheckedChanged;
      // 
      // btnCancelUpdateResultStatus
      // 
      btnCancelUpdateResultStatus.Anchor = AnchorStyles.Bottom;
      btnCancelUpdateResultStatus.Location = new Point(259, 449);
      btnCancelUpdateResultStatus.Margin = new Padding(3, 2, 3, 2);
      btnCancelUpdateResultStatus.Name = "btnCancelUpdateResultStatus";
      btnCancelUpdateResultStatus.Size = new Size(66, 21);
      btnCancelUpdateResultStatus.TabIndex = 41;
      btnCancelUpdateResultStatus.Text = "Abort";
      btnCancelUpdateResultStatus.UseVisualStyleBackColor = true;
      btnCancelUpdateResultStatus.Click += btnCancelUpdateResultStatus_Click;
      // 
      // btnUpdateResultStatus
      // 
      btnUpdateResultStatus.Anchor = AnchorStyles.Bottom;
      btnUpdateResultStatus.Location = new Point(189, 449);
      btnUpdateResultStatus.Margin = new Padding(3, 2, 3, 2);
      btnUpdateResultStatus.Name = "btnUpdateResultStatus";
      btnUpdateResultStatus.Size = new Size(66, 21);
      btnUpdateResultStatus.TabIndex = 40;
      btnUpdateResultStatus.Text = "Update";
      btnUpdateResultStatus.UseVisualStyleBackColor = true;
      btnUpdateResultStatus.Click += btnUpdateResultStatus_Click;
      // 
      // cbArchiveResult
      // 
      cbArchiveResult.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      cbArchiveResult.AutoSize = true;
      cbArchiveResult.Location = new Point(27, 451);
      cbArchiveResult.Name = "cbArchiveResult";
      cbArchiveResult.Size = new Size(66, 19);
      cbArchiveResult.TabIndex = 39;
      cbArchiveResult.Text = "Archive";
      cbArchiveResult.UseVisualStyleBackColor = true;
      cbArchiveResult.CheckedChanged += cbArchiveResult_CheckedChanged;
      // 
      // edResultTodoDetails
      // 
      edResultTodoDetails.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      edResultTodoDetails.Location = new Point(23, 222);
      edResultTodoDetails.Multiline = true;
      edResultTodoDetails.Name = "edResultTodoDetails";
      edResultTodoDetails.ScrollBars = ScrollBars.Vertical;
      edResultTodoDetails.Size = new Size(534, 222);
      edResultTodoDetails.TabIndex = 21;
      // 
      // cbTodoResultType
      // 
      cbTodoResultType.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
      cbTodoResultType.FormattingEnabled = true;
      cbTodoResultType.Items.AddRange(new object[] { "Completed", "Aborted", "Failed" });
      cbTodoResultType.Location = new Point(141, 6);
      cbTodoResultType.Name = "cbTodoResultType";
      cbTodoResultType.Size = new Size(121, 27);
      cbTodoResultType.TabIndex = 6;
      cbTodoResultType.SelectedIndexChanged += cbTodoResultType_SelectedIndexChanged;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      label1.Location = new Point(9, 8);
      label1.Name = "label1";
      label1.Size = new Size(117, 21);
      label1.TabIndex = 5;
      label1.Text = "Results by Type";
      // 
      // lbTodoResults
      // 
      lbTodoResults.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      lbTodoResults.FormattingEnabled = true;
      lbTodoResults.Location = new Point(21, 39);
      lbTodoResults.Name = "lbTodoResults";
      lbTodoResults.Size = new Size(536, 139);
      lbTodoResults.TabIndex = 4;
      lbTodoResults.SelectedIndexChanged += lbTodoResults_SelectedIndexChanged;
      // 
      // tbErrorOut
      // 
      tbErrorOut.Dock = DockStyle.Fill;
      tbErrorOut.Location = new Point(0, 25);
      tbErrorOut.Margin = new Padding(3, 2, 3, 2);
      tbErrorOut.Multiline = true;
      tbErrorOut.Name = "tbErrorOut";
      tbErrorOut.ScrollBars = ScrollBars.Both;
      tbErrorOut.Size = new Size(588, 91);
      tbErrorOut.TabIndex = 1;
      // 
      // tsErrorPopup
      // 
      tsErrorPopup.ImageScalingSize = new Size(20, 20);
      tsErrorPopup.Items.AddRange(new ToolStripItem[] { toolStripLabel1, tsBtnDismiss });
      tsErrorPopup.Location = new Point(0, 0);
      tsErrorPopup.Name = "tsErrorPopup";
      tsErrorPopup.Size = new Size(588, 25);
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
      // tRun
      // 
      tRun.Interval = 250;
      tRun.Tick += tRun_Tick;
      // 
      // Form1
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(845, 632);
      Controls.Add(splitContainer1);
      Icon = (Icon)resources.GetObject("$this.Icon");
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
      ((System.ComponentModel.ISupportInitialize)edItemDesc).EndInit();
      tpData.ResumeLayout(false);
      tpData.PerformLayout();
      tpHtml.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)wvDescription).EndInit();
      tpReview.ResumeLayout(false);
      tpReview.PerformLayout();
      tpSchedule.ResumeLayout(false);
      tpSchedule.PerformLayout();
      tpResults.ResumeLayout(false);
      tpResults.PerformLayout();
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
    private Label lbRelationId;
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
    private TextBox edItemData;
    private Label label2;
    private TextBox edAppDefaultFolder;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripMenuItem miDeleteItem;
    private Button btnAppDefaultFolderBrowse;
    private Button btnCancelAppDefaultF;
    private Button btnSaveDefaultFolder;
    private Button btnShowErrors;
    private ToolStripMenuItem miAddFile;
    private ToolStripSeparator toolStripSeparator3;
    private ToolStripMenuItem miGenerate;
    private ToolStripMenuItem miAddLibrary;
    private ToolStripMenuItem miAddDiModel;
    private ToolStripMenuItem miAddNamespace;
    private ToolStripMenuItem miAddClass;
    private ToolStripMenuItem miAddSolution;
    private ToolStripMenuItem miAddSolutionImport;
    private ToolStripMenuItem miAddClassImport;
    private ToolStripMenuItem miAddClassProp;
    private ToolStripMenuItem miAddClassMethod;
    private Button btnGenerateDesc;
    private ToolStripMenuItem miAddClassMethodParam;
    private FastColoredTextBoxNS.FastColoredTextBox edItemDesc;
    private ToolStripMenuItem miAddEntity;
    private ToolStripMenuItem miAddEntityProperty;
    private CheckBox cbShowPkgInLib;
    private Button btnWriteFile;
    private Button button1;
    private TabPage tpHtml;
    private Microsoft.Web.WebView2.WinForms.WebView2 wvDescription;
    private CheckBox cbShowSessions;
    private ToolStripMenuItem miAddDigitalOperator;
    private ToolStripMenuItem miAddOrgFolder;
    private ToolStripMenuItem miAddOrgFile;
    private Button btnImportOrgDocs;
    private ToolStripMenuItem miAddOrgDesk;
    private ToolStripMenuItem miAddDeskTodo;
    private Button btnAttemptTodo;
    private ToolStripMenuItem miAddForeachTodo;
    private TabPage tpSchedule;
    private TabPage tpReview;
    private Label labelNotReady;
    private ListBox lbNotReady;
    private Button btnStartStop;
    private ListBox lbReady;
    private TextBox edReadyRefItem;
    private TextBox edReadyPrompt;
    private TextBox edReadyTodoName;
    private Button btnAbortReadUpdate;
    private Button btnUpdateReady;
    private CheckBox cbSetReadyfromReview;
    private System.Windows.Forms.Timer tRun;
    private Label lbWorkingStatus;
    private TextBox edWorkingRefItem;
    private TextBox edWorkingPrompt;
    private TextBox edWorkingName;
    private Button btnAbortWorking;
    private Button btnUpdateWorking;
    private CheckBox cbReadyWorking;
    private TabPage tpResults;
    private Label label1;
    private ListBox lbTodoResults;
    private ComboBox cbTodoResultType;
    private CheckBox cbDeleteResult;
    private Button btnCancelUpdateResultStatus;
    private Button btnUpdateResultStatus;
    private CheckBox cbArchiveResult;
    private TextBox edResultTodoDetails;
    private CheckBox cbDeleteNotReady;
    private ToolStripMenuItem miAddOrgRole;
    private ComboBox cbHarness;
    private LinkLabel lbClaudeLaunch;
  }
}
