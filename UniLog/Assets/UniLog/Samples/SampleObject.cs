using System.Collections;
using UniLog;
using UnityEngine;

public class SampleObject : MonoBehaviour
{
	private void Start()
	{
		//Log.settings.detailMode = true; // 詳細モードへの切り替え

		StartCoroutine(LogIntermittent(0));
	}

	private IEnumerator LogIntermittent(float sleepTime)
	{
        Log.Fatal("アプリケーションが続行不能なヤバいエラーが発生");
		yield return new WaitForSeconds(sleepTime);
        Log.Error("エラーが発生");
		yield return new WaitForSeconds(sleepTime);
        Log.Warning("エラーではないが、注意を要する状態");
		yield return new WaitForSeconds(sleepTime);
        Log.Notice("正常系で、重要度の高い情報", "[CustomHeader]");
		yield return new WaitForSeconds(sleepTime);
        Log.Info("普通の情報", "");
		yield return new WaitForSeconds(sleepTime);
        Log.Debug("基本的にユーザに公開すべきでない、開発者向けの詳細情報");

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
