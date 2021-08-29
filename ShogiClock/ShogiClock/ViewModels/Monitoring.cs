using System;
using System.Windows.Input;

namespace ShogiClock.ViewModels
{
    /// <summary>
    /// 棋譜を監視するコマンドです
    /// </summary>
    public class Monitoring : ICommand
    {
        private bool _isBusy = false;

        /// <summary>
        /// 忙しいか？ 忙しいと実行できません
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this._isBusy;
            }
            set
            {
                if (this._isBusy == value)
                {
                    return;
                }
                this._isBusy = value;
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
            return !this.IsBusy;
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
            if (this.IsBusy)
            {
                return;
            }

            // TODO ここに処理を書く
            System.Windows.MessageBox.Show("hoge");
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
