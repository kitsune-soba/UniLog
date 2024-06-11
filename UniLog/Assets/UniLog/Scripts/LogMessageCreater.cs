namespace UniLog
{
	// 整形されたログメッセージを生成するクラス
	public static class LogMessageCreater
	{
		// 出力用のメッセージを生成する
		public static string CreateMessage(string header, LogLevel level, string message)
		{
			bool hasHeader = header != null;
			bool tagged = GetLevelTag(level, out string tag);

			if (hasHeader && tagged) { return header + tag + message; }
			else if (hasHeader && !tagged) { return header + message; }
			else if (tagged) { return tag + message; }
			else { return message; }
		}

		// メッセージに付加するログレベル識別用のタグを返す
		private static bool GetLevelTag(LogLevel level, out string result)
		{
			switch (level)
			{
				case LogLevel.Fatal:
					result = "[Error][Fatal]";
					return true;
				case LogLevel.Error:
					result = "[Error]";
					return true;
				case LogLevel.Warning:
					result = "[Warning]";
					return true;
				case LogLevel.Debug:
					result = "[Debug]";
					return true;
				default:
					result = null;
					return false;
			}
		}
	}
}
