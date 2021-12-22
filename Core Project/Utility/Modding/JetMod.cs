using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JET.Utility.Modding
{
    public abstract class JetMod
    {
        /// <summary>
        /// Called when your mod is first loaded.
        /// </summary>
        /// <param name="instances">A list of mod instances that your mod depends on.</param>
        /// <param name="gameVersion">The version of the client your mod has been loaded on.</param>
        protected internal abstract void Initialize(IReadOnlyDictionary<Type, JetMod> instances, string gameVersion);
    }
}
