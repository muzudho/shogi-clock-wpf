using ShogiClock.Models;
using ShogiClock.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ShogiClock.Views
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

        private Task MonitoringTask()
        {
            return Task.Run(() =>
            {
                // このコードブロックは ワーカー スレッド。UIにはアクセスできません
                try
                {
                    for (; ; )
                    {
                        // UIから URL取得
                        // Example: https://golan.sakura.ne.jp/denryusen/dr2_tsec/kifufiles/dr2tsec+buoy_james8nakahi_dr2b3-11-bottom_43_dlshogi_xylty-60-2F+dlshogi+xylty+20210718131042.csa
                        (string url, string tournament, int intervalSeconds) = Task.Run(() => this.firstPlayerLabel.Dispatcher.Invoke(() =>
                        {
                            // このコードブロックは UIスレッド。UIを更新できます
                            var viewModel = this.DataContext as ClockViewModel;
                            var sec = viewModel.IntervalSeconds;
                            if (sec < 1)
                            {
                                // とりあえず、最低でも1秒に強制しとこ
                                sec = 1;
                                viewModel.IntervalSeconds = sec;
                            }
                            return (viewModel.UrlText, viewModel.Tournament.CurrentItem as string, sec);
                        })).Result;

                        // CSAファイル読取
                        CsaFile csaFile;
                        if (CsaFile.Load(tournament, url, out csaFile))
                        {
                            // 読込成功時
                            // すぐ終わる処理
                            bool finished = Task.Run(() => this.firstPlayerLabel.Dispatcher.Invoke(() =>
                            {
                                // このコードブロックは UIスレッド。UIを更新できます
                                var viewModel = this.DataContext as ClockViewModel;
                                viewModel.FirstPlayerText = $"{csaFile.RemainingTime[1] / 60}分{csaFile.RemainingTime[1] % 60}秒";
                                viewModel.SecondPlayerText = $"{csaFile.RemainingTime[2] / 60}分{csaFile.RemainingTime[2] % 60}秒";

                                if (0 < csaFile.EndTime.Ticks)
                                {
                                    viewModel.StatusText = $"{csaFile.EndTime.ToString("yyyy/MM/dd HH:mm:ss")}に対局終了。自動更新おわり  (最終更新 {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")})";
                                    this.tournamentComboBox.IsEnabled = true;
                                    this.monitorButton.IsEnabled = true;
                                    this.intervalSeconds.IsEnabled = true;
                                    return true;
                                }
                                viewModel.StatusText = DateTime.Now.ToString("自動更新中 (最終更新 yyyy/MM/dd HH:mm:ss)");
                                return false;
                            })).Result;
                            if (finished)
                            {
                                break;
                            }
                        }
                        else
                        {
                            // 読込成功時
                            // すぐ終わる処理
                            Task.Run(() => this.firstPlayerLabel.Dispatcher.Invoke(() =>
                            {
                                // このコードブロックは UIスレッド。UIを更新できます
                                var viewModel = this.DataContext as ClockViewModel;
                                viewModel.FirstPlayerText = $"先手----";
                                viewModel.SecondPlayerText = $"後手----";
                                viewModel.StatusText = DateTime.Now.ToString("棋譜を読み込めていません。自動更新おわり (最終更新 yyyy/MM/dd HH:mm:ss)");
                            })).Wait();
                            break;
                        }

                        // 更新間隔（秒）
                        Thread.Sleep(intervalSeconds * 1000);
                    }
                }
                catch (System.AggregateException)
                {
                    // 内部的には TaskCanceledException。
                    // ウィンドウを閉じたときなどに、タスクが中断してここを通ります。
                    // 単に終了します
                }
            });
        }

        /// <summary>
        /// 監視開始ボタン クリック。
        /// async を付けて非同期にすれば　それが別スレッドになっている分けだから、
        /// thread.Start(); しなくてよくなるはず
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MonitorButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // このコードブロックはUIスレッド。UIにアクセスできます
            this.tournamentComboBox.IsEnabled = false;
            this.monitorButton.IsEnabled = false;
            this.intervalSeconds.IsEnabled = false;

            var viewModel = this.DataContext as ClockViewModel;

            // UIから URLを取得
            var url = this.urlTextBox.Text;

            // ビューをクリアー
            viewModel.FirstPlayerText = "先手----";
            viewModel.SecondPlayerText = "後手----";

            await this.MonitoringTask();

            // 監視終了
            this.tournamentComboBox.IsEnabled = true;
            this.monitorButton.IsEnabled = true;
            this.intervalSeconds.IsEnabled = true;
        }
    }
}
