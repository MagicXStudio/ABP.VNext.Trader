using System.Windows.Controls;

namespace Trader.Client.CoreUI.ChineseChess
{
    /// <summary>
    /// ChessPiece.xaml 的交互逻辑
    /// </summary>
    public partial class ChessPiece : UserControl
    {
        public ChessPiece(string text, int id)
        {
            InitializeComponent();
            Text = text;
            Id = id;
            this.Title.Text = text;
        }

        public string Text { get; set; }

        public int Id { get; set; }
    }
}
