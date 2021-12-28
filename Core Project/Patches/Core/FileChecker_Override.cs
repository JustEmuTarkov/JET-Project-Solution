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
	/// Patch that disables internal integrity checks (completly), so we can run "deobfuscated" assembly-csharp
	/// You can use that to add your own integrity checks
	/// </summary>
	class FileChecker_Override : GenericPatch<FileChecker_Override>
	{
		public FileChecker_Override() : base(prefix: nameof(PatchPrefix)) { }

		private string _ClassName = "ConsistencyMetadataProvider";
		private string _MethodName = "GetConsistencyMetadata";

		protected override MethodBase GetTargetMethod()
		{
			return Constants.Instance.FileCheckerAssemblyTypes
				.Single(Class => Class.Name == _ClassName)
				.GetMethod(_MethodName, Constants.Instance.PublicInstanceFlag);
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
