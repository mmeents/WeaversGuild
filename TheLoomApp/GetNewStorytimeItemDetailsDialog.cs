using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Weavers.Core.Enums;
using Weavers.Core.Handlers.ItemTypes;
using Weavers.Core.Extensions;
using TheLoomApp.Models;
using System.ComponentModel;

namespace TheLoomApp {
  public partial class GetNewStorytimeItemDetailsDialog : Form {
    private readonly IServiceScopeFactory serviceScopeFactory;
    private WeItemType _targetTypeToCreate;
    private TreeView _tvKb;

    public string ItemName => edName.Text;

    public string Description => edDescription.Text;    
    public int? PovTypeId => cbPov.SelectedValue as int?;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int TargetSceneCount {
      get { return edTargetSceneCount.Value.AsInt(); }
      set { edTargetSceneCount.Value = value; }
    }

    public string EntryState => edEntryState.Text;
    public string ExitState => edExitState.Text;
    public string Tone => edEntryState.Text;

    public GetNewStorytimeItemDetailsDialog(
      IServiceScopeFactory serviceScopeFactory, 
      WeItemType targetTypeToCreate, 
      TreeView tvKb
    ) {
      this.serviceScopeFactory = serviceScopeFactory;
      this._targetTypeToCreate = targetTypeToCreate;
      this._tvKb = tvKb;
      var selctedNode = _tvKb.SelectedNode as ItemNode;
      InitializeComponent();

      this.Text = $"Create New {_targetTypeToCreate.ToString()}";
      lbAddTarget.Text = $"Please enter the following for the new {_targetTypeToCreate.ToString()}";

      
      edTargetSceneCount.Visible = false;
      lbTargetSceneCount.Visible = false;
      lbPov.Visible = false;
      cbPov.Visible = false;
      lbEntry.Visible = false;
      edEntryState.Visible = false;
      lbExit.Visible = false;
      edExitState.Visible = false;

      if (_targetTypeToCreate == WeItemType.RealmModel) {
        lbEntry.Top = edName.Top + edName.Height + 20;
        edEntryState.Top = lbEntry.Top;
        lbEntry.Text = "Tone:";
        lbEntry.Visible = true;
        edEntryState.Visible = true;

        var top = edEntryState.Top + edEntryState.Height + 20;
        edDescription.Top = top;
        lbDescription.Top = top;

      } else if (_targetTypeToCreate == WeItemType.StoryModel) {

        edTargetSceneCount.Visible = true;
        lbTargetSceneCount.Visible = true;
        lbPov.Visible = true;
        cbPov.Visible = true;
        loadPovs();
        var top = cbPov.Top + cbPov.Height + 20;
        edDescription.Top = top;
        lbDescription.Top = top;

      } else if (_targetTypeToCreate == WeItemType.SceneModel) {

        lbPov.Visible = true;
        lbPov.Text = "POV Type:";
        cbPov.Visible = true;
        loadPovs();

        lbEntry.Visible = true;
        edEntryState.Visible = true;
        lbExit.Visible = true;
        edExitState.Visible = true;

        var top = edExitState.Top + edExitState.Height + 20;
        edDescription.Top = top;
        lbDescription.Top = top;
      } else {
        var top = edName.Top + edName.Height + 20;
        edDescription.Top = top;
        lbDescription.Top = top;
      }

      this.Height = edDescription.Top + edDescription.Height + (btnCancel.Height * 3);
      _tvKb = tvKb;
    }

    private void btnOk_Click(object sender, EventArgs e) {
      if (string.IsNullOrWhiteSpace(ItemName)) {
        MessageBox.Show("Please enter a name for the new item.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        this.DialogResult = DialogResult.None;
        return;
      }
    }

    private async void loadPovs() {
      using var scope = serviceScopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var povs = await mediator.Send(new GetItemsByItemTypeQuery((int)WeItemType.PovTypes));
      cbPov.DataSource = povs;
      cbPov.DisplayMember = "Description";
      cbPov.ValueMember = "Value";
    }
  }
}
