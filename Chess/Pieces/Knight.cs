using Chess.Enums;
using Chess.Models;
using System;
using System.Collections.Generic;

namespace Chess.Pieces
{
    public class Knight : Piece
    {
        public override int Value => 3;

        public Knight(Square square, PieceColor color) : base(square, color, color == PieceColor.White ? "6.png" : "7.png")
        {
        }

        public override Piece Clone(Square square, PieceColor color)
        {
            return new Knight(square, color);
        }

        public override List<Square> LegalMoves()
        {
            List<Square> result = new List<Square>();

            List<Square> candidateSquares = new List<Square>();
            candidateSquares.Add(GetSquare(ActiveSquare, x => x.Left(2), y => y.Above(1)));
            candidateSquares.Add(GetSquare(ActiveSquare, x => x.Left(2), y => y.Below(1)));
            candidateSquares.Add(GetSquare(ActiveSquare, x => x.Right(2), y => y.Above(1)));
            candidateSquares.Add(GetSquare(ActiveSquare, x => x.Right(2), y => y.Below(1)));
            candidateSquares.Add(GetSquare(ActiveSquare, x => x.Left(1), y => y.Above(2)));
            candidateSquares.Add(GetSquare(ActiveSquare, x => x.Right(1), y => y.Above(2)));
            candidateSquares.Add(GetSquare(ActiveSquare, x => x.Left(1), y => y.Below(2)));
            candidateSquares.Add(GetSquare(ActiveSquare, x => x.Right(1), y => y.Below(2)));

            AddIfCanMove(candidateSquares, result);

            return result;
        }

        private Square GetSquare(Square currentSquare, Func<Square, Square> x, Func<Square, Square> y)
        {
            Square xSquare = x(currentSquare);

            if (xSquare != null)
            {
                return y(xSquare);
            }

            return null;
        }

        private void AddIfCanMove(IEnumerable<Square> squares, List<Square> result)
        {
            foreach (Square square in squares)
            {
                if (square == null)
                {
                    continue;
                }

                if (square.IsOccupied)
                {
                    if (square.ContainsPieceOfColor(EnemyColor))
                    {
                        result.Add(square);
                    }

                    continue;
                }

                result.Add(square);
            }
        }
    }
}
