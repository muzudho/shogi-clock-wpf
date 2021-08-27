using ShogiClock.ViewModels;
using System.Windows.Controls;

namespace ShogiClock.UserControls
{
    /// <summary>
    /// ClockUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ClockUserControl : UserControl
    {
        public ClockUserControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as ClockViewModel;
            viewModel.FirstPlayerText = "先手WIP";
            viewModel.SecondPlayerText = "後手WIP";
        }
    }
}
