using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLauncher
{
    class Constants
    {
        internal static class URL
        {
            internal static string ServerConnect = "/launcher/server/connect";
            internal static string ProfileLogin = "/launcher/profile/login";
            internal static string ProfileRegister = "/launcher/profile/register";
            internal static string ProfileRemove = "/launcher/profile/remove";
            internal static string ProfileGet = "/launcher/profile/get";
            internal static string ProfileChangeEmail = "/launcher/profile/change/email";
            internal static string ProfileChangePassword = "/launcher/profile/change/password";
            internal static string ProfileChangeWipe = "/launcher/profile/change/wipe";
        }
    }
}
