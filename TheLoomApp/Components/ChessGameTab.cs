using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Weavers.Core.Models;
using Weavers.Core.Handlers.Chess;
using System.Drawing;
using System.Windows.Forms;
using MediatR;
using Weavers.Core.Extensions;
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Types;
using Rudzoft.ChessLib.Enums;
using Rudzoft.ChessLib.Fen;
using Rudzoft.ChessLib.MoveGeneration;

namespace TheLoomApp.Components {
  public class ChessGameTab : TabPage {
    private string _titleLabel = "Chess Game";
    private string _moveHistoryText = "";
    private Label gameStatus;
    private Panel basePanel;
    private ChessBoard chessBoard1;
    private Label moveHistory;
    private Label lbNextMove;
    private ComboBox cbNextMove;
    private NumericUpDown edTodoId;
    private Button btnDoNextMove;
    private readonly IServiceScopeFactory _scopeFactory;
    private ItemDto? _item = null;

    public event Action<string> MoveCompleted;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string TitleLabel {
      get => _titleLabel;
      set {
        if (_titleLabel != value) {
          _titleLabel = value;
          this.Text = _titleLabel; // Update the TabPage's text          
        }
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string MoveHistoryText {
      get => _moveHistoryText;
      set {
        if (_moveHistoryText != value) {
          _moveHistoryText = value;
          moveHistory.Text = _moveHistoryText;
        }
      }
    }

    public ChessGameTab(IServiceScopeFactory scopeFactory) {
      _scopeFactory = scopeFactory;
      InitializeComponent();
    }

    private void InitializeComponent() {
      basePanel = new Panel();
      chessBoard1 = new ChessBoard();
      gameStatus = new Label();
      lbNextMove = new Label();
      moveHistory = new Label();
      cbNextMove = new ComboBox();
      edTodoId = new NumericUpDown();
      btnDoNextMove = new Button();
      basePanel.SuspendLayout();
      SuspendLayout();
      // 
      // basePanel
      //       
      basePanel.Name = "basePanel";
      basePanel.Size = new Size(200, 100);
      basePanel.TabIndex = 0;
      basePanel.TabStop = false;
      // 
      // gameStat
      // 
      gameStatus.Name = "gameStat";
      gameStatus.Size = new Size(200, 20);
      gameStatus.TabIndex = 1;
      // 
      // chessBoard1
      // 
      chessBoard1.Margin = new Padding(3, 2, 3, 2);
      chessBoard1.Name = "chessBoard1";
      chessBoard1.Size = new Size(200, 100);
      chessBoard1.TabIndex = 0;
      chessBoard1.MoveCompleted += chessBoard1_MoveCompleted;
      // 
      // moveHistory
      // 
      moveHistory.Name = "moveHistory";
      moveHistory.Size = new Size(200, 20);
      moveHistory.TabIndex = 2;
      //
      // lbNextMove
      //
      lbNextMove.Name = "lbNextMove";
      lbNextMove.Size = new Size(150, 20);
      lbNextMove.TabIndex = 4;
      lbNextMove.AutoSize = true;
      //
      // cbNextMove
      //
      cbNextMove.Name = "cbNextMove";
      cbNextMove.Size = new Size(240, 20);
      cbNextMove.TabIndex = 3;
      //
      // edTodoId
      //
      edTodoId.Name = "edTodoId";
      edTodoId.Size = new Size(80, 20);
      edTodoId.Minimum = 1;
      edTodoId.Maximum = int.MaxValue;
      edTodoId.Value = 1;
      //
      // btnDoNextMove
      //
      btnDoNextMove.Name = "btnDoNextMove";
      btnDoNextMove.Size = new Size(140, 20);
      btnDoNextMove.TabIndex = 2;
      btnDoNextMove.Text = "Manual Move";
      btnDoNextMove.Click += btnDoNextMove_Click;
      btnDoNextMove.Visible = true;
      btnDoNextMove.Enabled = true;
      // 
      // ChessGameTab
      //
      Controls.Add(basePanel);      
      basePanel.Controls.Add(gameStatus);
      basePanel.Controls.Add(chessBoard1);
      basePanel.Controls.Add(moveHistory);
      basePanel.Controls.Add(cbNextMove);
      basePanel.Controls.Add(lbNextMove);
      basePanel.Controls.Add(edTodoId);
      basePanel.Controls.Add(btnDoNextMove);
      Resize += ChessGameTab_Resize;
      basePanel.ResumeLayout(false);
      ResumeLayout(false);

    }


    public async void SetupChessTabForItem(ItemDto item) {
      _item = item;
      var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var position = scope.ServiceProvider.GetRequiredService<IPosition>();
      var game = position.GetCurrentGame(item);
      if (game != null) {
        chessBoard1.SyncWithGame(game);      
      }
      var sideToMove = game.Pos.SideToMove.IsWhite ? "White" : "Black";
      TitleLabel = $"Chess Game - {item.Name}";
      gameStatus.Text = _titleLabel+ $" - {sideToMove} to move";
      var playerMoves = JsonSerializer.Deserialize<List<MoveRecord>>(item.Data) ?? new List<MoveRecord>();
      MoveHistoryText = "Moves Made: "+string.Join(",", playerMoves.Select(m => m.Move.ToString()));
      var ml = game.Pos.GenerateMoves();
      cbNextMove.Items.Clear();
      cbNextMove.Items.AddRange(ml.Select(m => new MoveOption(m.Move)).ToArray());
      lbNextMove.Text = "Set Next Moves, TodoId:  ";
      ChessGameTab_Resize(this, EventArgs.Empty);
    }

    private void ChessGameTab_Resize(object sender, EventArgs e) {
      var measuredSize = this.Size;
      var charHeight = gameStatus.Height;
      if (basePanel != null && chessBoard1 != null) {
        basePanel.Size = new Size(measuredSize.Width - 5, measuredSize.Height - 5 );                
        chessBoard1.Location = new Point(10, basePanel.Top + 10 + charHeight);
        chessBoard1.Size = new Size(basePanel.Width - 20, basePanel.Height - 80 - charHeight);
        
        gameStatus.Location = new Point(10, basePanel.Top + 5);
        gameStatus.Width = chessBoard1.Width;

        moveHistory.Location = new Point(10, chessBoard1.Top + chessBoard1.Height + 5);
        moveHistory.Width = chessBoard1.Width;

        lbNextMove.Location = new Point(10, moveHistory.Top + moveHistory.Height + 5);

        cbNextMove.Location = new Point(lbNextMove.Width, moveHistory.Top + moveHistory.Height + 5);
        edTodoId.Location = new Point(cbNextMove.Left+cbNextMove.Width+ 5, moveHistory.Top + moveHistory.Height + 5);
        btnDoNextMove.Location = new Point(edTodoId.Left + edTodoId.Width + 25, moveHistory.Top + moveHistory.Height + 5);
        btnDoNextMove.Height = cbNextMove.Height;


      }
    }

    private void chessBoard1_MoveCompleted(string obj) {
      if (_item == null) {
        MessageBox.Show($"No game item is currently loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      var scope = _scopeFactory.CreateScope();
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var position = scope.ServiceProvider.GetRequiredService<IPosition>();
      var game = position.GetCurrentGame(_item);
      if (game.Pos.isMoveValid(obj, out Move move)) {        
        cbNextMove.SelectedIndex = cbNextMove.Items.IndexOf(new MoveOption(move));        
      } else {
        MessageBox.Show($"Invalid move attempted: {obj}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private async void btnDoNextMove_Click(object sender, EventArgs e) {
      if (_item != null && cbNextMove.SelectedItem != null && edTodoId != null) {
        try {
          var scope = _scopeFactory.CreateScope();
          var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
          string nextMove = cbNextMove.SelectedItem.ToString();
          var result = await mediator.Send(new ChessMakeMoveCommand(_item.Id, nextMove, (int)edTodoId.Value));
          edTodoId.Value = result.TodoId ?? edTodoId.Value;
          MoveCompleted?.Invoke(nextMove);
        } catch(Exception ex) { 
          MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }        
      } else { 
        MessageBox.Show($"Please select a move and ensure TodoId is valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }      
    }

    private sealed record MoveOption(Move Move) {
      public override string ToString() => Move.ToString();
    }
  }
}
