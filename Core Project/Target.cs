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
				result = new GameObject("JET Instance");
				Object.DontDestroyOnLoad(result);
				//initialize Jet core instance
				result.GetOrAddComponent<MonoBehaviour.JET_Instance>();
			}
		}
	}
}
