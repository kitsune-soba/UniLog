using NUnit.Framework;
using UniLog;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	// Settings のテスト
	// Settings のテストは基本的にエディタモードテストで行い、
	// プレイモード中でのみ確認できる機能のみをここでテストする
	public class SettingsPlayModeTest
	{
		private readonly string testPatternSettingsPath_1 = "TestPatternSettings_1";
		private readonly string testPatternSettingsPath_2 = "TestPatternSettings_2";

		// GetCurrentDebugOutputCondition のテスト
		// 要件１：Unityエディタ以外での再生時、detailMode が偽であれば、通常モードのデバッグ出力条件の設定を返す
		// 要件２：Unityエディタ以外での再生時、detailMode が真であれば、詳細モードのデバッグ出力条件の設定を返す
		// 要件３：Unityエディタでの再生時には、エディタモードのデバッグ出力条件の設定を返す（エディタモードテストで確認）
		// ※ 現状、エディタモード以外で実装してあるのがWin版だけなので、Win環境のみテストする
		[Test, UnityPlatform(RuntimePlatform.WindowsPlayer)] // 要件１
		public void GetCurrentDebugOutputCondition_DefaultModeTest()
		{
			Settings settings1 = Resources.Load<Settings>(testPatternSettingsPath_1);
			OutputCondition condition1 = settings1.GetCurrentDebugOutputCondition();
			Assert.IsFalse(condition1.enable);
			Assert.AreEqual(LogLevel.Fatal, condition1.level);

			Settings settings2 = Resources.Load<Settings>(testPatternSettingsPath_2);
			OutputCondition condition2 = settings2.GetCurrentDebugOutputCondition();
			Assert.IsTrue(condition2.enable);
			Assert.AreEqual(LogLevel.Debug, condition2.level);
		}

		[Test, UnityPlatform(RuntimePlatform.WindowsPlayer)] // 要件２
		public void GetCurrentDebugOutputCondition_DetailModeTest()
		{
			Settings settings1 = Resources.Load<Settings>(testPatternSettingsPath_1);
			settings1.detailMode = true;
			OutputCondition condition1 = settings1.GetCurrentDebugOutputCondition();
			Assert.IsTrue(condition1.enable);
			Assert.AreEqual(LogLevel.Error, condition1.level);
			settings1.detailMode = false; // 他のテストに影響してしまうようなので初期状態に戻しておく

			Settings settings2 = Resources.Load<Settings>(testPatternSettingsPath_2);
			settings2.detailMode = true;
			OutputCondition condition2 = settings2.GetCurrentDebugOutputCondition();
			Assert.IsFalse(condition2.enable);
			Assert.AreEqual(LogLevel.Information, condition2.level);
			settings2.detailMode = false; // 他のテストに影響してしまうようなので初期状態に戻しておく
		}

		// GetCurrentLogFileCondition のテスト
		// 要件１：Unityエディタ以外での再生時、detailMode が偽であれば、通常モードのログファイル出力条件の設定を返す
		// 要件２：Unityエディタ以外での再生時、detailMode が真であれば、詳細モードのログファイル出力条件の設定を返す
		// 要件３：Unityエディタでの再生時には、エディタモードのログファイル出力条件の設定を返す（エディタモードテストで確認）
		// ※ 現状、エディタモード以外で実装してあるのがWin版だけなので、Win環境のみテストする
		[Test, UnityPlatform(RuntimePlatform.WindowsPlayer)] // 要件２
		public void GetCurrentLogFileCondition_DefaultModeTest()
		{
			Settings settings1 = Resources.Load<Settings>(testPatternSettingsPath_1);
			OutputCondition condition1 = settings1.GetCurrentLogFileCondition();
			Assert.IsTrue(condition1.enable);
			Assert.AreEqual(LogLevel.Notice, condition1.level);

			Settings settings2 = Resources.Load<Settings>(testPatternSettingsPath_2);
			OutputCondition condition2 = settings2.GetCurrentLogFileCondition();
			Assert.IsFalse(condition2.enable);
			Assert.AreEqual(LogLevel.Warning, condition2.level);
		}

		[Test, UnityPlatform(RuntimePlatform.WindowsPlayer)] // 要件３
		public void GetCurrentLogFileCondition_DetailModeTest()
		{
			Settings settings1 = Resources.Load<Settings>(testPatternSettingsPath_1);
			settings1.detailMode = true;
			OutputCondition condition1 = settings1.GetCurrentLogFileCondition();
			Assert.IsFalse(condition1.enable);
			Assert.AreEqual(LogLevel.Information, condition1.level);
			settings1.detailMode = false; // 他のテストに影響してしまうようなので初期状態に戻しておく

			Settings settings2 = Resources.Load<Settings>(testPatternSettingsPath_2);
			settings2.detailMode = true;
			OutputCondition condition2 = settings2.GetCurrentLogFileCondition();
			Assert.IsTrue(condition2.enable);
			Assert.AreEqual(LogLevel.Error, condition2.level);
			settings2.detailMode = false; // 他のテストに影響してしまうようなので初期状態に戻しておく
		}

		// GetLogFilePath のテスト
		// 要件１：PC(Windows, Mac, Linux) 環境であれば、設定されたパスをそのまま返す（エディタモードテスト及びプレイモードテストで確認）
		// 要件２：PC以外の環境であれば、Application.persistentDataPath を基準としたパスを返す
		[Test, UnityPlatform( // 要件１
			RuntimePlatform.WindowsPlayer,
			RuntimePlatform.LinuxPlayer,
			RuntimePlatform.OSXPlayer)]
		public void GetLogFilePath_PCTest()
		{
			Settings settings1 = Resources.Load<Settings>(testPatternSettingsPath_1);
			Settings settings2 = Resources.Load<Settings>(testPatternSettingsPath_2);

			Assert.AreEqual("test1.log", settings1.GetLogFilePath());
			Assert.AreEqual("test2.log", settings2.GetLogFilePath());
		}

		[Test, UnityPlatform(RuntimePlatform.IPhonePlayer)] // 要件２（エディタ、PC以外向けのテストだが、RuntimePlatformの羅列がしんどいので取り敢えずiPhone向けのテストとして書いておく）
		public void GetLogFilePath_OtherTest()
		{
			Settings settings1 = Resources.Load<Settings>(testPatternSettingsPath_1);
			Settings settings2 = Resources.Load<Settings>(testPatternSettingsPath_2);

			Assert.AreEqual(Application.persistentDataPath + '/' + "test1.log", settings1.GetLogFilePath());
			Assert.AreEqual(Application.persistentDataPath + '/' + "test2.log", settings2.GetLogFilePath());
		}
	}
}
