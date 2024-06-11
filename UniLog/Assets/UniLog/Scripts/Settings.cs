using System.Diagnostics;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UniLog
{
	// ログ出力の設定
	//[CreateAssetMenu(menuName = "UniLog/UniLogSettings", fileName = "UniLogSettings")] // アセット作成時のスクリプタブルオブジェクト生成用。普段は必要ない。
	public class Settings : ScriptableObject
	{
		// デバッグ出力
		[SerializeField]
		private string header_ = string.Empty;
		[SerializeField]
		private OutputCondition debugOutputConditionInDefaultMode = new OutputCondition { enable = true, level = LogLevel.Information };
		[SerializeField]
		private OutputCondition debugOutputConditionInDetailMode = new OutputCondition { enable = true, level = LogLevel.Debug };
		[SerializeField]
		private OutputCondition debugOutputConditionOnEditor = new OutputCondition { enable = true, level = LogLevel.Debug };

		// ログファイル
		[SerializeField]
		private string logFilePathRaw = string.Empty;
		[SerializeField]
		private bool logFileAppend_ = false;
		[SerializeField]
		private OutputCondition logFileConditionInDefaultMode = new OutputCondition { enable = true, level = LogLevel.Notice };
		[SerializeField]
		private OutputCondition logFileConditionInDetailMode = new OutputCondition { enable = true, level = LogLevel.Debug };
		[SerializeField]
		private OutputCondition logFileConditionOnEditor = new OutputCondition { enable = false, level = LogLevel.Debug };

		// public bool detailMode { get; set; } = false; と書いて済ませたいところだが、
		// これだと detailModeが シリアライズされて、detailMode への代入がスクリプタブルオブジェクトのアセットを変更する操作となる。
		// つまり再生が終わっても代入された値が保持される。（但しエディタでの再生時のみ。）
		// 再生開始時は detailMode が false という仕様につき、DetailModeクラスで包むことでシリアライズを避ける。
		private class DetailMode { public bool value; }
		private DetailMode detailMode_ = new DetailMode { value = false };
		public bool detailMode { get => detailMode_.value; set => detailMode_.value = value; }

		public string header { get => string.IsNullOrEmpty(header_) ? $"[{Application.productName}]" : header_; }
		public bool logFileAppend { get => logFileAppend_; }

		// 現在有効なデバッグ出力の出力条件を返す
		public OutputCondition GetCurrentDebugOutputCondition()
		{
			if (Application.isEditor) { return debugOutputConditionOnEditor; }
			else if (detailMode) { return debugOutputConditionInDetailMode; }
			else { return debugOutputConditionInDefaultMode; }
		}

		// 現在有効なログファイル出力の出力条件を返す
		public OutputCondition GetCurrentLogFileCondition()
		{
			if (Application.isEditor) { return logFileConditionOnEditor; }
			else if (detailMode) { return logFileConditionInDetailMode; }
			else { return logFileConditionInDefaultMode; }
		}

		// ログファイルのパスを返す
		public string GetLogFilePath()
		{
			if (Application.isEditor ||
				(Application.platform == RuntimePlatform.LinuxPlayer) ||
				(Application.platform == RuntimePlatform.OSXPlayer) ||
				(Application.platform == RuntimePlatform.WindowsPlayer))
			{
				return string.IsNullOrEmpty(logFilePathRaw) ? $"{Application.productName}.log" : logFilePathRaw;
			}
			else
			{
				// PC以外の環境でどこにログファイルを出力すると便利か知らないので、取り敢えずこのようにしておく
				return Application.persistentDataPath + '/' + logFilePathRaw;
			}
		}

#if UNITY_EDITOR
		[CustomEditor(typeof(Settings))]
		public class SettingsEditor : Editor
		{
			private SerializedProperty header_;
			private SerializedProperty debugOutputConditionInDefaultMode;
			private SerializedProperty debugOutputConditionInDetailMode;
			private SerializedProperty debugOutputConditionOnEditor;
			private SerializedProperty logFilePathRaw;
			private SerializedProperty logFileAppend_;
			private SerializedProperty logFileConditionInDefaultMode;
			private SerializedProperty logFileConditionInDetailMode;
			private SerializedProperty logFileConditionOnEditor;

			private bool debugOutputFoldout = true;
			private bool logFileFoldout = true;

			private void OnEnable()
			{
				header_ = serializedObject.FindProperty(nameof(header_));
				debugOutputConditionInDefaultMode = serializedObject.FindProperty(nameof(debugOutputConditionInDefaultMode));
				debugOutputConditionInDetailMode = serializedObject.FindProperty(nameof(debugOutputConditionInDetailMode));
				debugOutputConditionOnEditor = serializedObject.FindProperty(nameof(debugOutputConditionOnEditor));
				logFilePathRaw = serializedObject.FindProperty(nameof(logFilePathRaw));
				logFileAppend_ = serializedObject.FindProperty(nameof(logFileAppend_));
				logFileConditionInDefaultMode = serializedObject.FindProperty(nameof(logFileConditionInDefaultMode));
				logFileConditionInDetailMode = serializedObject.FindProperty(nameof(logFileConditionInDetailMode));
				logFileConditionOnEditor = serializedObject.FindProperty(nameof(logFileConditionOnEditor));
			}

			public override void OnInspectorGUI()
			{
				serializedObject.Update();

				Settings settings = target as Settings;

				// デバッグ出力の設定を表示
				EditorGUILayout.LabelField("Debug output", EditorStyles.boldLabel);
				EditorGUI.indentLevel++;
				header_.stringValue = EditorGUILayout.TextField("Header", header_.stringValue);
				debugOutputFoldout = EditorGUILayout.Foldout(debugOutputFoldout, "Log Level");
				if (debugOutputFoldout)
				{
					ShowOutputCondition("Default Mode", debugOutputConditionInDefaultMode);
					ShowOutputCondition("Detail Mode", debugOutputConditionInDetailMode);
					ShowOutputCondition("Unity Editor", debugOutputConditionOnEditor);
				}
				EditorGUI.indentLevel--;

				// ログファイルの設定を表示
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Log file", EditorStyles.boldLabel);
				EditorGUI.indentLevel++;
				logFilePathRaw.stringValue = EditorGUILayout.TextField("Destination", logFilePathRaw.stringValue);
				logFileAppend_.boolValue = EditorGUILayout.Toggle("Append", logFileAppend_.boolValue);
				logFileFoldout = EditorGUILayout.Foldout(logFileFoldout, "Log Level");
				if (logFileFoldout)
				{
					ShowOutputCondition("Default Mode", logFileConditionInDefaultMode);
					ShowOutputCondition("Detail Mode", logFileConditionInDetailMode);
					ShowOutputCondition("Unity Editor", logFileConditionOnEditor);
				}
				EditorGUI.indentLevel--;

				// ログファイルがあれば、ログファイルを開くボタンを表示
				string logFilePath = settings.GetLogFilePath();
				if (File.Exists(logFilePath))
				{
					EditorGUILayout.Space();
					if (GUILayout.Button(string.Format("Open \"{0}\"", logFilePath)))
					{
						Process.Start(logFilePath);
					};
				}

				serializedObject.ApplyModifiedProperties();
			}

			// 出力条件の設定を表示する
			private void ShowOutputCondition(string label, SerializedProperty condition)
			{
				EditorGUILayout.BeginHorizontal();
				SerializedProperty enable = condition.FindPropertyRelative("enable");
				enable.boolValue = EditorGUILayout.Toggle(label, enable.boolValue);
				if (enable.boolValue)
				{
					SerializedProperty level = condition.FindPropertyRelative("level");
					level.enumValueIndex = (int)(LogLevel)EditorGUILayout.EnumPopup((LogLevel)level.enumValueIndex, GUILayout.MaxWidth(120));
				}
				else
				{
					EditorGUILayout.LabelField("Disabled");
				}
				GUILayout.FlexibleSpace(); // 左寄せにする
				EditorGUILayout.EndHorizontal();
			}
		}
#endif // UNITY_EDITOR
	}
}
