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
				Mono.JET_Instance.Instance = new GameObject("JET Instance");
				Object.DontDestroyOnLoad(Mono.JET_Instance.Instance);
				//initialize Jet core instance
				Mono.JET_Instance.Instance.GetOrAddComponent<Mono.JET_Instance>();
			}
		}
	}
}
