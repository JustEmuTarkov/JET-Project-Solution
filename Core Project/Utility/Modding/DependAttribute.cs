using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JET.Utility.Modding
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class DependAttribute : Attribute
    {
        internal Type[] Mods { get; }

        /// <summary>
        /// Specifies mods to depend on. Depending on a mod will load the mod before yours. If the specified mod cannot be loaded,
        /// your mod will not be loaded and an error will be displayed.
        /// </summary>
        /// <param name="mods">A list of mods to depend on.</param>
        public DependAttribute(params Type[] mods)
        {
            Mods = mods;
        }
    }
}
