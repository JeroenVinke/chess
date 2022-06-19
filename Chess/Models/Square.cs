using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chess.Enums;
using Chess.Pieces;

namespace Chess.Models
{
    [DebuggerDisplay("{X}, {Y}, {Color}, {ActivePiece}")]
    public class Square : NotifyPropertyChangedBase
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Board Board { get; set; }


        SquareColor _color;
        public SquareColor Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    NotifyPropertyChanged();
                }
            }
        }

        Piece _activePiece;
        public Piece ActivePiece
        {
            get { return _activePiece; }
            set
            {
                if (_activePiece != value)
                {
                    _activePiece = value;

                    if (value != null)
                    {
                        _activePiece.ActiveSquare = this;
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    foreach (Square square in Board.AllSquares())
                    {
                        if (square != this)
                        {
                            square.IsSelected = false;
                            square.IsHighlighted = false;
                        }
                    }

                    _isSelected = value;

                    if (value)
                    {
                        if (ActivePiece != null)
                        {
                            foreach (Square square in ActivePiece.GetEffectiveReachableSquares())
                            {
                                square.IsHighlighted = true;
                            }
                        }
                    }

                    NotifyPropertyChanged();
                }
            }
        }

        internal bool IsAttackedBy(PieceColor color)
        {
            foreach (Piece enemyPiece in Board.Pieces.Where(x => x.Color == color))
            {
                if (enemyPiece.AttackingSquares().Contains(this))
                {
                    return true;
                }
            }

            return false;
        }

        bool _isHighlighted;
        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                if (_isHighlighted != value)
                {
                    _isHighlighted = value;
                    NotifyPropertyChanged();
                }
            }
        }

        internal bool ContainsPieceOfColor(PieceColor color)
        {
            return IsOccupied && color == ActivePiece.Color;
        }

        public List<Square> Column { get; internal set; }
        public List<Square> Row { get; internal set; }
        public bool IsOccupied => ActivePiece != null;

        public Square(Board board, int i, int y)
        {
            X = i;
            Y = y;

            Board = board;
            Color = (X + Y) % 2 == 0 ? SquareColor.Light : SquareColor.Dark;
        }

        public Square Above(int count = 1) => Column.Count > Y + count ? Column[Y + count] : null;
        public Square Below(int count = 1) => Y >= count ? Column[Y - count] : null;
        public Square Left(int count = 1) => X >= count ? Row[X - count] : null;
        public Square Right(int count = 1) => Row.Count > X + count ? Row[X + count] : null;

        public IEnumerable<Square> AllAbove() => Column.Skip(Y + 1);
        public IEnumerable<Square> AllBelow() => Column.Take(Y).Reverse();
        public IEnumerable<Square> AllLeft() => Row.Take(X).Reverse();
        public IEnumerable<Square> AllRight() => Row.Skip(X + 1);
        public IEnumerable<Square> DiagonalRightUp()
        {
            return GetDiagnoal(x => x.Right(), x => x.Above());
        }
        public IEnumerable<Square> DiagonalRightDown()
        {
            return GetDiagnoal(x => x.Right(), x => x.Below());
        }
        public IEnumerable<Square> DiagonalLeftUp()
        {
            return GetDiagnoal(x => x.Left(), x => x.Above());
        }
        public IEnumerable<Square> DiagonalLeftDown()
        {
            return GetDiagnoal(x => x.Left(), x => x.Below());
        }

        private IEnumerable<Square> GetDiagnoal(Func<Square, Square> x, Func<Square, Square> y)
        {
            List<Square> squares = new List<Square>();

            Square cur = this;

            while (true)
            {
                Square right = x(cur);

                if (right != null)
                {
                    Square up = y(right);

                    if (up != null)
                    {
                        squares.Add(up);
                        cur = up;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return squares;
        }
    }

}
