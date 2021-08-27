# shogi-clock-wpf

![20210828shogi93.png](./ShogiClock/doc/img/20210828shogi93.png)  

floodgate、または 電竜戦 の　残り持ち時間　を覗き見ます。  
リアルタイムでのカウントダウンはしません。  
フィッシャークロック・ルールのみ対応、秒読み無理。  

## Set up

Visual Studio 2019 とか使って ソースをビルドしてください。  

## Run

フォルダー階層は違うかも知れませんが  
`C:\GitHub\shogi-clock-wpf\ShogiClock\ShogiClock\bin\Release\ShogiClock.exe` をダブルクリックしてください。  

電竜戦と floodgate で細かく違うから 両方で動作テストしてください。  

## Stop

対局が終了すると止まります。  
途中で止めるには、ウィンドウを閉じて強制終了してください。  

## Step up

かっこいい時計が欲しいなら、WPFで作ってるので ビューとか一から改造してみてください。  

## Troubleshooting

floodgateの棋譜のURL**ではない**例:  

```plain
http://wdoor.c.u-tokyo.ac.jp/shogi/view/2021/08/10/wdoor+floodgate-300-10F+python-dlshogi2+Krist_483_473stb_1000k+20210810213010.csa
                                   ~~~~
```

👆　これは棋譜ではなく、将棋盤の画面のURLです。 URLに `view` の文字が入っています  

floodgateの棋譜のURLの例:  

```plain
http://wdoor.c.u-tokyo.ac.jp/shogi/LATEST//2021/08/10/wdoor+floodgate-300-10F+Qhapaq_WCSC29_8c+Kristallweizen_R9-3950X+20210810230009.csa
                                   ~~~~~~
```

👆 ok。 URLに `LATEST` の文字が入っています  

電竜戦の棋譜のURLの例:  

```plain
https://golan.sakura.ne.jp/denryusen/dr2_tsec/kifufiles/dr2tsec+buoy_james8nakahi_dr2b3-11-bottom_43_dlshogi_xylty-60-2F+dlshogi+xylty+20210718131042.csa
```
