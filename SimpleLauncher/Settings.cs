using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SimpleLauncher
{
    internal class Settings
    {
        private string m_fileName = "settings.json";
        public SettingsFile LoadedConfig = new SettingsFile();
        internal Settings()
        {
            if (File.Exists(m_fileName))
            {
                string settingsContent = File.ReadAllText(m_fileName);
                LoadedConfig = JsonSerializer.Deserialize<SettingsFile>(settingsContent);
                if (LoadedConfig.DefaultServer != "") 
                {
                    Core.Request = new Request(LoadedConfig.DefaultServer);
                }
            }
            else 
            {
                File.WriteAllText(m_fileName, JsonSerializer.Serialize(LoadedConfig));
            }
        }

        public class SettingsFile
        {
            public string DefaultServer = "";
            public bool AutoUseDefaultServer = false;
            public bool SaveUserLoginData = false;
            public LoginInfo LoginData = new LoginInfo();
            public List<string> SavedServers = new List<string>();

            public class LoginInfo 
            {
                public string Login = "";
                public string Password = "";
            }
        }
    }
}
