using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JET.Utility.Modding
{
    internal class ModSettings
    {
        internal string ModName { get; }
        internal string ModAuthor { get; }
        internal string ModVersion { get; }
        internal string[] CompatibleWith { get; }
        internal Type[] DependsOn { get; }
        internal Type[] SoftDependsOn { get; }
        internal Type ModType { get; }

        public ModSettings(Type modType)
        {
            ModType = modType;
            var assembly = ModType.Assembly;
            var modInfoAttribute =
                assembly.GetCustomAttributes(typeof(ModInfoAttribute), false).FirstOrDefault() as ModInfoAttribute;
            var compatibleWithAttribute =
                assembly.GetCustomAttributes(typeof(CompatibleWithAttribute), false).FirstOrDefault() as
                    CompatibleWithAttribute;
            var dependAttribute =
                assembly.GetCustomAttributes(typeof(DependAttribute), false).FirstOrDefault() as DependAttribute;
            var softDependAttribute =
                assembly.GetCustomAttributes(typeof(SoftDependAttribute), false)
                    .FirstOrDefault() as SoftDependAttribute;

            ModName = modInfoAttribute?.ModName;
            ModAuthor = modInfoAttribute?.Author;
            ModVersion = modInfoAttribute?.Version;
            CompatibleWith = compatibleWithAttribute?.GameVersions ?? new string[] { };
            DependsOn = dependAttribute?.Mods ?? new Type[] { };
            SoftDependsOn = softDependAttribute?.Mods ?? new Type[] { };
        }
    }
}
