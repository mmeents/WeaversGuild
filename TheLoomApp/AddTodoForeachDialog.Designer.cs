namespace TheLoomApp {
  partial class AddTodoForeachDialog {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddTodoForeachDialog));
      tabState = new TabControl();
      tabForeach = new TabPage();
      btnNext = new Button();
      btnCancel = new Button();
      lbAtDesk = new Label();
      label1 = new Label();
      edForeach = new TextBox();
      tabTodo = new TabPage();
      label3 = new Label();
      cbType = new ComboBox();
      lbName = new Label();
      cbValues = new ComboBox();
      lbItemDemo = new Label();
      edPromptTemplate = new FastColoredTextBoxNS.FastColoredTextBox();
      label2 = new Label();
      btnBack = new Button();
      btnOk = new Button();
      tabState.SuspendLayout();
      tabForeach.SuspendLayout();
      tabTodo.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)edPromptTemplate).BeginInit();
      SuspendLayout();
      // 
      // tabState
      // 
      tabState.Controls.Add(tabForeach);
      tabState.Controls.Add(tabTodo);
      tabState.Dock = DockStyle.Fill;
      tabState.Location = new Point(0, 0);
      tabState.Name = "tabState";
      tabState.SelectedIndex = 0;
      tabState.Size = new Size(800, 450);
      tabState.TabIndex = 0;
      // 
      // tabForeach
      // 
      tabForeach.Controls.Add(btnNext);
      tabForeach.Controls.Add(btnCancel);
      tabForeach.Controls.Add(lbAtDesk);
      tabForeach.Controls.Add(label1);
      tabForeach.Controls.Add(edForeach);
      tabForeach.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
      tabForeach.Location = new Point(4, 24);
      tabForeach.Name = "tabForeach";
      tabForeach.Padding = new Padding(3);
      tabForeach.Size = new Size(792, 422);
      tabForeach.TabIndex = 0;
      tabForeach.Text = "Foreach";
      tabForeach.UseVisualStyleBackColor = true;
      // 
      // btnNext
      // 
      btnNext.Location = new Point(276, 349);
      btnNext.Name = "btnNext";
      btnNext.Size = new Size(90, 30);
      btnNext.TabIndex = 4;
      btnNext.Text = "Next";
      btnNext.UseVisualStyleBackColor = true;
      btnNext.Click += btnNext_Click;
      // 
      // btnCancel
      // 
      btnCancel.DialogResult = DialogResult.Cancel;
      btnCancel.Location = new Point(396, 349);
      btnCancel.Name = "btnCancel";
      btnCancel.Size = new Size(96, 30);
      btnCancel.TabIndex = 3;
      btnCancel.Text = "Cancel";
      btnCancel.UseVisualStyleBackColor = true;
      // 
      // lbAtDesk
      // 
      lbAtDesk.AutoSize = true;
      lbAtDesk.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      lbAtDesk.Location = new Point(20, 24);
      lbAtDesk.Name = "lbAtDesk";
      lbAtDesk.Size = new Size(92, 21);
      lbAtDesk.TabIndex = 2;
      lbAtDesk.Text = "At Desk: NA";
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(40, 60);
      label1.Name = "label1";
      label1.Size = new Size(432, 19);
      label1.TabIndex = 1;
      label1.Text = "In text below, Add the for each line to append to the prompt in next. ";
      // 
      // edForeach
      // 
      edForeach.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edForeach.Location = new Point(71, 102);
      edForeach.Multiline = true;
      edForeach.Name = "edForeach";
      edForeach.Size = new Size(685, 175);
      edForeach.TabIndex = 0;
      // 
      // tabTodo
      // 
      tabTodo.Controls.Add(label3);
      tabTodo.Controls.Add(cbType);
      tabTodo.Controls.Add(lbName);
      tabTodo.Controls.Add(cbValues);
      tabTodo.Controls.Add(lbItemDemo);
      tabTodo.Controls.Add(edPromptTemplate);
      tabTodo.Controls.Add(label2);
      tabTodo.Controls.Add(btnBack);
      tabTodo.Controls.Add(btnOk);
      tabTodo.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
      tabTodo.Location = new Point(4, 24);
      tabTodo.Name = "tabTodo";
      tabTodo.Padding = new Padding(3);
      tabTodo.Size = new Size(792, 422);
      tabTodo.TabIndex = 1;
      tabTodo.Text = "Add Todo";
      tabTodo.UseVisualStyleBackColor = true;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(35, 276);
      label3.Name = "label3";
      label3.Size = new Size(66, 20);
      label3.TabIndex = 13;
      label3.Text = "Ref Type";
      // 
      // cbType
      // 
      cbType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      cbType.FormattingEnabled = true;
      cbType.Location = new Point(114, 273);
      cbType.Name = "cbType";
      cbType.Size = new Size(371, 27);
      cbType.TabIndex = 12;
      cbType.SelectedIndexChanged += cbType_SelectedIndexChanged;
      // 
      // lbName
      // 
      lbName.AutoSize = true;
      lbName.Location = new Point(36, 310);
      lbName.Name = "lbName";
      lbName.Size = new Size(65, 20);
      lbName.TabIndex = 11;
      lbName.Text = "Ref Item";
      // 
      // cbValues
      // 
      cbValues.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      cbValues.FormattingEnabled = true;
      cbValues.Location = new Point(114, 306);
      cbValues.Name = "cbValues";
      cbValues.Size = new Size(371, 27);
      cbValues.TabIndex = 10;
      // 
      // lbItemDemo
      // 
      lbItemDemo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      lbItemDemo.Location = new Point(30, 214);
      lbItemDemo.Name = "lbItemDemo";
      lbItemDemo.Size = new Size(723, 48);
      lbItemDemo.TabIndex = 9;
      lbItemDemo.Text = "then: ";
      // 
      // edPromptTemplate
      // 
      edPromptTemplate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edPromptTemplate.AutoCompleteBracketsList = new char[]
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
      edPromptTemplate.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
      edPromptTemplate.AutoScrollMinSize = new Size(0, 16);
      edPromptTemplate.BackBrush = null;
      edPromptTemplate.CharHeight = 16;
      edPromptTemplate.CharWidth = 9;
      edPromptTemplate.DefaultMarkerSize = 8;
      edPromptTemplate.DisabledColor = Color.FromArgb(100, 180, 180, 180);
      edPromptTemplate.FindForm = null;
      edPromptTemplate.Font = new Font("Courier New", 10.8F);
      edPromptTemplate.GoToForm = null;
      edPromptTemplate.Hotkeys = resources.GetString("edPromptTemplate.Hotkeys");
      edPromptTemplate.IsReplaceMode = false;
      edPromptTemplate.Location = new Point(42, 49);
      edPromptTemplate.Name = "edPromptTemplate";
      edPromptTemplate.Paddings = new Padding(0);
      edPromptTemplate.ReplaceForm = null;
      edPromptTemplate.SelectionColor = Color.FromArgb(60, 0, 0, 255);
      edPromptTemplate.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("edPromptTemplate.ServiceColors");
      edPromptTemplate.Size = new Size(711, 150);
      edPromptTemplate.TabIndex = 8;
      edPromptTemplate.WordWrap = true;
      edPromptTemplate.Zoom = 100;
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      label2.Location = new Point(18, 15);
      label2.Name = "label2";
      label2.Size = new Size(508, 21);
      label2.TabIndex = 7;
      label2.Text = "Write the prompt below that will duplicate with line from previous page.";
      // 
      // btnBack
      // 
      btnBack.Location = new Point(276, 349);
      btnBack.Name = "btnBack";
      btnBack.Size = new Size(90, 30);
      btnBack.TabIndex = 6;
      btnBack.Text = "Back";
      btnBack.UseVisualStyleBackColor = true;
      btnBack.Click += btnBack_Click;
      // 
      // btnOk
      // 
      btnOk.DialogResult = DialogResult.OK;
      btnOk.Location = new Point(396, 349);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(96, 30);
      btnOk.TabIndex = 5;
      btnOk.Text = "OK";
      btnOk.UseVisualStyleBackColor = true;
      // 
      // AddTodoForeachDialog
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(800, 450);
      Controls.Add(tabState);
      Name = "AddTodoForeachDialog";
      StartPosition = FormStartPosition.CenterParent;
      Text = "AddTodoForeachDialog";
      Shown += AddTodoForeachDialog_Shown;
      tabState.ResumeLayout(false);
      tabForeach.ResumeLayout(false);
      tabForeach.PerformLayout();
      tabTodo.ResumeLayout(false);
      tabTodo.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)edPromptTemplate).EndInit();
      ResumeLayout(false);
    }

    #endregion

    private TabControl tabState;
    private TabPage tabForeach;
    private TabPage tabTodo;
    private TextBox edForeach;
    private Button btnNext;
    private Button btnCancel;
    private Label lbAtDesk;
    private Label label1;
    private Button btnBack;
    private Button btnOk;
    private Label lbItemDemo;
    private FastColoredTextBoxNS.FastColoredTextBox edPromptTemplate;
    private Label label2;
    private Label label3;
    private ComboBox cbType;
    private Label lbName;
    private ComboBox cbValues;
  }
}