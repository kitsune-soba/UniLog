using System;
using UnityEngine;

namespace UniLog
{
	// 出力条件
	[Serializable]
	public class OutputCondition
	{
		[SerializeField]
		public bool enable;
		[SerializeField]
		public LogLevel level;

		// メッセージ（のレベル）が出力に値するか確認する
		public bool Acceptable(LogLevel messageLevel)
		{
			return enable && (messageLevel <= level);
		}
	}
}
