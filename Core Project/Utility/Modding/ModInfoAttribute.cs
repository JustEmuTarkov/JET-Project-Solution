using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JET.Utility.Modding
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ModInfoAttribute : Attribute
    {
        internal string ModName { get; } = null;
        internal string Author { get; } = null;
        internal string Version { get; } = null;

        /// <summary>
        /// Basic information about your mod.
        /// </summary>
        /// <param name="modName">The name of the mod.</param>
        /// <param name="author">The author of the mod.</param>
        /// <param name="version">The current version of your mod.</param>
        public ModInfoAttribute(string modName, string author, string version)
        {
            ModName = modName;
            Author = author;
            Version = version;
        }
    }
}
