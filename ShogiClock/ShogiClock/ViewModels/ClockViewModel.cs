using Livet;
using System.Windows.Data;
using System.Windows.Input;

namespace ShogiClock.ViewModels
{
    public class ClockViewModel : ViewModel
    {
        private string _urlText = string.Empty;

        /// <summary>
        /// URLのテキスト
        /// </summary>
        public string UrlText
        {
            get
            {
                return this._urlText;
            }
            set
            {
                if (this._urlText == value)
                {
                    return;
                }
                this._urlText = value;
                RaisePropertyChanged("UrlText");
            }
        }

        private CollectionView _tournament = new CollectionView(new string[] { "floodgate", "denryu-sen" });

        /// <summary>
        /// 棋譜を読みに行く間隔（秒）
        /// </summary>
        public CollectionView Tournament
        {
            get
            {
                return this._tournament;
            }
            set
            {
                if (this._tournament == value)
                {
                    return;
                }
                this._tournament = value;
                RaisePropertyChanged("Tournament");
            }
        }

        private int _intervalSeconds = 15;

        /// <summary>
        /// 棋譜を読みに行く間隔（秒）
        /// </summary>
        public int IntervalSeconds
        {
            get
            {
                return this._intervalSeconds;
            }
            set
            {
                if (this._intervalSeconds == value)
                {
                    return;
                }
                this._intervalSeconds = value;
                RaisePropertyChanged("IntervalSeconds");
            }
        }

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

        private string _statusText = string.Empty;

        /// <summary>
        /// 状況のテキスト
        /// </summary>
        public string StatusText
        {
            get
            {
                return this._statusText;
            }
            set
            {
                if (this._statusText == value)
                {
                    return;
                }
                this._statusText = value;
                RaisePropertyChanged("StatusText");
            }
        }

        private ICommand _monitoring;

        /// <summary>
        /// 監視をするコマンド
        /// </summary>
        public ICommand Monitoring
        {
            get
            {
                return _monitoring ?? (_monitoring = new Monitoring());
            }
        }
    }
}
