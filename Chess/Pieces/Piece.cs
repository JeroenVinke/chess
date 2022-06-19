using Chess.Enums;
using Chess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Pieces
{
    public abstract class Piece : NotifyPropertyChangedBase
    {

        protected Square _originalSquare;
        public Square ActiveSquare { get; set; }
        public PieceColor Color { get; set; }
        public MoveDirection MoveDirection { get; set; }
        public string Image { get; set; }
        public PieceColor EnemyColor => Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        public abstract int Value { get; }
        public Board Board { get; set; }
        public int X => ActiveSquare.X;
        public int Y => ActiveSquare.Y;
        public bool HasMoved { get; set; }

        public Piece(Square square, PieceColor color, string image)
        {
            Board = square.Board;
            ActiveSquare = square;
            Color = color;
            Image = "pack://application:,,,/Resources/" + image;
            square.ActivePiece = this;
            _originalSquare = square;

            MoveDirection = Color == PieceColor.White ? MoveDirection.Up : MoveDirection.Down;
        }

        public virtual List<Square> AttackingSquares() => LegalMoves();

        public abstract List<Square> LegalMoves();

        public List<Square> GetEffectiveReachableSquares()
        {
            if (!Board.IsVirtualBoard && Board.ColorToMove != Color)
            {
                return new List<Square>();
            }

            King king = (King)Board.Pieces.First(x => x is King && x.Color == Color);

            var reachableSquares = LegalMoves();
            List<Square> result = new List<Square>();

            foreach (var square in reachableSquares)
            {
                Board cloned = Board.Clone();

                cloned.Rows[Y][X].ActivePiece.MoveTo(cloned.Rows[square.Y][square.X]);

                King virtualKing = cloned.Pieces.Where(x => x.Color == Color && x is King).Cast<King>().First();

                if (!virtualKing.IsInCheck())
                {
                    result.Add(square);
                }
            }

            return result;
        }

        internal void MoveTo(Square toSquare)
        {
            Square fromSquare = ActiveSquare;

            if (toSquare.IsOccupied && toSquare.ActivePiece.Color == EnemyColor)
            {
                Capture(toSquare.ActivePiece);
            }

            HasMoved = true;
            ActiveSquare.ActivePiece = null;
            ActiveSquare.IsSelected = false;
            toSquare.ActivePiece = this;

            OnAfterMove(fromSquare, toSquare);
        }

        public abstract Piece Clone(Square square, PieceColor color);

        protected virtual void OnAfterMove(Square from, Square to)
        {
            Board.Yield();
        }

        private void Capture(Piece piece)
        {
            ActiveSquare.Board.OnPieceCapture(piece);
        }

        internal bool CanMoveTo(Square square)
        {
            if (Board.ColorToMove != Color)
            {
                return false;
            }

            if (GetEffectiveReachableSquares().Contains(square))
            {
                return true;
            }

            return false;
        }

        protected virtual bool IsPinned()
        {
            if (!Board.IsVirtualBoard)
            {
                Board cloned = Board.Clone();

                cloned.Rows[Y][X].ActivePiece.Vanish();

                King king = cloned.Pieces.Where(x => x.Color == Color && x is King).Cast<King>().First();

                if (king.IsInCheck())
                {
                    return true;
                }

                return false;
            }
            else
            {
                King king = Board.Pieces.Where(x => x.Color == Color && x is King).Cast<King>().First();

                if (king.IsInCheck())
                {
                    return true;
                }

                return false;
            }
        }

        public void Vanish()
        {
            Board.RemovePiece(this);
            ActiveSquare.ActivePiece = null;
        }
    }
}
