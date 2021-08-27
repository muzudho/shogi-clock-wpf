using ShogiClock.ViewModels;
using System.Threading;
using System.Threading.Tasks;
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

        /// <summary>
        /// 監視ボタン クリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // このコードブロックはUIスレッド。UIにアクセスできますが、処理は早く終わらなければいけません
            var viewModel = this.DataContext as ClockViewModel;

            // URLを取得
            var url = this.urlTextBox.Text;

            // ビューに表示
            viewModel.FirstPlayerText = "先手WIP";
            viewModel.SecondPlayerText = "後手WIP";

            // ワーカースレッドを起動します
            var start = new ThreadStart(MonitoringThread);
            var thread = new Thread(start);
            thread.Start();
        }

        /// <summary>
        /// いわゆるワーカースレッド。
        /// URLを監視するループです。
        /// </summary>
        private void MonitoringThread()
        {
            // このコードブロックは ワーカー スレッド。UIにはアクセスできません

            // TODO ループ
            // 時間のかかる処理
            for (; ; )
            {
                // UIから URL取得
                string url = Task.Run(() => this.firstPlayerLabel.Dispatcher.Invoke(() =>
                {
                    // このコードブロックは UIスレッド。UIを更新できます
                    var viewModel = this.DataContext as ClockViewModel;
                    return viewModel.UrlText;
                })).Result;

                // TODO CSAファイル読取

                // 残り時間取得

                // 時間のかかる処理
                Thread.Sleep(3 * 1000);

                // すぐ終わる処理
                Task.Run(() => this.firstPlayerLabel.Dispatcher.Invoke(() =>
                {
                    // このコードブロックは UIスレッド。UIを更新できます
                    var viewModel = this.DataContext as ClockViewModel;
                    viewModel.FirstPlayerText = url;// "a";
                    viewModel.SecondPlayerText += "b";
                }));
            }

            //this.Dispatcher.Invoke((Action)(() =>
            //{
            //    this.FirstPlayerText = "Good bye!";
            //}));
        }

    }
}
