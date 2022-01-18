using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLauncher
{
    internal class JsonStructures
    {
        internal class AccountInfo
        {
            public string id;
            public string email;
            public string password;
            public bool wipe;

            public AccountInfo()
            {
                id = "";
                email = "";
                password = "";
                wipe = false;
            }
        }
        internal struct Login
        {
            public string email;
            public string password;

            public Login(string email, string password)
            {
                this.email = email;
                this.password = password;
            }
        }
        internal struct Register
        {
            public string email;
            public string password;
            public string edition;

            public Register(string email, string password, string edition)
            {
                this.email = email;
                this.password = password;
                this.edition = edition;
            }
        }
        internal struct Change
        {
            public string email;
            public string password;
            public string change;

            public Change(string email, string password, string change)
            {
                this.email = email;
                this.password = password;
                this.change = change;
            }
        }
        internal class ServerInfo
        {
            public string connectUrl;
            public string backendUrl;
            public string name;
            public string[] editions;

            public ServerInfo()
            {
                connectUrl = backendUrl = "https://127.0.0.1";
                name = "Local JET Server";
                editions = new string[0];
            }
        }
    }
}
