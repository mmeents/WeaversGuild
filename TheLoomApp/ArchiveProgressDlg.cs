using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Weavers.Core.Handlers.AssetArchive;
using Weavers.Core.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TheLoomApp {
  public partial class ArchiveProgressDlg : Form {
    private readonly ItemDto _archiveItem;
    private readonly IMediator _mediator;
    private readonly CancellationTokenSource _cts;
    private bool _running = false;

    public ArchiveProgressDlg(IMediator mediator, ItemDto archiveItem, CancellationToken outerCt = default) {
      InitializeComponent();
      this._mediator = mediator;
      this._archiveItem = archiveItem;
      this._cts = CancellationTokenSource.CreateLinkedTokenSource(outerCt);
    }

    private async void ArchiveProgressDlg_Shown(object sender, EventArgs e) {
      progressBar.Style = ProgressBarStyle.Marquee;
      progressBar.MarqueeAnimationSpeed = 30;
      label1.Text = "Starting…";
      btnCancel.Text = "Cancel";
      _running = true;

      try {
        var progress = new Progress<SyncAssetArchiveProgress>(p => {
          if (!_running || IsDisposed) return;   // late report after completion/close
          label1.Text = $"Scanned {p.Scanned:N0} · unchanged {p.Unchanged:N0} · hashed {p.Hashed:N0} · errors {p.Errors:N0}";
        });

        var command = new SyncAssetArchiveCommand(_archiveItem.Id) { Progress = progress };
        var result = await Task.Run(() => _mediator.Send(command, _cts.Token), _cts.Token);

        _running = false;
        label1.Text = result.Message ?? "";
        if (result.ErrorSamples.Count > 0)
          MessageBox.Show(this, string.Join(Environment.NewLine, result.ErrorSamples),
                          $"{result.Errors:N0} files had errors");
      } catch (OperationCanceledException) { _running = false; label1.Text = "Sync cancelled. Nothing was marked missing."; } 
        catch (Exception ex) { _running = false; label1.Text = "Sync failed."; MessageBox.Show(this, ex.Message, "Sync failed"); } 
        finally {
        _running = false;
        progressBar.Style = ProgressBarStyle.Blocks;
        progressBar.Value = progressBar.Maximum;
        btnCancel.Text = "Close";
        btnCancel.Enabled = true;       
      }
    }

    private void btnCancel_Click(object sender, EventArgs e) {
      if (_running) {
        _cts.Cancel();
        btnCancel.Enabled = false;
        label1.Text = "Cancelling… finishing current batch";
      } else {
        Close();
      }
    }
    private void ArchiveProgressDlg_FormClosing(object sender, FormClosingEventArgs e) {
      if (!_running) return;
      e.Cancel = true;                   // stay open until the handler unwinds
      btnCancel_Click(this, EventArgs.Empty);
    }

    protected override void OnFormClosed(FormClosedEventArgs e) {
      _cts.Dispose();
      base.OnFormClosed(e);
    }

  }
}
