using Chess.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Chess.Controls
{
    public class ChessBoardControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Board Board
        {
            get { return (Board)GetValue(BoardProperty); }
            set { SetValue(BoardProperty, value); }
        }

        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register("Board", typeof(Board), typeof(ChessBoardControl), new PropertyMetadata(null, BoardChanged));

        public Grid Grid { get; set; }

        public ChessBoardControl()
        {
        }

        private static void BoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChessBoardControl control = (ChessBoardControl)d;

            control.Render();
        }

        private void Render()
        {
            if (Grid != null)
            {
                RemoveVisualChild(Grid);
                Grid = null;
            }

            if (Board == null)
            {
                return;
            }

            Grid = new Grid();

            for (int i = 0; i < Board.Rows.Count; i++)
            {
                Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < Board.Columns.Count; i++)
            {
                Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }


            for (int i = 0; i < Board.Rows.Count; i++)
            {

                for (int y = 0; y < Board.Columns.Count; y++)
                {
                    SquareControl squareControl = new SquareControl(Board.Rows[i][y]);
                    squareControl.SetBinding(SquareControl.ActivePieceProperty, new Binding(nameof(Square.ActivePiece)) { Source = Board.Rows[i][y] });
                    squareControl.SetBinding(SquareControl.IsSelectedProperty, new Binding(nameof(Square.IsSelected)) { Source = Board.Rows[i][y] });
                    squareControl.SetBinding(SquareControl.IsHighlightedProperty, new Binding(nameof(Square.IsHighlighted)) { Source = Board.Rows[i][y] });
                    squareControl.SetValue(Grid.RowProperty, Board.Rows.Count - i - 1);
                    squareControl.SetValue(Grid.ColumnProperty, y);
                    Grid.Children.Add(squareControl);
                }
            }

            AddChild(Grid);
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
