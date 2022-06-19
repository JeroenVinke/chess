using Chess.Enums;
using Chess.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Chess.Controls
{
    public class CapturedPiecesControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Board Board
        {
            get { return (Board)GetValue(BoardProperty); }
            set { SetValue(BoardProperty, value); }
        }

        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register("Board", typeof(Board), typeof(CapturedPiecesControl), new PropertyMetadata(null, BoardChanged));

        public StackPanel StackPanel { get; set; }

        public CapturedPiecesControl()
        {
            StackPanel = new StackPanel();
            StackPanel.Orientation = Orientation.Horizontal;

            AddChild(StackPanel);
        }

        private void OnPiecesChanged(object sender, EventArgs e)
        {
            Render();
        }

        private static void BoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CapturedPiecesControl control = (CapturedPiecesControl)d;

            if (control.Board != null)
            {
                control.Board.OnCapture -= control.OnPiecesChanged;
                control.Board.OnPieceAdd -= control.OnPiecesChanged;
                control.Board.OnPieceRemove -= control.OnPiecesChanged;
            }

            control.Board.OnCapture += control.OnPiecesChanged;
            control.Board.OnPieceAdd += control.OnPiecesChanged;
            control.Board.OnPieceRemove += control.OnPiecesChanged;

            control.Render();
        }

        private void Render()
        {
            if (StackPanel != null)
            {
                StackPanel.Children.Clear();
            }

            if (Board == null)
            {
                return;
            }

            AddCapturedPiecesFor(PieceColor.White);
            AddCapturedPiecesFor(PieceColor.Black);

            AddScore();
        }

        private void AddScore()
        {
            int advantageWhite = Board.Pieces.Where(x => x.Color == PieceColor.White).Sum(x => x.Value) - Board.Pieces.Where(x => x.Color == PieceColor.Black).Sum(x => x.Value);

            if (advantageWhite > 0)
            {
                StackPanel.Children.Add(new Label() { Content = "White: + " + advantageWhite });
            }
            else if (advantageWhite < 0)
            {
                StackPanel.Children.Add(new Label() { Content = "Black: + " + advantageWhite });
            }
        }

        private void AddCapturedPiecesFor(PieceColor color)
        {
            foreach (var piece in Board.CapturedPieces[color])
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(piece.Image);
                bitmapImage.EndInit();

                Image image = new Image();
                image.Source = bitmapImage;
                image.Width = 48;
                image.Height = 48;

                StackPanel.Children.Add(image);
            }
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
