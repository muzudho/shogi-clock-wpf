using Livet;

namespace ShogiClock.ViewModels
{
    public class ClockViewModel : ViewModel
    {
        private string _firstPlayerText = string.Empty;

        /// <summary>
        /// 先手のテキスト
        /// </summary>
        public string FirstPlayerText
        {
            get
            {
                return this._firstPlayerText;
            }
            set
            {
                if (this._firstPlayerText == value)
                {
                    return;
                }
                this._firstPlayerText = value;
                RaisePropertyChanged("FirstPlayerText");
            }
        }

        private string _secondPlayerText = string.Empty;

        /// <summary>
        /// 後手のテキスト
        /// </summary>
        public string SecondPlayerText
        {
            get
            {
                return this._secondPlayerText;
            }
            set
            {
                if (this._secondPlayerText == value)
                {
                    return;
                }
                this._secondPlayerText = value;
                RaisePropertyChanged("SecondPlayerText");
            }
        }
    }
}
