using Chess.Enums;
using Chess.Models;
using System.Collections.Generic;

namespace Chess.Pieces
{
    public class Queen : Piece
    {
        public override int Value => 9;

        public Queen(Square square, PieceColor color) : base(square, color, color == PieceColor.White ? "1.png" : "0.png")
        {
        }

        public override Piece Clone(Square square, PieceColor color)
        {
            return new Queen(square, color);
        }

        public override List<Square> LegalMoves()
        {
            List<Square> result = new List<Square>();

            AddIfCanMove(ActiveSquare.AllAbove(), result);
            AddIfCanMove(ActiveSquare.AllLeft(), result);
            AddIfCanMove(ActiveSquare.AllRight(), result);
            AddIfCanMove(ActiveSquare.AllBelow(), result);

            AddIfCanMove(ActiveSquare.DiagonalLeftDown(), result);
            AddIfCanMove(ActiveSquare.DiagonalLeftUp(), result);
            AddIfCanMove(ActiveSquare.DiagonalRightDown(), result);
            AddIfCanMove(ActiveSquare.DiagonalRightUp(), result);

            return result;
        }

        private void AddIfCanMove(IEnumerable<Square> squares, List<Square> result)
        {
            foreach (Square square in squares)
            {
                if (square.IsOccupied)
                {
                    if (square.ContainsPieceOfColor(EnemyColor))
                    {
                        result.Add(square);
                    }

                    break;
                }

                result.Add(square);
            }
        }
    }
}
