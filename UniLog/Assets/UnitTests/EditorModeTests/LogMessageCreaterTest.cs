using NUnit.Framework;
using UniLog;

namespace Tests
{
	// LogMessageCreater のテスト
	public class LogMessageCreaterTest
	{
		private readonly string header = "[UniLogTest]";
		private readonly string message = "Test message.";

		// CreateMessage のテスト
		// 要件１：引数で指定されたメッセージに、ヘッダと、必要に応じてログレベルのタグを付加する
		// 要件２：引数で指定されたヘッダとして null 又は空文字が渡された場合、単にキーワードの付加が省略される
		[Test] // 要件１
		public void CreateMessage_AddKeywordAndLevelTagTest()
		{
			// メッセージにヘッダと、必要に応じてログレベルのタグを付加する
			Assert.AreEqual(
				header + "[Error][Fatal]" + message,
				LogMessageCreater.CreateMessage(header, LogLevel.Fatal, message));

			Assert.AreEqual(
				header + "[Error]" + message,
				LogMessageCreater.CreateMessage(header, LogLevel.Error, message));

			Assert.AreEqual(
				header + "[Warning]" + message,
				LogMessageCreater.CreateMessage(header, LogLevel.Warning, message));

			Assert.AreEqual(
				header + message,
				LogMessageCreater.CreateMessage(header, LogLevel.Notice, message));

			Assert.AreEqual(
				header + message,
				LogMessageCreater.CreateMessage(header, LogLevel.Information, message));

			Assert.AreEqual(
				header + "[Debug]" + message,
				LogMessageCreater.CreateMessage(header, LogLevel.Debug, message));
		}

		[Test] // 要件２
		public void CreateDebugOutputMessage_OmitKeywordTest()
		{
			Assert.AreEqual(
				message,
				LogMessageCreater.CreateMessage(null, LogLevel.Information, message));

			Assert.AreEqual(
				message,
				LogMessageCreater.CreateMessage(string.Empty, LogLevel.Information, message));
		}
	}
}
