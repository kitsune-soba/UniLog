using NUnit.Framework;
using System.IO;
using UniLog;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	// LogOutput のテスト
	public class LogOutputTest
	{
		private readonly string header = "[LogOutputTest]";
		private readonly string noticeSettingsPath = "NoticeSettings";
		private readonly string invalidLogPathSettingsPath = "InvalidLogPathSettings";
		private readonly string disabledSettingsPath = "DisabledSettings";
		private readonly string invalidSettingsPath = "invalid path test";

		private void ClearFile(string path)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		// コンストラクタのテスト
		// 要件１：引数で指定されたパスの設定をロードし、status フラグが Fine となる。
		// 要件２：設定のロードに失敗すると status フラグが SettingsLoadError となり、それ以降のログ出力は無視される
		[Test] // 要件１
		public void Constructor_LoadSettingsTest()
		{
			using (LogOutput output = new LogOutput(noticeSettingsPath))
			{
				Assert.AreEqual(Resources.Load<Settings>(noticeSettingsPath), output.settings);
				Assert.AreEqual(LogOutput.Status.Fine, output.status);
			}
		}

		[Test] // 要件２
		public void Constructor_LoadSettingsFailTest()
		{
			LogAssert.ignoreFailingMessages = true;

			using (LogOutput output = new LogOutput(invalidSettingsPath))
			{
				Assert.AreEqual(LogOutput.Status.SettingsLoadError, output.status);
			}
		}

		// status のテスト
		// 要件１：特に異常が無ければ Fine である
		// 要件２：設定のロードに失敗した場合は status の SettingsLoadError ビットが立つ
		// 要件３：ログファイルの書き込みが失敗した場合は　status の LogFileIOError ビットが立つ
		[Test] // 要件１
		public void Status_FineTest()
		{
			using (LogOutput output = new LogOutput(noticeSettingsPath))
			{
				Assert.AreEqual(Resources.Load<Settings>(noticeSettingsPath), output.settings);
			}
		}

		[Test] // 要件２
		public void Status_SettingsLoadErrorTest()
		{
			LogAssert.ignoreFailingMessages = true;
			using (LogOutput output = new LogOutput(invalidSettingsPath))
			{
				Assert.AreEqual(LogOutput.Status.SettingsLoadError, output.status);
			}
		}

		[Test] // 要件３
		public void Status_LogFileIOErrorTest()
		{
			LogAssert.ignoreFailingMessages = true;
			using (LogOutput output = new LogOutput(invalidLogPathSettingsPath))
			{
				output.WriteLine("", LogLevel.Information, header); // ファイルを開くためにログを書き込む
				Assert.AreEqual(LogOutput.Status.LogFileIOError, output.status);
			}
		}

		// settings のテスト
		// 要件１：コンストラクタでロードされた設定を返す
		[Test] // 要件１
		public void Settings_ReturnSettingsTest()
		{
			using (LogOutput output = new LogOutput(noticeSettingsPath))
			{
				Assert.AreEqual(Resources.Load<Settings>(noticeSettingsPath), output.settings);
			}
		}

		// WriteLine のテスト
		// 要件１：status フラグが Fine の場合、コンストラクタでロードされた設定の条件を満たせば、デバッグ出力およびログファイル出力を行う
		// 要件２：初めてログファイルへ出力する時にログファイルをオープンする。オープンに失敗した場合はログファイルへ出力しない。
		// 要件３：status フラグの SettingsLoadError ビットが立っている場合、何もしない
		// 要件４：status フラグの LogFileIOError ビットが立っている場合、ログファイルへの書き込みは行わない
		[Test] // 要件１
		public void WriteLine_FineTest()
		{
			LogAssert.ignoreFailingMessages = true;
			string logPath = string.Empty;

			// 出力が有効で、ログレベルが　LogLevel.Notice である設定ファイルがロードされていれば、
			// ログレベルが LogLevel.Notice 以下のメッセージのみ出力される
			using (LogOutput output = new LogOutput(noticeSettingsPath))
			{
				logPath = output.settings.GetLogFilePath();
				ClearFile(logPath);

				// 条件の確認
				Assert.AreEqual(LogOutput.Status.Fine, output.status);
				OutputCondition debugOutputCondition = output.settings.GetCurrentDebugOutputCondition();
				Assert.IsTrue(debugOutputCondition.enable);
				Assert.AreEqual(LogLevel.Notice, debugOutputCondition.level);
				OutputCondition logFileCondition = output.settings.GetCurrentLogFileCondition();
				Assert.IsTrue(logFileCondition.enable);
				Assert.AreEqual(LogLevel.Notice, logFileCondition.level);

				// 出力されないテスト
				output.WriteLine("This is a message that should NOT be output.", LogLevel.Debug, header);
				output.WriteLine("This is a message that should NOT be output.", LogLevel.Information, header);
				Assert.IsFalse(File.Exists(logPath));

				// 出力されるテスト
				output.WriteLine("This is a message that should be output.", LogLevel.Notice, header);
				output.WriteLine("This is a message that should be output.", LogLevel.Warning, header);
				output.WriteLine("This is a message that should be output.", LogLevel.Error, header);
				output.WriteLine("This is a message that should be output.", LogLevel.Fatal, header);
				Assert.IsTrue(File.Exists(logPath));
			}
			ClearFile(logPath);

			// 出力が無効の場合、出力されない
			using (LogOutput output = new LogOutput(disabledSettingsPath))
			{
				logPath = output.settings.GetLogFilePath();
				ClearFile(logPath);

				// 条件の確認
				Assert.AreEqual(LogOutput.Status.Fine, output.status);
				Assert.IsFalse(output.settings.GetCurrentDebugOutputCondition().enable);
				Assert.IsFalse(output.settings.GetCurrentLogFileCondition().enable);

				// 出力されないテスト
				output.WriteLine("This is a message that should NOT be output.", LogLevel.Debug, header);
				output.WriteLine("This is a message that should NOT be output.", LogLevel.Information, header);
				output.WriteLine("This is a message that should NOT be output.", LogLevel.Notice, header);
				output.WriteLine("This is a message that should NOT be output.", LogLevel.Warning, header);
				output.WriteLine("This is a message that should NOT be output.", LogLevel.Error, header);
				output.WriteLine("This is a message that should NOT be output.", LogLevel.Fatal, header);
				Assert.IsFalse(File.Exists(logPath));
			}
		}

		[Test] // 要件２
		public void WriteLine_OpenLogFileTest()
		{
			string logPath = string.Empty;

			using (LogOutput output = new LogOutput(noticeSettingsPath))
			{
				logPath = output.settings.GetLogFilePath();
				ClearFile(logPath);

				Assert.IsFalse(File.Exists(logPath));
				output.WriteLine("This is a message that should NOT be output.", LogLevel.Debug, header);
				Assert.IsFalse(File.Exists(logPath));
				output.WriteLine("This is a message that should be output.", LogLevel.Notice, header); // 初めてログファイルへ出力する
				Assert.IsTrue(File.Exists(logPath));
			}
			ClearFile(logPath);
		}

		[Test] // 要件３
		public void WriteLine_SettingsLoadErrorTest()
		{
			LogAssert.ignoreFailingMessages = true;
			using (LogOutput output = new LogOutput(invalidSettingsPath))
			{
				Assert.IsTrue(output.status.HasFlag(LogOutput.Status.SettingsLoadError));
				output.WriteLine("This is a message that should NOT be output.", LogLevel.Fatal, header); // 何も起きないはず
			}
		}

		[Test] // 要件４
		public void WriteLine_LogFileIOErrorTest()
		{
			LogAssert.ignoreFailingMessages = true;
			using (LogOutput output = new LogOutput(invalidLogPathSettingsPath))
			{
				output.WriteLine("This is a message that should NOT be output.", LogLevel.Information, header); // ファイルを開くためにログを書き込む
				Assert.IsTrue(output.status.HasFlag(LogOutput.Status.LogFileIOError));
				output.WriteLine("This is a message that should NOT be output.", LogLevel.Information, header); // ログファイルへの出力は行われない
			}
		}
	}
}
