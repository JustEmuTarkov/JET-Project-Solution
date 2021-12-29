using NLog.Targets;
using UnityEngine;

namespace JET
{
	[Target("JET")]
	public sealed class Target : TargetWithLayout
	{
		/// <summary>
		/// A constructor used by NLog to auto load when game starts
		/// </summary>
		public Target()
		{
			//check if instance isnt already inside - its mostly just in case
			GameObject result = GameObject.Find("JET Instance"); 
			if (result == null)
			{
				// now create empty game object in unity and set it to not get removed or scene change
				Mono.JET_Instance.Instance = new GameObject("JET Instance");
				Object.DontDestroyOnLoad(Mono.JET_Instance.Instance);
				// initialize Jet core instance
				Mono.JET_Instance.Instance.GetOrAddComponent<Mono.JET_Instance>();
			}
		}
	}
}
