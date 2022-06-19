using Chess.Enums;
using Chess.Models;
using Chess.Pieces;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chess.Controls
{
    [DebuggerDisplay("{Square}")]
    public class SquareControl : UserControl, INotifyPropertyChanged
    {
        public Square Square { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Piece ActivePiece
        {
            get { return (Piece)GetValue(ActivePieceProperty); }
            set { SetValue(ActivePieceProperty, value); }
        }

        private Grid Grid { get; set; }

        public static readonly DependencyProperty ActivePieceProperty =
            DependencyProperty.Register("ActivePiece", typeof(Piece), typeof(SquareControl), new PropertyMetadata(null, ActivePieceChanged));



        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SquareControl), new PropertyMetadata(false, IsSelectedChanged));



        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(SquareControl), new PropertyMetadata(false, IsHighlightedChanged));


        public SquareControl(Square square)
        {
            Grid = new Grid();

            Square = square;
            AllowDrop = true;

            MouseDown += SquareControl_MouseDown;

            AddChild(Grid);

            Drop += SquareControl_Drop;

            Render();
        }

        private void SquareControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SquareControl ctrl = (SquareControl)sender;

            ctrl.Square.IsSelected = true;
        }

        private static void IsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SquareControl ctrl = (SquareControl)d;

            ctrl.Render();
        }

        private static void IsHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SquareControl ctrl = (SquareControl)d;

            ctrl.Render();
        }

        private void SquareControl_Drop(object sender, DragEventArgs e)
        {
            SquareControl dropSource = sender as SquareControl;
            SquareControl dragSource = (SquareControl)e.Data.GetData(typeof(SquareControl));

            if (dropSource != dragSource)
            {
                if (dragSource.ActivePiece.CanMoveTo(dropSource.Square))
                {
                    dragSource.ActivePiece.MoveTo(dropSource.Square);
                }
            }
        }

        private static void ActivePieceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SquareControl ctrl = (SquareControl)d;

            ctrl.Render();
        }

        private void Render()
        {
            if (IsSelected)
            {
                Background = Brushes.CadetBlue;
            }
            else if (IsHighlighted)
            {
                Background = Brushes.LightGoldenrodYellow;
            }
            else
            {
                Background = Square.Color == SquareColor.Light ? Brushes.White : Brushes.LightBlue;
            }

            if (Grid != null)
            {
                Grid.Children.Clear();
            }

            if (ActivePiece == null)
            {
                return;
            }

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(ActivePiece.Image);
            bitmapImage.EndInit();

            Image image = new Image();
            image.Source = bitmapImage;
            image.Margin = new Thickness(5);

            image.MouseMove += Image_MouseMoved;

            Grid.Children.Add(image);
        }

        private void Image_MouseMoved(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            if (image != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(image,
                                        this,
                                        DragDropEffects.Move);
            }
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
