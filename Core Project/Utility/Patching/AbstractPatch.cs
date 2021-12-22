using System.Reflection;

namespace JET.Utility.Patching
{
	public abstract class AbstractPatch
	{
		public string methodName;
		public BindingFlags flags;

		public abstract MethodInfo TargetMethod();
	}
}
