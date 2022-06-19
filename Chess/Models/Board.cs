using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Chess.Enums;
using Chess.Pieces;

namespace Chess.Models
{
    public class Board : NotifyPropertyChangedBase
    {
        public Dictionary<int, List<Square>> Rows { get; set; }
        public Dictionary<int, List<Square>> Columns { get; set; }
        public List<Piece> Pieces { get; set; }
        public PieceColor ColorToMove { get; set; } = PieceColor.White;
        public event EventHandler OnCapture;
        public event EventHandler OnPieceAdd;
        public event EventHandler OnPieceRemove;

        public Dictionary<PieceColor, List<Piece>> CapturedPieces = new Dictionary<PieceColor, List<Piece>>();

        public bool IsVirtualBoard { get; set; }

        public Board()
        {
            Pieces = new List<Piece>();

            CapturedPieces[PieceColor.White] = new List<Piece>();
            CapturedPieces[PieceColor.Black] = new List<Piece>();

            CreateCells();
        }

        public Board Clone()
        {
            Board cloned = new Board();
            cloned.IsVirtualBoard = true;
            cloned.ColorToMove = ColorToMove;

            foreach (Piece piece in Pieces)
            {
                Piece clonedPiece = piece.Clone(cloned.Rows[piece.ActiveSquare.Y][piece.ActiveSquare.X], piece.Color);

                cloned.AddPiece(clonedPiece);
            }

            return cloned;
        }

        public IEnumerable<Square> AllSquares()
        {
            List<Square> squares = new List<Square>();
            squares.AddRange(Rows.Values.SelectMany(x => x));
            squares.AddRange(Columns.Values.SelectMany(x => x));

            return squares;
        }

        public void CreatePieces()
        {
            CreatePieces(0, PieceColor.White);
            CreatePieces(7, PieceColor.Black);

            CreatePawns();
        }

        private void CreatePieces(int row, PieceColor color)
        {
            Pieces.Add(new Rook(Rows[row][0], color));
            Pieces.Add(new Knight(Rows[row][1], color));
            Pieces.Add(new Bishop(Rows[row][2], color));
            Pieces.Add(new Queen(Rows[row][3], color));
            Pieces.Add(new King(Rows[row][4], color));
            Pieces.Add(new Bishop(Rows[row][5], color));
            Pieces.Add(new Knight(Rows[row][6], color));
            Pieces.Add(new Rook(Rows[row][7], color));
        }

        internal void Yield()
        {
            ColorToMove = ColorToMove == PieceColor.White ? PieceColor.Black : PieceColor.White;

            if (!IsVirtualBoard)
            {
                King king = (King)Pieces.First(x => x is King && x.Color == ColorToMove);

                if (king != null)
                {
                    if (king.IsInCheckMate())
                    {
                        MessageBox.Show("Checkmate!");
                    }
                    else if (king.IsInStaleMate())
                    {
                        MessageBox.Show("Stalemate!");
                    }
                    else if (king.IsInCheck())
                    {
                        System.Diagnostics.Debug.WriteLine("Check!");
                    }
                }
            }
        }

        internal void AddPiece(Piece piece)
        {
            Pieces.Add(piece);
            OnPieceAdd?.Invoke(this, null);
        }

        internal void RemovePiece(Piece piece)
        {
            Pieces.Remove(piece);
            OnPieceRemove?.Invoke(this, null);
        }

        internal void OnPieceCapture(Piece piece)
        {
            CapturedPieces[piece.EnemyColor].Add(piece);
            OnCapture?.Invoke(this, null);
            RemovePiece(piece);
        }

        private void CreatePawns()
        {
            for (int i = 0; i < Columns.Count; i++)
            {
                Pieces.Add(new Pawn(Rows[1][i], PieceColor.White));
            }
            for (int i = 0; i < Columns.Count; i++)
            {
                Pieces.Add(new Pawn(Rows[6][i], PieceColor.Black));
            }
        }

        private void CreateCells()
        {
            Rows = new Dictionary<int, List<Square>>();
            Columns = new Dictionary<int, List<Square>>();

            int width = 8;
            int height = 8;

            for (int x = 0; x < width; x++)
            {
                if (!Columns.ContainsKey(x))
                {
                    Columns[x] = new List<Square>();
                }

                for (int y = 0; y < height; y++)
                {
                    if (!Rows.ContainsKey(y))
                    {
                        Rows[y] = new List<Square>();
                    }

                    Square square = new Square(this, x, y);
                    Rows[y].Insert(x, square);
                    Columns[x].Insert(y, square);
                    square.Row = Rows[y];
                    square.Column = Columns[x];
                }
            }
        }
    }

}
