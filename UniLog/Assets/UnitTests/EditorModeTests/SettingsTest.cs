using NUnit.Framework;
using UniLog;
using UnityEngine;

namespace Tests
{
	// Settings のテスト
	public class SettingsTest
	{
		private readonly string testPatternSettingsPath_1 = "TestPatternSettings_1";
		private readonly string testPatternSettingsPath_2 = "TestPatternSettings_2";

		// detailMode のテスト
		// 要件１：初期状態が偽であること
		[Test] // 要件１
		public void DetailMode_InitialValueTest()
		{
			Settings settings = Resources.Load<Settings>(testPatternSettingsPath_1);
			Assert.IsFalse(settings.detailMode);
		}

		// filteringKeyword のテスト
		// 要件１：設定されたフィルタリング用キーワードを返す
		[Test] // 要件１
		public void FilteringKeyword_Test()
		{
			Settings settings1 = Resources.Load<Settings>(testPatternSettingsPath_1);
			Settings settings2 = Resources.Load<Settings>(testPatternSettingsPath_2);

			Assert.AreEqual("[TestPatternSettings_1]", settings1.filteringKeyword);
			Assert.AreEqual("[TestPatternSettings_2]", settings2.filteringKeyword);
		}

		// logFileAppend のテスト
		// 要件１：設定されたログファイルの追記モードのフラグを返す
		[Test] // 要件１
		public void LogFileAppend_Test()
		{
			Settings settings1 = Resources.Load<Settings>(testPatternSettingsPath_1);
			Settings settings2 = Resources.Load<Settings>(testPatternSettingsPath_2);

			Assert.IsFalse(settings1.logFileAppend);
			Assert.IsTrue(settings2.logFileAppend);
		}

		// GetCurrentDebugOutputCondition のテスト
		// 要件１：Unityエディタ以外での再生時、detailMode が偽であれば、通常モードのデバッグ出力条件の設定を返す（プレイモードテストで確認）
		// 要件２：Unityエディタ以外での再生時、detailMode が真であれば、詳細モードのデバッグ出力条件の設定を返す（プレイモードテストで確認）
		// 要件３：Unityエディタでの再生時には、エディタモードのデバッグ出力条件の設定を返す
		[Test] // 要件３
		public void GetCurrentDebugOutputCondition_EditorModeTest()
		{
			Settings settings1 = Resources.Load<Settings>(testPatternSettingsPath_1);
			Settings settings2 = Resources.Load<Settings>(testPatternSettingsPath_2);

			// detailMode が偽であっても、エディタでの再生時はエディタモードとなる
			{
				OutputCondition condition1 = settings1.GetCurrentDebugOutputCondition();
				Assert.IsFalse(condition1.enable);
				Assert.AreEqual(LogLevel.Warning, condition1.level);

				OutputCondition condition2 = settings2.GetCurrentDebugOutputCondition();
				Assert.IsTrue(condition2.enable);
				Assert.AreEqual(LogLevel.Notice, condition2.level);
			}

			// detailMode が真であっても、エディタでの再生時はエディタモードとなる
			settings1.detailMode = true;
			settings2.detailMode = true;
			{
				OutputCondition condition1 = settings1.GetCurrentDebugOutputCondition();
				Assert.IsFalse(condition1.enable);
				Assert.AreEqual(LogLevel.Warning, condition1.level);

				OutputCondition condition2 = settings2.GetCurrentDebugOutputCondition();
				Assert.IsTrue(condition2.enable);
				Assert.AreEqual(LogLevel.Notice, condition2.level);
			}

			// 他のテストに影響してしまうようなので初期状態に戻しておく
			settings1.detailMode = false;
			settings2.detailMode = false;
		}

		// GetCurrentLogFileCondition のテスト
		// 要件１：Unityエディタ以外での再生時、detailMode が偽であれば、通常モードのログファイル出力条件の設定を返す（プレイモードテストで確認）
		// 要件２：Unityエディタ以外での再生時、detailMode が真であれば、詳細モードのログファイル出力条件の設定を返す（プレイモードテストで確認）
		// 要件３：Unityエディタでの再生時には、エディタモードのログファイル出力条件の設定を返す
		[Test] // 要件３
		public void GetCurrentLogFileCondition_EditorModeTest()
		{
			Settings settings1 = Resources.Load<Settings>(testPatternSettingsPath_1);
			Settings settings2 = Resources.Load<Settings>(testPatternSettingsPath_2);

			// detailMode が偽であっても、エディタでの再生時はエディタモードとなる
			{
				OutputCondition condition1 = settings1.GetCurrentLogFileCondition();
				Assert.IsTrue(condition1.enable);
				Assert.AreEqual(LogLevel.Debug, condition1.level);

				OutputCondition condition2 = settings2.GetCurrentLogFileCondition();
				Assert.IsFalse(condition2.enable);
				Assert.AreEqual(LogLevel.Fatal, condition2.level);
			}

			// detailMode が真であっても、エディタでの再生時はエディタモードとなる
			settings1.detailMode = true;
			settings2.detailMode = true;
			{
				OutputCondition condition1 = settings1.GetCurrentLogFileCondition();
				Assert.IsTrue(condition1.enable);
				Assert.AreEqual(LogLevel.Debug, condition1.level);

				OutputCondition condition2 = settings2.GetCurrentLogFileCondition();
				Assert.IsFalse(condition2.enable);
				Assert.AreEqual(LogLevel.Fatal, condition2.level);
			}

			// 他のテストに影響してしまうようなので初期状態に戻しておく
			settings1.detailMode = false;
			settings2.detailMode = false;
		}

		// GetLogFilePath のテスト
		// 要件１：PC(Windows, Mac, Linux) 環境であれば、設定されたパスをそのまま返す（エディタモードテスト及びプレイモードテストで確認）
		// 要件２：PC以外の環境であれば、Application.persistentDataPath を基準としたパスを返す（プレイモードテストで確認）
		[Test] // 要件１
		public void GetLogFilePath_EditorTest()
		{
			Settings settings1 = Resources.Load<Settings>(testPatternSettingsPath_1);
			Settings settings2 = Resources.Load<Settings>(testPatternSettingsPath_2);

			Assert.AreEqual("test1.log", settings1.GetLogFilePath());
			Assert.AreEqual("test2.log", settings2.GetLogFilePath());
		}
	}
}
