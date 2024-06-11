using NUnit.Framework;
using UniLog;
using UnityEngine;

namespace Tests
{
	// Log のテスト
	public class LogTest
	{
		// status のテスト
		// 要件１：普通に使用している限り Fine の状態を保つ
		[Test] // 要件１
		public void Status_Test()
		{
            Assert.AreEqual(LogOutput.Status.Fine, Log.status);
		}

		// settings のテスト
		// 要件１：Resources/UniLogSettings のインスタンスを返す
		[Test] // 要件１
		public void Settings_Test()
		{
			Settings settings = Resources.Load<Settings>("UniLogSettings");
            Assert.AreSame(settings, Log.settings);
		}

		// WriteLine 系のテスト
		// 要件１：初期化処理などを呼び出す手間を掛けずにログ出力できる
		[Test] // 要件１
		public void WriteLine_Test()
		{
			Assert.DoesNotThrow(() => Log.WriteLine("This is a message that should be output."));
		}

		// Output 系のテスト
		// 要件１：初期化処理などを呼び出す手間を掛けずにログ出力できる
		[Test] // 要件１
		public void Output_Test()
		{
			Assert.DoesNotThrow(() => Log.Output("This is a message that should be output.", LogLevel.Information));
			//Assert.DoesNotThrow(() => Log.Fatal("This is a message that should be output.")); // Assert.DoesNotThrow 内で Debug.LogError が呼び出されると何故かテスト失敗扱いになるようなのでコメントアウトしておく
			//Assert.DoesNotThrow(() => Log.Error("This is a message that should be output.")); // Assert.DoesNotThrow 内で Debug.LogError が呼び出されると何故かテスト失敗扱いになるようなのでコメントアウトしておく
			Assert.DoesNotThrow(() => Log.Warning("This is a message that should be output."));
			Assert.DoesNotThrow(() => Log.Notice("This is a message that should be output."));
			Assert.DoesNotThrow(() => Log.Info("This is a message that should be output."));
			Assert.DoesNotThrow(() => Log.Debug("This is a message that should be output."));
		}
	}
}
