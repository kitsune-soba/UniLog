using NUnit.Framework;
using UniLog;

namespace Tests
{
	// LogMessageCreater のテスト
	public class LogMessageCreaterTest
	{
		private readonly string keyword = "[UniLogTest]";
		private readonly string message = "Test message.";

		// CreateDebugOutputMessage のテスト
		// 要件１：引数で指定されたメッセージに、フィルタリング用のキーワードと、必要に応じてログレベルのタグを付加する
		// 要件２：引数で指定されたフィルタリングキーワードとして空文字が渡された場合、単にキーワードの付加が省略される
		[Test] // 要件１
		public void CreateDebugOutputMessage_AddKeywordAndLevelTagTest()
		{
			// メッセージにフィルタリング用キーワードと、必要に応じてログレベルのタグを付加する
			Assert.AreEqual(
				keyword + "[Error][Fatal]" + message,
				LogMessageCreater.CreateDebugOutputMessage(keyword, LogLevel.Fatal, message));

			Assert.AreEqual(
				keyword + "[Error]" + message,
				LogMessageCreater.CreateDebugOutputMessage(keyword, LogLevel.Error, message));

			Assert.AreEqual(
				keyword + "[Warning]" + message,
				LogMessageCreater.CreateDebugOutputMessage(keyword, LogLevel.Warning, message));

			Assert.AreEqual(
				keyword + message,
				LogMessageCreater.CreateDebugOutputMessage(keyword, LogLevel.Notice, message));

			Assert.AreEqual(
				keyword + message,
				LogMessageCreater.CreateDebugOutputMessage(keyword, LogLevel.Information, message));

			Assert.AreEqual(
				keyword + message,
				LogMessageCreater.CreateDebugOutputMessage(keyword, LogLevel.Debug, message));
		}

		[Test] // 要件２
		public void CreateDebugOutputMessage_OmitKeywordTest()
		{
			Assert.AreEqual(
				message,
				LogMessageCreater.CreateDebugOutputMessage(string.Empty, LogLevel.Information, message));
		}

		// CreateLogFileMessage のテスト
		// 要件１：引数で指定されたメッセージに、必要に応じてログレベルのタグを付加する
		[Test] // 要件１
		public void CreateLogFileMessage_AddLevelTagTest()
		{
			Assert.AreEqual(
				"[Error][Fatal]" + message,
				LogMessageCreater.CreateLogFileMessage(LogLevel.Fatal, message));

			Assert.AreEqual(
				"[Error]" + message,
				LogMessageCreater.CreateLogFileMessage(LogLevel.Error, message));

			Assert.AreEqual(
				"[Warning]" + message,
				LogMessageCreater.CreateLogFileMessage(LogLevel.Warning, message));

			Assert.AreEqual(
				message,
				LogMessageCreater.CreateLogFileMessage(LogLevel.Notice, message));

			Assert.AreEqual(
				message,
				LogMessageCreater.CreateLogFileMessage(LogLevel.Information, message));

			Assert.AreEqual(
				message,
				LogMessageCreater.CreateLogFileMessage(LogLevel.Debug, message));
		}
	}
}
