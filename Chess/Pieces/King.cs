using Chess.Enums;
using Chess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Pieces
{
    public class King : Piece
    {
        public override int Value => 0;


        public King(Square square, PieceColor color) : base(square, color, color == PieceColor.White ? "2.png" : "3.png")
        {
        }

        public override Piece Clone(Square square, PieceColor color)
        {
            return new King(square, color);
        }

        public override List<Square> AttackingSquares()
        {
            List<Square> result = new List<Square>();

            AddIfCanMove(result, ActiveSquare.Left());
            AddIfCanMove(result, ActiveSquare.Right());
            AddIfCanMove(result, ActiveSquare.Above());
            AddIfCanMove(result, ActiveSquare.Below());
            AddIfCanMove(result, ActiveSquare.Above());
            AddIfCanMove(result, ActiveSquare.Below());
            AddIfCanMove(result, ActiveSquare.Left()?.Above());
            AddIfCanMove(result, ActiveSquare.Right()?.Above());
            AddIfCanMove(result, ActiveSquare.Left()?.Below());
            AddIfCanMove(result, ActiveSquare.Right()?.Below());

            return result;
        }

        public override List<Square> LegalMoves()
        {
            List<Square> result = AttackingSquares();

            if (!HasMoved)
            {
                AddCastleSquares(ActiveSquare.AllLeft(), result);
                AddCastleSquares(ActiveSquare.AllRight(), result);
            }

            return result;
        }

        private void AddCastleSquares(IEnumerable<Square> squares, List<Square> result)
        {
            bool canCastle = true;
            bool foundRook = true;

            foreach (var square in squares)
            {
                if (square.IsOccupied)
                {
                    if (square.ActivePiece is Rook rook && rook.Color == Color && !rook.HasMoved)
                    {
                        foundRook = true;
                        break;
                    }
                    canCastle = false;
                    break;
                }
            }

            if (!foundRook)
            {
                canCastle = false;
            }

            if (canCastle)
            {
                foreach (var square in squares.Take(2))
                {
                    if (square.IsAttackedBy(EnemyColor))
                    {
                        canCastle = false;
                        break;
                    }
                }

                if (canCastle)
                {
                    result.Add(squares.Skip(1).First());
                }
            }
        }

        protected override void OnAfterMove(Square from, Square to)
        {
            Square kingSquare = to;

            if (Math.Abs(from.X - kingSquare.X) == 2)
            {
                Func<Square, Square> x;
                Func<Square, Square> xInverse;

                if (from.X - kingSquare.X < 0)
                {
                    x = s => s.Right();
                    xInverse = s => s.Left();
                }
                else
                {
                    x = s => s.Left();
                    xInverse = s => s.Right();
                }


                while (x(to) != null)
                {
                    if (x(to).ActivePiece is Rook rook)
                    {
                        rook.ActiveSquare.ActivePiece = null;
                        xInverse(kingSquare).ActivePiece = rook;
                        break;
                    }

                    to = x(to);
                }
            }

            base.OnAfterMove(from, kingSquare);
        }

        protected override bool IsPinned()
        {
            return false;
        }

        public bool IsInCheck()
        {
            foreach (Piece enemyPiece in Board.Pieces.Where(x => x.Color == EnemyColor && !(x is King)))
            {
                if (enemyPiece.AttackingSquares().Contains(ActiveSquare))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsInCheckMate()
        {
            if (Board.ColorToMove == Color && IsInCheck() && LegalMoves().Count == 0)
            {
                return true;
            }

            return false;
        }

        public bool IsInStaleMate()
        {
            foreach (Piece piece in Board.Pieces.Where(x => x.Color == Color))
            {
                if (piece.GetEffectiveReachableSquares().Count > 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void AddIfCanMove(List<Square> result, Square square)
        {
            if (square != null)
            {
                if (!square.IsOccupied || square.ContainsPieceOfColor(EnemyColor))
                {
                    Board cloned = Board.Clone();

                    cloned.Rows[Y][X].ActivePiece.MoveTo(cloned.Rows[square.Y][square.X]);

                    King king = cloned.Pieces.Where(x => x.Color == Color && x is King).Cast<King>().First();

                    if (!king.IsInCheck())
                    {
                        result.Add(square);
                    }
                }
            }
        }
    }
}
