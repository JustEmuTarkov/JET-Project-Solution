using System;
using System.Threading;

namespace SimpleLauncher
{
    class Core
    {
        /// <summary>
        /// Main application Loop
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            i_Main.Loop();
        }
        internal static Game Game;
        internal static Request Request;
        internal static MainInstance i_Main;
        internal static CMD CMD;
        internal static Settings Config = new Settings();
    }
}
