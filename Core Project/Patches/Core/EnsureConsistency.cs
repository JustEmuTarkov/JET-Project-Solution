using FilesChecker;
using JET.Utility.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JET.Patches.Core
{
	/// <summary>
	/// Patch that disables internal integrity checks so we can run "deobfuscated" assembly-csharp
	/// </summary>
    class EnsureConsistency : GenericPatch<EnsureConsistency>
	{
		public EnsureConsistency() : base(prefix: nameof(PatchPrefix)) { }

		private string _ClassName = "ConsistencyMetadataProvider";
		private string _MethodName = "GetConsistencyMetadata";

		protected override MethodBase GetTargetMethod()
		{
			return Constants.FileCheckerAssemblyTypes
				.Single(Class => Class.Name == _ClassName)
				.GetMethod(_MethodName, Constants.PublicInstanceFlag);
		}

		static bool PatchPrefix(ref System.Collections.Generic.IReadOnlyList<FileConsistencyMetadata> __result)
		{
			__result = new FileConsistencyMetadata[] { }; 
			/*
			you can add your own files to check to disallow game to run if something is not found 
			filename, size, checksum, is critical
			new FileConsistencyMetadata("EscapeFromTarkov.exe", 661712L, 72195026, true),
			__result = Task.FromResult<ICheckResult>(new ScanResult());
			*/
			return false;
		}
	}
	//class ScanResult : ICheckResult
	//{
	//	public TimeSpan ElapsedTime { get; private set; }
	//	public Exception Exception { get; private set; }

	//	public ScanResult()
	//	{
	//		ElapsedTime = new TimeSpan(5);
	//		Exception = null;

	//	}
	//}
}
