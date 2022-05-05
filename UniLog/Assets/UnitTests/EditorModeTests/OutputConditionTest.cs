using NUnit.Framework;
using UniLog;

namespace Tests
{
	// OutputCondition のテスト
	public class OutputConditionTest
	{
		// Acceptable のテスト
		// 要件１：enable が真の場合、引数で指定されたログレベルが level 以下であれば真、そうでなければ偽を返す
		// 要件２：enable が偽の場合、常に偽を返す
		[Test] // 要件１
		public void Acceptable_LevelCkeckTest(
			[Range((int)LogLevel.Fatal, (int)LogLevel.Debug)] int settingLevel,
			[Range((int)LogLevel.Fatal, (int)LogLevel.Debug)] int messageLevel)
		{
			OutputCondition condition = new OutputCondition { enable = true, level = (LogLevel)settingLevel };
			Assert.IsTrue((messageLevel <= settingLevel) == condition.Acceptable((LogLevel)messageLevel));
		}

		[Test] // 要件２
		public void Acceptable_DisableTest(
			[Range((int)LogLevel.Fatal, (int)LogLevel.Debug)] int settingLevel,
			[Range((int)LogLevel.Fatal, (int)LogLevel.Debug)] int messageLevel)
		{
			OutputCondition condition = new OutputCondition { enable = false, level = (LogLevel)settingLevel };
			Assert.IsFalse(condition.Acceptable((LogLevel)messageLevel));
		}
	}
}
