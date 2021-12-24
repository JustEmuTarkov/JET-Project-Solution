using NLog.Targets;
using UnityEngine;

namespace JET
{
	[Target("JET")]
	public sealed class Target : TargetWithLayout
	{
		public Target()
		{
			//Loader<Instance>.Load();
			GameObject result = GameObject.Find("JET Instance");
			if (result == null)
			{
				MonoBehaviour.JET_Instance.Instance = new GameObject("JET Instance");
				Object.DontDestroyOnLoad(MonoBehaviour.JET_Instance.Instance);
				//initialize Jet core instance
				MonoBehaviour.JET_Instance.Instance.GetOrAddComponent<MonoBehaviour.JET_Instance>();
			}
		}
	}
}
