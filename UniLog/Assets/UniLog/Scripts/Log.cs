namespace UniLog
{
	// 静的メソッド的な手軽さでログ出力するための、LogOutputのラッパー
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
		public static void WriteLine(string message, LogLevel level = LogLevel.Information)
		{
			output.WriteLine(message, level);
		}
	}
}
