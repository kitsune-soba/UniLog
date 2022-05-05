using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UniLog
{
	// ログファイルへの書き込みを行うクラス
	public class LogFileWriter : IDisposable
	{
		private StreamWriter stream;
		private readonly Stopwatch stopwatch = new Stopwatch();

		public void Dispose()
		{
			stream?.Close();
			stream = null;
		}

		// コンストラクタ
		// ファイルのオープンに失敗した場合、System.IO.StreamWriter と同等の例外を投げる
		public LogFileWriter(string path, bool append)
		{
			OpenLogFile(path, append);
			WriteHeader(append);
			stopwatch.Start();
		}

		// ログファイル書き込み
		public void WriteLine(string message)
		{
			stream.WriteLine(stopwatch.Elapsed.TotalSeconds.ToString("F3") + "\t| " + message);
		}

		// ログファイルを開く
		private void OpenLogFile(string path, bool append)
		{
			stream = new StreamWriter(path, append)
			{
				AutoFlush = true
			};

#if UNITY_EDITOR
			// Log クラスなど、static な空間にインスタンス化されるとエディタ再生の停止時に開放されないようなので、
			// エディタで再生を停止した時にリソースが解放されるように仕込んでおく
			EditorApplication.playModeStateChanged += (PlayModeStateChange state) =>
			{
				if (state == PlayModeStateChange.EnteredEditMode) { Dispose(); }
			};
#else
			// NOP
			// エディタ再生時以外の終了時は、StreamWriter のファイナライザが解放してくれることをアテにして明示的に開放しない。
			// （いずれかのゲームオブジェクトの OnDestroy() がログを書く可能性があり、かつ
			// 全ての OnDestroy() より後にリソース開放のコードを明示的に呼び出す方法が見つからなかった。）
#endif
		}

		// 書き込み開始時のヘッダを書き込む
		private void WriteHeader(bool append)
		{
			if (append)
			{
				stream.WriteLine(
					'\n' +
					new string('-', 80) + '\n' +
					"Log start at " + DateTime.Now.ToString() + '\n' +
					"Application: " + Application.productName + ' ' + Application.version + '\n' +
					"platform: " + Application.platform);
			}
			else
			{
				stream.WriteLine(
					"Log start at " + DateTime.Now.ToString() + '\n' +
					"Application: " + Application.productName + ' ' + Application.version + '\n' +
					"Platform: " + Application.platform + '\n' +
					new string('-', 80));
			}
		}
	}
}
