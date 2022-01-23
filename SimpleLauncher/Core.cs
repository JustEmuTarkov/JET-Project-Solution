using System;
using System.Threading;

namespace SimpleLauncher
{
    public class Core
    {
        /// <summary>
        /// Main application Loop
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            i_Main.Loop();
        }
        internal static Game Game = new Game();
        internal static Request Request = new Request();
        internal static MainInstance i_Main = new MainInstance();
        internal static CMD CMD = new CMD();
        internal static Settings Config = new Settings();
    }
}
