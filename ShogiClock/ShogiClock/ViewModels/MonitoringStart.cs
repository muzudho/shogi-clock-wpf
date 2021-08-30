using System;
using System.Windows.Input;

namespace ShogiClock.ViewModels
{
    /// <summary>
    /// 棋譜を監視するコマンドです
    /// </summary>
    public class MonitoringStart : ICommand
    {
        public MonitoringStart()
        {

        }

        private bool _isEnabled = true;

        /// <summary>
        /// 忙しいか？ 忙しいと実行できません
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return this._isEnabled;
            }
            set
            {
                if (this._isEnabled == value)
                {
                    return;
                }
                this._isEnabled = value;
                this.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 実行できますか？
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return this.IsEnabled;
        }

        /// <summary>
        /// 実行可能かどうかの状況が変わった時に実行される関数を登録できます
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 実行します
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            // 完璧な対策ではありませんが、実行できなくします。
            this.IsEnabled = false;

            // TODO ここに処理を書く
            System.Windows.MessageBox.Show("hoge");

            // View をどうやって取る？
            /*
            this.tournamentComboBox.IsEnabled = false;
            this.monitorButton.IsEnabled = false;
            this.intervalSeconds.IsEnabled = false;

            // このコードブロックはUIスレッド。UIにアクセスできますが、処理は早く終わらなければいけません
            var viewModel = this.DataContext as ClockViewModel;

            // URLを取得
            var url = this.urlTextBox.Text;

            // ビューをクリアー
            viewModel.FirstPlayerText = "先手----";
            viewModel.SecondPlayerText = "後手----";

            // ワーカースレッドを起動します
            var start = new ThreadStart(MonitoringThread);
            var thread = new Thread(start);
            thread.Start();
            */
        }

        /// <summary>
        /// 実行可能かどうかの状況が変わった時に呼び出してください
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
