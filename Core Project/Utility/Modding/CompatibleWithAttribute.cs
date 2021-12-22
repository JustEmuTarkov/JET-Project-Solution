using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JET.Utility.Modding
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class CompatibleWithAttribute : Attribute
    {
        internal string[] GameVersions { get; }

        /// <summary>
        /// Specifies which game versions your mod is compatible with.
        /// </summary>
        /// <param name="gameVersions">A list of game versions. Leave blank to load the mod no matter the version.</param>
        public CompatibleWithAttribute(params string[] gameVersions)
        {
            GameVersions = gameVersions;
        }
    }
}
