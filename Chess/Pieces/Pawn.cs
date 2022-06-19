using Chess.Enums;
using Chess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Pieces
{
    public class Pawn : Piece
    {
        public override int Value => 1;

        public Pawn(Square square, PieceColor color) : base(square, color, color == PieceColor.White ? "10.png" : "11.png")
        {
        }

        public override Piece Clone(Square square, PieceColor color)
        {
            return new Pawn(square, color);
        }

        public override List<Square> LegalMoves()
        {
            List<Square> result = new List<Square>();

            Func<Square, int, Square> y = (s, amount) => MoveDirection == MoveDirection.Up ? s.Above(amount) : s.Below(amount);

            Square above = y(ActiveSquare, 1);

            if (above != null && !above.IsOccupied)
            {
                result.Add(above);

                if (_originalSquare == ActiveSquare && y(ActiveSquare, 2) != null && !y(ActiveSquare, 2).IsOccupied)
                {
                    result.Add(y(ActiveSquare, 2));
                }
            }

            if (above != null)
            {
                if (above.Left() != null && above.Left().ContainsPieceOfColor(EnemyColor))
                {
                    result.Add(above.Left());
                }
                if (above.Right() != null && above.Right().ContainsPieceOfColor(EnemyColor))
                {
                    result.Add(above.Right());
                }
            }

            return result;
        }

        protected override void OnAfterMove(Square from, Square to)
        {
            base.OnAfterMove(from, to);

            if (MoveDirection == MoveDirection.Up && ActiveSquare.Above() == null
                || MoveDirection == MoveDirection.Down && ActiveSquare.Below() == null)
            {
                Promote();
            }
        }

        private void Promote()
        {
            ActiveSquare.Board.RemovePiece(this);

            Queen queen = new Queen(ActiveSquare, Color);
            ActiveSquare.Board.AddPiece(queen);
            ActiveSquare.ActivePiece = queen;
        }
    }
}
