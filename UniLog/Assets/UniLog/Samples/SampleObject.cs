using UniLog;
using UnityEngine;

public class SampleObject : MonoBehaviour
{
	private void Start()
	{
		//Log.settings.detailMode = true; // 詳細モードへの切り替え

		Log.WriteLine("アプリケーションが続行不能なヤバいエラーが発生", LogLevel.Fatal);
		Log.WriteLine("エラーが発生", LogLevel.Error);
		Log.WriteLine("エラーではないが、注意を要する状態", LogLevel.Warning);
		Log.WriteLine("正常系で、重要度の高い情報", LogLevel.Notice);
		Log.WriteLine("普通の情報"); // これと等しい：Log.WriteLine("普通の情報", LogLevel.Information);
		Log.WriteLine("基本的にユーザに公開すべきでない、開発者向けの詳細情報", LogLevel.Debug);
	}
}
