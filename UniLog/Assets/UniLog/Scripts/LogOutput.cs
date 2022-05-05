using System;
using System.IO;
using UnityEngine;
#if UNITY_STANDALONE_WIN
using System.Runtime.InteropServices;
#endif

namespace UniLog
{
	// ログレベル
	public enum LogLevel
	{
		Fatal = 0,
		Error = 1,
		Warning = 2,
		Notice = 3,
		Information = 4,
		Debug = 5
	}

	// ログ出力
	// メッセージをデバッグ出力やログファイルへ出力する
	public class LogOutput : IDisposable
	{
		[Flags]
		public enum Status
		{
			Fine = 0,
			SettingsLoadError = 1 << 0, // 設定をロードできなかった
			LogFileIOError = 1 << 1 // ログファイルを開けなかった
		}

		// 状態の取得
		public Status status { get; private set; } = Status.Fine;
		public Settings settings { get; private set; } = null;

		private readonly string selfFilteringKeyword = "[UniLog]";
		private LogFileWriter logFile = null;

#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "OutputDebugStringW")]
		private static extern void OutputDebugString(string message);
#endif

		public void Dispose()
		{
			logFile?.Dispose();
			logFile = null;
		}

		// コンストラクタ
		public LogOutput(string settingsPath)
		{
			// 設定をロードする
			settings = Resources.Load<Settings>(settingsPath);
			if (settings == null)
			{
				Debug.LogError(string.Format("{0}[Error]Failed to load \"{1}\" in Resources directory.", selfFilteringKeyword, settingsPath));
				status |= Status.SettingsLoadError;
			}
		}

		// ログ出力
		public void WriteLine(string message, LogLevel level = LogLevel.Information)
		{
			if (status.HasFlag(Status.SettingsLoadError)) { return; } // 設定のロードに失敗した場合は既にコンストラクタでエラーを吐いている筈なので、ここではエラー出力はしない

			// デバッグ出力
			if (settings.GetCurrentDebugOutputCondition().Acceptable(level))
			{
				string formattedMessage = LogMessageCreater.CreateDebugOutputMessage(settings.filteringKeyword, level, message);
				WriteDebugOutputLine(formattedMessage, level);
			}

			// ログファイル出力
			if (settings.GetCurrentLogFileCondition().Acceptable(level))
			{
				string formattedMessage = LogMessageCreater.CreateLogFileMessage(level, message);
				WriteLogFileLine(formattedMessage);
			}
		}

		// デバッグ出力する
		private void WriteDebugOutputLine(string message, LogLevel level)
		{
#if UNITY_EDITOR
			if (level <= LogLevel.Error) { Debug.LogError(message); }
			else if (level == LogLevel.Warning) { Debug.LogWarning(message); }
			else { Debug.Log(message); }
#elif UNITY_STANDALONE_WIN
			OutputDebugString(message);
#else
			// NOP（その他のプラットフォームでどうするか知らないので未実装）
#endif
		}

		// ログファイルに出力する
		private void WriteLogFileLine(string message)
		{
			if (status.HasFlag(Status.LogFileIOError)) { return; } // ファイルのIOエラーがあった場合は既にエラーを吐いた筈なので、ここではエラー出力しない

			if (logFile == null)
			{
				try
				{
					logFile = new LogFileWriter(settings.GetLogFilePath(), settings.logFileAppend);
				}
				catch (Exception exception)
				{
					if ((exception is ArgumentException) ||
						(exception is IOException))
					{
						string errorMessage = string.Format("Failed to open the log file with {0} mode : {1}",
							settings.logFileAppend ? "append" : "write", settings.GetLogFilePath());
						WriteDebugOutputLine(LogMessageCreater.CreateDebugOutputMessage(selfFilteringKeyword, LogLevel.Error, errorMessage), LogLevel.Error);
						status |= Status.LogFileIOError;
						return;
					}
					throw;
				}
			}

			logFile.WriteLine(message);
		}
	}
}
