namespace TheLoomApp {
  partial class ArchiveProgressDlg {
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
      label1 = new Label();
      progressBar = new ProgressBar();
      btnCancel = new Button();
      SuspendLayout();
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
      label1.Location = new Point(38, 27);
      label1.Name = "label1";
      label1.Size = new Size(50, 20);
      label1.TabIndex = 0;
      label1.Text = "label1";
      // 
      // progressBar
      // 
      progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      progressBar.Location = new Point(28, 109);
      progressBar.Name = "progressBar";
      progressBar.Size = new Size(550, 23);
      progressBar.TabIndex = 1;
      // 
      // btnCancel
      // 
      btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      btnCancel.Location = new Point(503, 80);
      btnCancel.Name = "btnCancel";
      btnCancel.Size = new Size(75, 23);
      btnCancel.TabIndex = 2;
      btnCancel.Text = "Cancel";
      btnCancel.UseVisualStyleBackColor = true;
      btnCancel.Click += btnCancel_Click;
      // 
      // ArchiveProgressDlg
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(606, 156);
      Controls.Add(btnCancel);
      Controls.Add(progressBar);
      Controls.Add(label1);
      Name = "ArchiveProgressDlg";
      StartPosition = FormStartPosition.CenterParent;
      Text = "ArchiveProgressDlg";
      FormClosing += ArchiveProgressDlg_FormClosing;
      Shown += ArchiveProgressDlg_Shown;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label label1;
    private ProgressBar progressBar;
    private Button btnCancel;
  }
}