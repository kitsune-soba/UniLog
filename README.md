# UniLog

## 概要

- エディタ上の再生でもスタンドアロン（ビルドされたアプリ）でも有効なログ出力のための Unity パッケージ
- 出力ごとにログレベル（Fatal, Error, Warning, Notice, Infomation, Debug）を指定できる
- ログの出力先は、Unity エディタのコンソール・ログファイル・デバッグ出力<sup>[1](#note_DebugOutput)</sup> から好きに選べる（複数可）
- 実行時の状況（エディタで再生、スタンドアロン、など）とログレベルに応じたログ出力先のルールを設定できる
- デフォルトの設定で出力するだけなら `Debug.Log` 並みに簡単に使える（下記の 「使い方（簡易）」 を参照）

## 使い方（簡易）

### パッケージの導入

1. [UniLog の unitypackage をダウンロード](https://github.com/kitsune-soba/UniLog/releases)してプロジェクトにインポートする

2. プロジェクトのアセンブリ定義から Assets/UniLog/UniLog.asmdef を参照する

### ログ出力のコードを書く

以下のようにログ出力のコードを書く。6種類のログレベルを選べる。

```
using UniLog;

...

Log.Fatal("Unable to recover...");
Log.Error("An error has occurred!!");
Log.Warning("Warning!");
Log.Notice("Important notice.");
Log.Info("Just for information.");
Log.Debug("Debug information for developer.");
```

## 使い方（詳細）

### 設定

Assets/UniLog/Resources/UniLogSettings を選択すると、インスペクタに以下のような設定が表示されるので、適宜設定する。

![Image/unilog_settings.png](Image/unilog_settings.png "Assets/UniLog/Resources/UniLogSettings のインスペクタ")

- Debug output<sup>[1](#note_DebugOutput) </sup>
	- Header: ログメッセージごとの先頭に自動的に挿入される文字列。[DebugView](https://docs.microsoft.com/en-us/sysinternals/downloads/debugview) などでデバッグ出力を見る際のフィルタリングに用いる。未入力の場合は、Project Settings > Player > Product Name が用いられる。
	- Log Level: 動作モード（後述）ごとの出力の閾値。チェックマークを外すと、対応するモードではデバッグ出力しない。
- Log file
	- Destination: 出力するログファイルの相対パス。未入力の場合は、Project Settings > Player > Product Name + .log が用いられる。
	- Append: ログファイルを追記モードで開く。（OFF の場合は上書きモードで開く。）
	- Log Level: 動作モード（後述）ごとの出力の閾値。チェックマークを外すと、対応するモードではログファイルに出力しない。

また、UniLog の動作モード（後述）を Detail Mode にする場合は、全てのログ出力に先立って以下のように `detailMode` フラグを立てる。

```
Log.settings.detailMode = true;
```

### 動作モード

UniLog には3つの動作モードがある。

- **Default Mode** : スタンドアロンアプリ（ビルドされたアプリ）として実行中であればこのモードで動作する。
- **Detail Mode** : スタンドアロンアプリとして実行中に `detailMode` フラグに true を代入するとこのモードになる。予め上述の UniLogSettings で DetailMode のログの閾値を DefaultMode よりも緩くしておき、不具合対応の時などに詳細なログを有効化したい場合に使用する。例えば、特定のコマンドライン引数や設定ファイルのオプションで `detailMode` フラグを立てるコードを書いておけば、本番環境などにおいてもその場で詳細なログを有効化できる。ログ出力はそれなりの処理コストが掛かるため、普段は Default Mode にて重要なログのみを出力し、詳細な解析が必要な時だけ Detail Mode に切り替える使い方を推奨する。
- **Unity Editor** : エディタでの再生中であればこのモードで動作する。`detailMode` フラグは無視される。

### カスタムヘッダ

ログ出力の先頭には上述の UniLogSettings で指定したヘッダ（未指定であればプロダクト名）が付加されるが、ログ出力ごとにカスタムヘッダを指定することもできる。

```
// 例えば UniLogSettings で Header に Sample と入力していた場合...
Log.Info("Just for information."); // 出力：[Sample]Just for information.
Log.Info("Just for information.", "[Custom]"); // 出力：[Custom]Just for information. ← これがカスタムヘッダ
```

## 開発環境

- Unity 2022.3.32f1
- Visual Studio 2022

## UniLog のパッケージの作り方

Assets/UniLog 以下のアセットをエクスポートする。（但し Assets/UniLog/Samples ディレクトリ以下は除外。）

## 既知の問題 / 制限

- 現状、デバッグ出力<sup>[1](#note_DebugOutput)</sup> は Windows のみで動作する。他の環境でのデバッグ出力に対応したければ、UniLog/Assets/UniLog/Scripts/LogOutput.cs の `WriteDebugOutputLine` メソッドに他の環境向けのコードを追加する必要がある。

---
<sup name="note_DebugOutput">1</sup> Windows であれば DBWIN_BUFFER に出力される。[DebugView](https://docs.microsoft.com/en-us/sysinternals/downloads/debugview) や [ProcessLogger](https://github.com/kitsune-soba/ProcessLogger) などで出力を見ることができる。
