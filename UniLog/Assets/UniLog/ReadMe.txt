UniLog 1.0.0

詳しい説明や最新版はこちら：https://github.com/kitsune-soba/UniLog

■ クイックスタート

１．インストール

UniLog_*.unitypackage をインポート

２．スクリプトを書く

このようにしてログ出力を行う↓
第一引数はログメッセージ。第二引数はログレベル。

// using UniLog してあるという前提
Log.WriteLine("This is an error message.", LogLevel.Error);

また、このようにして動作モードを Detail Mode に切り替えることができる↓

// using UniLog してあるという前提
Log.settings.detailMode = true;

３．ログ出力の設定

Assets/UniLog/Resources/UniLogSettings を選択して、インスペクタで適宜設定を行う。
動作モードごとに出力の閾値となるログレベルを指定できる。
