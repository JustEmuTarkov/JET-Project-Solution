using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SimpleLauncher
{

    internal class MainInstance
    {
        internal bool Stop = false;
        internal JsonStructures.ServerInfo ConnectedServer = new JsonStructures.ServerInfo();
        internal JsonStructures.AccountInfo ConnectedAccount = new JsonStructures.AccountInfo();

        private MenuOption m_PrevAppState = MenuOption.WelcomeMessage;
        private MenuOption m_AppState = MenuOption.WelcomeMessage;
        private MenuOption m_MenuDefault = MenuOption.Menu;
        internal MenuOption GetAppState
        {
            get
            {
                return m_AppState;
            }
        }
        internal MenuOption GetPrevAppState
        {
            get
            {
                return m_PrevAppState;
            }
        }
        private void SetPrevAppState() 
        {
            SetAppState(m_PrevAppState);
        }
        private void SetAppState(MenuOption state)
        {
            m_PrevAppState = m_AppState;
            m_AppState = state;
        }
        private void SetMenuDefault(MenuOption state)
        {
            m_MenuDefault = state;
        }
        private bool SkipWelcomeMsg {
            get 
            {
                return false;
            }
        }
        internal enum MenuOption {
            WelcomeMessage = 1000,
            Menu = 1001,
            ConnectToServer = 0,
            CreateAccount = 1,
            Login = 2,
            StartGame = 3,
            EditAccount = 4,
            Options = 9,
            AddServer = 10,
            RemoveServer = 11,
            MoreInfo = 12
        }

        Dictionary<MenuOption, Dictionary<int, string>> MenuList = new Dictionary<MenuOption, Dictionary<int, string>>() {
            { MenuOption.Menu, new Dictionary<int, string>()
                {
                    { 0, "Connect To Server" },
                    { 9, "Options" },
                }
            },
            { MenuOption.ConnectToServer, new Dictionary<int, string>()
                {
                    { 1, "Create Account" },
                    { 2, "Login" },
                    { 9, "Options" },
                }
            },
            { MenuOption.Login, new Dictionary<int, string>()
                {
                    { 3, "Start Game" },
                    { 4, "Edit Account" },
                    { 9, "Options" },
                }
            },
            { MenuOption.Options, new Dictionary<int, string>()
                {
                    { 10, "Add Server" },
                    { 11, "Remove Server" },
                    { 12, "More Information" },
                }
            },
        };

        private void CreateMenuVariables() {
        }
        internal void Loop()
        {
            while (!Stop)
            {
                DrawMenuLoop();
                Thread.Sleep(25);
            }
            Core.CMD.WaitForInput("Program will now close press any key to continue...\n");
        }
        private void DrawMenuLoop()
        {
            switch (m_AppState) {
                case MenuOption.WelcomeMessage:
                    cMenu.WelcomeMessage(); break;
                case MenuOption.Menu:
                    cMenu.Menu(); break;
                case MenuOption.ConnectToServer:
                    cMenu.ConnectToServer(); break;
                case MenuOption.CreateAccount:
                    cMenu.CreateAccount(); break;
                case MenuOption.Login:
                    cMenu.Login(); break;
                case MenuOption.StartGame:
                    cMenu.StartGame(); break;
                case MenuOption.Options:
                    cMenu.Options(); break;
            }
        }

        /// <summary>
        /// Menu drawers for Console
        /// </summary>
        internal static class cMenu
        {
            internal static Dictionary<int, string> CurrentMenuList 
            {
                get 
                {
                    return Core.i_Main.MenuList.GetValueOrDefault(Core.i_Main.GetAppState);
                }
            }
            private static void DrawCurrentMenu() 
            {
                foreach (var menuOption in CurrentMenuList)
                {
                    Core.CMD.Write(menuOption.Key, $"- {menuOption.Value}", true, ">");
                }
                if (Core.i_Main.GetAppState >= MenuOption.Options && Core.i_Main.GetAppState < MenuOption.WelcomeMessage)
                {
                    Core.CMD.Write("b - Go Back", true, ">");
                }
                Core.CMD.Write("q - Close Application", true, ">");

                Core.CMD.Write("[DEBUG] Start", true, "@");
                Core.CMD.Write($"{Core.i_Main.GetAppState.ToString()}", true, "@");
                Core.CMD.Write($"{Core.i_Main.GetPrevAppState.ToString()}", true, "@");
                Core.CMD.Write("[DEBUG] End", true, "@");

            }
            private static void ProcessOptionChoosed()
            {
                Core.CMD.Write("Choose an option (and press Enter)...", true, "");
                string pressedKey = Console.ReadLine();
                int optionChoosed = -1;
                if (!int.TryParse(pressedKey, out optionChoosed))
                {
                    if (pressedKey.ToLower() == "b")
                    {
                        Core.i_Main.SetPrevAppState();
                    }
                    if (pressedKey.ToLower() == "q")
                    {
                        Core.i_Main.Stop = true;
                    }
                    return;
                }
                if (optionChoosed >= 0 && optionChoosed <= 9)
                {
                    Core.i_Main.SetAppState((MenuOption)optionChoosed);
                }
            }
            internal static void WelcomeMessage()
            {
                if (!Core.i_Main.SkipWelcomeMsg)
                {
                    Core.CMD.Write("- JustEmuTarkov Simple Launcher -");
                    Core.CMD.Write("Created by TheMaoci");

                    Thread.Sleep(500);
                    //Core.CMD.WaitForInput("Press any key to continue...");
                }
                Core.i_Main.SetAppState(MenuOption.Menu);
                Core.CMD.Clear();
            }
            internal static void Menu()
            {
                // Menu Drawing
                DrawCurrentMenu();
                ProcessOptionChoosed();
                Core.CMD.Clear();
            }
            internal static void ConnectToServer()
            {
                if (Core.Config.LoadedConfig.DefaultServer == "") {
                    return;
                }
                var ServerInfo = Core.Request.SendJson(Constants.URL.ServerConnect).JsonCastTo<JsonStructures.ServerInfo>();
                if (ServerInfo != default(JsonStructures.ServerInfo))
                {
                    Core.i_Main.ConnectedServer = ServerInfo;
                    Core.i_Main.SetAppState(MenuOption.ConnectToServer);
                    Core.i_Main.SetMenuDefault(MenuOption.ConnectToServer);
                    Core.CMD.Write($"Connected to: {Core.i_Main.ConnectedServer.name}");
                    DrawCurrentMenu();
                    ProcessOptionChoosed();
                    Core.CMD.Clear();
                }
            }
            internal static void Options()
            {
                DrawCurrentMenu();
                ProcessOptionChoosed();
                Core.CMD.Clear();
            }
            internal static void CreateAccount()
            {
                Core.CMD.Write("Type your Login:");
                string login = Console.ReadLine();
                Core.CMD.Clear();
                Core.CMD.Write("Type your Password (Not Required)");
                string password = Console.ReadLine();
                SelectGameEditionAgain:
                Core.CMD.Clear();
                Core.CMD.Write("Select Game Edition");
                int counter = 1;
                foreach (var _edition in Core.i_Main.ConnectedServer.editions) 
                {
                    Core.CMD.Write($"{counter++} -> {_edition}");
                }

                int edition = int.Parse(Console.ReadKey().KeyChar.ToString());

                if (edition > 0 && edition <= counter)
                {
                    JsonStructures.Register body = new JsonStructures.Register(login, password, Core.i_Main.ConnectedServer.editions[edition-1]);

                    string responseData = Core.Request.SendJson(body.ToString<JsonStructures.Register>(), Constants.URL.ProfileRegister);
                    if (responseData == "OK")
                    {
                        Core.CMD.Clear();
                        Core.CMD.Write("[SUCCESS] Account created");
                        Thread.Sleep(250);
                        Core.i_Main.SetAppState(Core.i_Main.GetPrevAppState);
                    }
                    else 
                    {
                        Core.CMD.Clear();
                        Core.CMD.Write("[ERROR] Account with such login already exists");

                    }
                }
                else 
                {
                    goto SelectGameEditionAgain;
                }
                Core.CMD.Clear();
            }
            internal static void Login()
            {
                TryAgain:
                Core.CMD.Write("Type your Login:");
                string login = Console.ReadLine();
                Core.CMD.Clear();
                Core.CMD.Write("Type your Password");
                string password = Console.ReadLine();
                Core.CMD.Clear();
                JsonStructures.Login body = new JsonStructures.Login(login, password);
                string responseData = Core.Request.SendJson(body.ToString<JsonStructures.Login>(), Constants.URL.ProfileLogin);

                if (responseData == "FAILED")
                {
                    Core.CMD.Clear();
                    Core.CMD.Write("[ERROR] Login or Password are incorrect");
                    goto TryAgain;
                }
                else 
                {
                    try
                    {
                        responseData = Core.Request.SendJson(body.ToString<JsonStructures.Login>(), Constants.URL.ProfileGet);
                        Core.i_Main.ConnectedAccount = responseData.JsonCastTo<JsonStructures.AccountInfo>();
                        Core.i_Main.SetAppState(MenuOption.Login);
                        Core.i_Main.SetMenuDefault(MenuOption.Login);
                        Core.CMD.Clear();
                    }
                    catch (Exception e) 
                    {
                        Core.CMD.Clear();
                        Core.CMD.Write("[ERROR] Unexpected error occured while parsing login data");
                        Core.CMD.Write(e.Message);
                        Core.CMD.Write(e.StackTrace);
                        goto TryAgain;
                    }
                }
            }
            internal static void StartGame()
            {
                string errorMsg = Core.Game.LaunchGame();
                if (errorMsg == "OK")
                {
                    Core.i_Main.Stop = true;
                }
                else 
                {
                    Core.CMD.Write(errorMsg, true, "[ERROR]");
                }

            }
        }
    }
}
