using ShogiClock.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Net;
using ShogiClock.Models;

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
            //try
            //{
            // TODO ループ
            for (; ; )
            {
                // UIから URL取得
                // Example: https://golan.sakura.ne.jp/denryusen/dr2_tsec/kifufiles/dr2tsec+buoy_james8nakahi_dr2b3-11-bottom_43_dlshogi_xylty-60-2F+dlshogi+xylty+20210718131042.csa
                (string url, string tournament, int intervalSeconds) = Task.Run(() => this.firstPlayerLabel.Dispatcher.Invoke(() =>
                {
                        // このコードブロックは UIスレッド。UIを更新できます
                        var viewModel = this.DataContext as ClockViewModel;
                    return (viewModel.UrlText, viewModel.Tournament.CurrentItem as string, viewModel.IntervalSeconds);
                })).Result;

                // CSAファイル読取
                var csaFile = CsaFile.Load(tournament, url);

                // すぐ終わる処理
                Task.Run(() => this.firstPlayerLabel.Dispatcher.Invoke(() =>
                {
                        // このコードブロックは UIスレッド。UIを更新できます
                        var viewModel = this.DataContext as ClockViewModel;
                    viewModel.FirstPlayerText = $"{csaFile.RemainingTime[1] / 60}分{csaFile.RemainingTime[1] % 60}秒";
                    viewModel.SecondPlayerText = $"{csaFile.RemainingTime[2] / 60}分{csaFile.RemainingTime[2] % 60}秒";
                }));

                // 更新間隔（秒）
                Thread.Sleep(intervalSeconds * 1000);
            }
            //}
            //catch (TaskCanceledException)
            //{
            //    // 単に終了します
            //}
        }
    }
}
