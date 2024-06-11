using System;

namespace UniLog
{
	// 静的メソッドで手軽にログ出力するための、LogOutputのラッパー
	public static class Log
	{
		private static LogOutput output_ = null;
		private static LogOutput output
		{
			get
			{
				if (output_ == null)
				{
					output_ = new LogOutput("UniLogSettings");
				}
				return output_;
			}
		}

		// 情報の取得
		public static LogOutput.Status status { get => output.status; }
		public static Settings settings { get => output.settings; }

		// ログ出力
		public static void Output(string message, LogLevel level, string customHeader = null)
		{
			output.WriteLine(message, level, customHeader == null ? settings.header : customHeader);
		}

		public static void Fatal(string message, string customHeader = null) => Output(message, LogLevel.Fatal, customHeader);
		public static void Error(string message, string customHeader = null) => Output(message, LogLevel.Error, customHeader);
		public static void Warning(string message, string customHeader = null) => Output(message, LogLevel.Warning, customHeader);
		public static void Notice(string message, string customHeader = null) => Output(message, LogLevel.Notice, customHeader);
		public static void Info(string message, string customHeader = null) => Output(message, LogLevel.Information, customHeader);
		public static void Debug(string message, string customHeader = null) => Output(message, LogLevel.Debug, customHeader);

		[Obsolete("Use Log.Fatal, Log.Error, Log.Warning, Log.Notice, Log.Info, or Log.Debug instead of Log.WriteLine.")]
		public static void WriteLine(string message, LogLevel level = LogLevel.Information)
		{
			Output(message, level);
		}
	}
}
