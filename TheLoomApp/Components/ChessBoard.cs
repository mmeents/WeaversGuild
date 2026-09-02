using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TheLoomApp.Components {
  public partial class ChessBoard : UserControl {
    private TableLayoutPanel _grid;
    private Button[,] _squares = new Button[8, 8];
    private string _selectedSquare = null; // Stores first click (e.g., "e2")

    // Expose an event so your main form knows when a human completed a move
    public event Action<string> MoveCompleted;
    public ChessBoard() {
      InitializeComponent();
      InitializeBoardLayout();
    }
    private System.ComponentModel.IContainer components = null;
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    private void InitializeComponent() {
      SuspendLayout();
      // 
      // ChessBoard
      // 
      AutoScaleDimensions = new SizeF(8F, 20F);
      AutoScaleMode = AutoScaleMode.Font;
      Name = "ChessBoard";
      Size = new Size(553, 586);
      ResumeLayout(false);
    }

    private void InitializeBoardLayout() {
      this.Size = new Size(400, 400);

      _grid = new TableLayoutPanel {
        Dock = DockStyle.Fill,
        RowCount = 8,
        ColumnCount = 8
      };

      // Set uniform 12.5% width and height for an exact 8x8 grid
      for (int i = 0; i < 8; i++) {
        _grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f));
        _grid.RowStyles.Add(new RowStyle(SizeType.Percent, 12.5f));
      }

      // Generate the squares from White's perspective (Rank 8 at top, Rank 1 at bottom)
      for (int rank = 7; rank >= 0; rank--) {
        for (int file = 0; file < 8; file++) {
          char fileChar = (char)('a' + file);
          int rankNum = rank + 1;
          string squareName = $"{fileChar}{rankNum}";

          var square = new Button {
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Symbol", 24, FontStyle.Regular), // Scaled for Unicode pieces
            Tag = squareName // Critical: Store the chess coordinate in the button itself
          };

          // Alternate square colors
          square.BackColor = (rank + file) % 2 == 0 ? Color.DarkGray : Color.White;
          square.FlatAppearance.BorderSize = 0;

          // Wire up the click event
          square.Click += Square_Click;

          _squares[file, rank] = square;

          // TableLayoutPanel grid mapping (File = Column, (7 - rank) = Row)
          _grid.Controls.Add(square, file, 7 - rank);
        }
      }

      this.Controls.Add(_grid);
    }

    private void Square_Click(object sender, EventArgs e) {
      if (sender is Button clickedButton && clickedButton.Tag is string squareCoord) {
        if (_selectedSquare == null) {
          // FIRST CLICK: Select piece to move
          _selectedSquare = squareCoord;
          clickedButton.BackColor = Color.LightYellow; // Highlight selection
        } else {
          // SECOND CLICK: Destination chosen
          string fromSquare = _selectedSquare;
          string toSquare = squareCoord;

          // Reset visual highlights
          ResetSquareColors();
          _selectedSquare = null;

          // If they clicked the same square twice, cancel the selection
          if (fromSquare == toSquare) return;

          // Construct the standard UCI string (e.g. "e2e4") and fire it off
          string uciMove = $"{fromSquare}{toSquare}";
          MoveCompleted?.Invoke(uciMove);
        }
      }
    }

    /// <summary>
    /// Updates the visual pieces on the grid using Rudzoft's current position state.
    /// </summary>
    public void SyncWithGame(IGame game) {
      var position = game.Pos;

      for (int rank = 0; rank < 8; rank++) {
        for (int file = 0; file < 8; file++) {
          // Query Rudzoft library for what piece occupies this specific index
          Square sq = new Square(rank, file);
          Piece piece = position.GetPiece(sq);

          // Map library piece types to clean Unicode text glyphs
          _squares[file, rank].Text = GetPieceUnicode(piece);
        }
      }
    }

    private void ResetSquareColors() {
      for (int rank = 0; rank < 8; rank++) {
        for (int file = 0; file < 8; file++) {
          _squares[file, rank].BackColor = (rank + file) % 2 == 0 ? Color.DarkGray : Color.White;
        }
      }
    }





    private string GetPieceUnicode(Piece piece) {
      // Maps Rudzoft library pieces directly to universal unicode symbols
      return piece.ToString() switch {
        "P" => "♙",
        "R" => "♖",
        "N" => "♘",
        "B" => "♗",
        "Q" => "♕",
        "K" => "♔", // White
        "p" => "♟",
        "r" => "♜",
        "n" => "♞",
        "b" => "♝",
        "q" => "♛",
        "k" => "♚", // Black
        _ => "" // Empty square
      };
    }
  }
}
