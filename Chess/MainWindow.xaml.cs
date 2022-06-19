using Chess.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Chess
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        Board _board;

        public event PropertyChangedEventHandler PropertyChanged;

        public Board Board
        {
            get { return _board; }
            set
            {
                if (_board != value)
                {
                    _board = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            Background = Brushes.LightGray;

            Board = new Board();
            Board.CreatePieces();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {

            if (sizeInfo.WidthChanged) this.Width = sizeInfo.NewSize.Height * 1;
            else this.Height = sizeInfo.NewSize.Width / 1;
        }
    }
}
