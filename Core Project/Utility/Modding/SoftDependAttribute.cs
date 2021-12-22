using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JET.Utility.Modding
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class SoftDependAttribute : Attribute
    {
        internal Type[] Mods { get; }

        /// <summary>
        /// Specifies mods to soft depend on. Soft depending on a mod will attempt to load the mod before yours,
        /// but will load your mod even if one or more of the specified mods are not present.
        /// </summary>
        /// <param name="mods">A list of mods to soft depend on.</param>
        public SoftDependAttribute(params Type[] mods)
        {
            Mods = mods;
        }
    }
}
