using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;

namespace ShogiClock.Models
{
    /// <summary>
    /// CSA形式の棋譜ファイル
    /// </summary>
    public class CsaFile
    {
        /// <summary>
        /// CSA形式の棋譜のバージョン
        /// </summary>
        private static readonly Regex _reVersion = new Regex(@"^(V[\d\.]+)$");

        /// <summary>
        /// 持ち時間（秒）
        /// 電竜戦では、$TIME_LIMIT（持ち時間）ではなく、$EVENT（イベント名）の方に持ち時間が書かれている。単位は秒だろうか？
        /// また - _ の取り扱いが不確かなので、うしろからパースすること。
        /// </summary>
        /// <example>$EVENT:dr2tsec+buoy_james8nakahi_dr2b3-11-bottom_43_dlshogi_xylty-60-2F+dlshogi+xylty+20210718131042</example>
        private static readonly Regex _reDenryuSenTimeLimit = new Regex(@"^\$EVENT:.+-(\d+)-\d+F\+[0-9A-Za-z_-]+\+[0-9A-Za-z_-]+\+\d{14}$");

        /// <summary>
        /// floodgate
        /// </summary>
        private static readonly Regex _reFloodgateTimeLimit = new Regex(@"^\$EVENT:wdoor\+floodgate-(\d+)-\d+F\+.+$");

        /// <summary>
        /// 手番。1が先手、2が後手。配列の添え字に使う
        /// </summary>
        /// <example>+2726FU</example>
        private static readonly Regex _rePhase = new Regex(@"^([+-])\d{4}\w{2}$");

        /// <summary>
        /// 開始時間
        /// </summary>
        /// <example>$START_TIME:2021/07/18 13:10:42</example>
        private static readonly Regex _reStartTime = new Regex(@"^\$START_TIME:(\d{4})/(\d{2})/(\d{2}) (\d{2}):(\d{2}):(\d{2})$");

        /// <summary>
        /// 加算時間
        /// </summary>
        /// <example>'Increment:2</example>
        private static readonly Regex _reIncrement = new Regex(@"^'Increment:(\d+)$");

        /// <summary>
        /// 消費時間
        /// </summary>
        /// <example>T2</example>
        private static readonly Regex _reErapsed = new Regex(@"^T(\d+)$");

        /// <summary>
        /// 終了時間
        /// </summary>
        /// <example>'$END_TIME:2021/08/10 21:14:45</example>
        private static readonly Regex _reEndTime = new Regex(@"^'\$END_TIME:(\d{4})/(\d{2})/(\d{2}) (\d{2}):(\d{2}):(\d{2})$");

        /// <summary>
        /// CSA形式の棋譜を指すURL
        /// </summary>
        public string Url { get; set; } = "";

        /// <summary>
        /// 大会。違いを吸収するのに使います。
        /// 電竜戦は "denryu-sen",
        /// floodgateは "floodgate"
        /// </summary>
        public string Tournament { get; set; } = "";

        /// <summary>
        /// CSA形式の棋譜のバージョン
        /// </summary>
        public string Version { get; set; } = "";

        /// <summary>
        /// 持ち時間（秒）
        /// [未使用,先手,後手]
        /// </summary>
        public int[] TimeLimit { get; set; } = new int[3] { 0, 0, 0 };

        /// <summary>
        /// 加算した時間（秒）
        /// [未使用,先手,後手]
        /// </summary>
        public int[] IncrementalTime { get; set; } = new int[3] { 0, 0, 0 };

        /// <summary>
        /// 秒読み（秒）
        /// [未使用,先手,後手]
        /// </summary>
        public int[] ByoyomiTime { get; set; } = new int[3] { 0, 0, 0 };

        /// <summary>
        /// 手番。1が先手、2が後手。配列の添え字に使います
        /// </summary>
        public int Phase { get; set; } = 0;

        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 加算時間
        /// </summary>
        public int Increment { get; set; } = 0;

        /// <summary>
        /// 消費時間
        /// [未使用,先手,後手]
        /// </summary>
        public int[] Erapsed { get; set; } = { 0, 0, 0 };

        /// <summary>
        /// 終了時間
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 残り時間。
        /// フィッシャークロック ルール
        /// 残り時間（秒） ＝ 持ち時間（秒） ＋　加算時間（秒）　- 消費時間（秒）
        /// 秒読みルールは分からん
        /// </summary>
        public int[] RemainingTime
        {
            get
            {
                return new int[3]{ 0,
                        this.TimeLimit[1] + this.IncrementalTime[1] - this.Erapsed[1],
                        this.TimeLimit[2] + this.IncrementalTime[2] - this.Erapsed[2]};
            }
        }

        /// <summary>
        /// 棋譜ファイルの読取。
        /// CSA形式でないファイルを読み込んだ場合、エラーを返します
        /// </summary>
        /// <param name="tournament">大会。電竜戦は "denryu-sen", floodgateは "floodgate"</param>
        /// <param name="csaUrl">CSA形式の棋譜ファイルを指すURL</param>
        /// <returns></returns>
        public static CsaFile Load(string tournament, string csaUrl)
        {
            var csaFile = new CsaFile();
            csaFile.Tournament = tournament;
            csaFile.Url = csaUrl;

            // 棋譜ファイル（CSA形式）を読む
            // CSAファイル読取
            WebClient webClient = new WebClient();
            var newLine = "\r\n";
            // floodgateは EUC-JP
            if (tournament == "floodgate")
            {
                webClient.Encoding = Encoding.GetEncoding("EUC-JP");
                newLine = "\n";
            }
            var csaText = webClient.DownloadString(csaFile.Url);

            int i = 0;
            foreach (var line in csaText.Split(new string[] { newLine }, StringSplitOptions.None))
            {
                if (i == 0)
                {
                    var result = CsaFile._reVersion.Match(line);
                    if (result.Success)
                    {
                        // OK, pass
                    }
                    else
                    {
                        // Error
                        throw new Exception($"It\'s not a CSA file. Expected: \"V2\", etc. Found: \"{line}\"");
                    }
                }

                var groups = CsaFile._rePhase.Matches(line);
                if (1 < groups.Count)
                {
                    // print(f"Phase {result.group(1)}")
                    string sign = groups[1].Value;
                    if (sign == "+")
                    {
                        csaFile.Phase = 1;
                        csaFile.IncrementalTime[1] += csaFile.Increment;
                    }
                    else if (sign == "-")
                    {
                        csaFile.Phase = 2;
                        csaFile.IncrementalTime[2] += csaFile.Increment;
                    }
                    else
                    {
                        csaFile.Phase = 0; // Error
                    }

                    continue;
                }

                groups = CsaFile._reErapsed.Matches(line);
                if (1 < groups.Count)
                {
                    // print(f"Erapsed {result.group(1)}")
                    csaFile.Erapsed[csaFile.Phase] += int.Parse(groups[1].Value);
                    continue;
                }

                if (tournament == "floodgate")
                {
                    // floodgate用
                    groups = CsaFile._reFloodgateTimeLimit.Matches(line);
                    if (1 < groups.Count)
                    {
                        // print(f"TimeLimit Sec={result.group(1)}")
                        // 先手と後手の持ち時間は同じ
                        csaFile.TimeLimit = new int[3] { 0, int.Parse(groups[1].Value), int.Parse(groups[1].Value) };
                        continue;
                    }
                }
                else
                {
                    // 電竜戦、その他用
                    groups = CsaFile._reDenryuSenTimeLimit.Matches(line);
                    if (1 < groups.Count)
                    {
                        // print(f"TimeLimit Sec={result.group(1)}")
                        // 先手と後手の持ち時間は同じ
                        csaFile.TimeLimit = new int[3] { 0, int.Parse(groups[1].Value), int.Parse(groups[1].Value) };
                        continue;
                    }
                }

                groups = CsaFile._reStartTime.Matches(line);
                if (1 < groups.Count)
                {
                    // print(f"StartTime [1]={result.group(1)} [2]={result.group(2)} [3]={result.group(3)} [4]={result.group(4)} [5]={result.group(5)} [6]={result.group(6)}")
                    csaFile.StartTime = new DateTime(
                        int.Parse(groups[1].Value),
                        int.Parse(groups[2].Value),
                        int.Parse(groups[3].Value),
                        int.Parse(groups[4].Value),
                        int.Parse(groups[5].Value),
                        int.Parse(groups[6].Value));
                    continue;
                }

                groups = CsaFile._reEndTime.Matches(line);
                if (1 < groups.Count)
                {
                    // print(f"EndTime [1]={result.group(1)} [2]={result.group(2)} [3]={result.group(3)} [4]={result.group(4)} [5]={result.group(5)} [6]={result.group(6)}")
                    csaFile.EndTime = new DateTime(
                        int.Parse(groups[1].Value),
                        int.Parse(groups[2].Value),
                        int.Parse(groups[3].Value),
                        int.Parse(groups[4].Value),
                        int.Parse(groups[5].Value),
                        int.Parse(groups[6].Value));
                    continue;
                }

                groups = CsaFile._reIncrement.Matches(line);
                if (1 < groups.Count)
                {
                    // print(f"Increment {result.group(1)}")
                    csaFile.Increment = int.Parse(groups[1].Value);
                    continue;
                }

                // print(f"> {line}")

                i++;
            }

            return csaFile;
        }
    }
}
