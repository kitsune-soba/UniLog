using NUnit.Framework;
using System.IO;
using UniLog;

namespace Tests
{
	// LogFileWriter のテスト
	public class LogFileWriterTest
	{
		private readonly string temporaryFileName = "temp_test.log";
		private readonly string invalidFileName = "\\\"invalid path test\"\\";

		// 一時ファイルを削除する
		private void ClearFile(string path)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		// ファイルの内容を読み取る
		private string ReadToEnd(string path)
		{
			using (StreamReader reader = new StreamReader(path))
			{
				return reader.ReadToEnd();
			}
		}

		[SetUp]
		public void SetUp()
		{
			ClearFile(temporaryFileName);
		}

		[TearDown]
		public void TearDown()
		{
			ClearFile(temporaryFileName);
		}

		// コンストラクタのテスト
		// 要件１：引数で指定されたパスのファイルをオープンする
		// 要件２：引数で指定されたモードでファイルをオープンする（append = true: 追記モード、append = false: 上書きモード）
		// 要件３：ファイルのオープン時にログのヘッダ情報を書き込む
		// 要件４：ファイルのオープンに失敗した場合、System.IO.StreamWriter と同等の例外を投げる
		[Test] // 要件１
		public void Constructor_FileOpenTest()
		{
			Assert.IsFalse(File.Exists(temporaryFileName));
			using (LogFileWriter writer = new LogFileWriter(temporaryFileName, true)) { }
			Assert.IsTrue(File.Exists(temporaryFileName));
		}

		[Test]　// 要件２
		public void Constructor_OpenModeTest()
		{
			string appendString = "foobar";
			string writeString = "hogefuga";

			// テストの下準備として、一旦ファイルを開いて適当なログを書き込む
			using (LogFileWriter writer = new LogFileWriter(temporaryFileName, true))
			{
				writer.WriteLine(appendString);
			}
			Assert.IsTrue(ReadToEnd(temporaryFileName).Contains(appendString)); // 書き込まれている事を一応確認

			// 上書きモードで開くテスト（以前に書き込んだ内容はクリアされるはず）
			using (LogFileWriter writer = new LogFileWriter(temporaryFileName, false))
			{
				writer.WriteLine(writeString);
			}
			Assert.IsFalse(ReadToEnd(temporaryFileName).Contains(appendString)); // 以前の書き込みがクリアされている
			Assert.IsTrue(ReadToEnd(temporaryFileName).Contains(writeString)); // 今回の書き込みを確認

			// もう一度追記モードで開くテスト（以前に書き込んだ内容が残っているはず）
			using (LogFileWriter writer = new LogFileWriter(temporaryFileName, true))
			{
				writer.WriteLine(appendString);
			}
			Assert.IsTrue(ReadToEnd(temporaryFileName).Contains(appendString)); // 以前の書き込みが残っている
			Assert.IsTrue(ReadToEnd(temporaryFileName).Contains(writeString)); // 今回の書き込みも存在する
		}

		[Test] // 要件３
		public void Constructor_HeaderTest()
		{
			using (LogFileWriter writer = new LogFileWriter(temporaryFileName, false)) { }
			Assert.AreNotEqual(string.Empty, ReadToEnd(temporaryFileName)); // 何かが書き込まれている
		}

		[Test] // 要件４
		public void Constructor_ExceptionTest()
		{
			Assert.Throws<System.ArgumentException>(() => new LogFileWriter(invalidFileName, false));
		}

		// WriteLine のテスト
		// 要件１：引数で指定された文字列 + 改行がファイルに書き込まれる
		[Test] // 要件１
		public void WriteLine_WriteTest()
		{
			string writeString = "foobar";
			using (LogFileWriter writer = new LogFileWriter(temporaryFileName, false))
			{
				writer.WriteLine(writeString);
			}

			string str = ReadToEnd(temporaryFileName);
			Assert.IsTrue(str.Contains(writeString)); // 書き込まれている
			Assert.IsTrue(str.EndsWith("\n")); // 末尾に改行が書き込まれている
		}
	}
}
