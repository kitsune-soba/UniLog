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
			stream.WriteLine(new string('-', 80));
			stream.WriteLine($"Log finish: {DateTime.Now.ToString()}");

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
			double rawSecondsDouble = stopwatch.Elapsed.TotalSeconds;
			uint rawSecondsUint = (uint)rawSecondsDouble;
			uint hours = rawSecondsUint / 3600;
			uint minutes = (rawSecondsUint % 3600) / 60;
			uint seconds = rawSecondsUint % 60;
			double decimalSeconds = (rawSecondsDouble - rawSecondsUint);
			stream.WriteLine($"{hours.ToString("D2")}:{minutes.ToString("D2")}:{seconds.ToString("D2")}{decimalSeconds.ToString("F3").TrimStart('0')} | {message}");
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
				stream.WriteLine(""); // 改行
			}
			stream.WriteLine($"Log start: {DateTime.Now.ToString()}");
			stream.WriteLine($"Application: {Application.productName} {Application.version}");
			stream.WriteLine($"Platform: {Application.platform}");
			stream.WriteLine($"{ new string('-', 80)}");
		}
	}
}
