namespace UniLog
{
	// 整形されたログメッセージを生成するクラス
	public static class LogMessageCreater
	{
		// デバッグ出力用のメッセージを生成する
		public static string CreateDebugOutputMessage(string filteringKeyword, LogLevel level, string message)
		{
			bool filtered = !string.IsNullOrEmpty(filteringKeyword);
			bool tagged = GetLevelTag(level, out string tag);

			if (filtered && tagged) { return filteringKeyword + tag + message; }
			else if (filtered && !tagged) { return filteringKeyword + message; }
			else if (tagged) { return tag + message; }
			else { return message; }
		}

		// ログファイル出力用のメッセージを生成する
		public static string CreateLogFileMessage(LogLevel level, string message)
		{
			if (GetLevelTag(level, out string tag)) { return tag + message; }
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
				default:
					result = null;
					return false;
			}
		}
	}
}
