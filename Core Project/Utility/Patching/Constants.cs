using FilesChecker;
using JET.Utility.Reflection;
using System;
using System.Linq;
using System.Reflection;

namespace JET.Utility.Patching
{
    public static class Constants
    {
        #region Binding flags
        // if you find any better naming for those variables jsut rename it using VS
        public const BindingFlags DefaultBindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        public const BindingFlags PublicInstanceFlag = BindingFlags.Public | BindingFlags.Instance;
        public const BindingFlags NonPublicInstanceFlag = BindingFlags.NonPublic | BindingFlags.Instance;
        public const BindingFlags NonPublicInstanceDeclaredOnlyFlag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        public const BindingFlags NonPublicFlag = BindingFlags.NonPublic;
        #endregion

        public static Assembly TargetAssembly = typeof(ActionTrigger).Assembly;
        public static Type[] TargetAssemblyTypes = TargetAssembly.GetTypes();
        public static Assembly FileCheckerAssembly = typeof(ICheckResult).Assembly;
        public static Type[] FileCheckerAssemblyTypes = FileCheckerAssembly.GetTypes();

        public static Type MainApplicationType = typeof(EFT.MainApplication);// TargetAssemblyTypes.Single(x => x.Name == "MainApplication");
        public static Type LocalGameType = TargetAssemblyTypes.Single(x => x.Name == "LocalGame");
        public static Type ExfilPointManagerType = TargetAssemblyTypes.Single(x => x.GetMethod("InitAllExfiltrationPoints") != null);
        public static Type UnityCertificateHandlerType = typeof(UnityEngine.Networking.CertificateHandler);
        public static Type UnityUnityWebRequestType = typeof(UnityEngine.Networking.UnityWebRequestTexture);
        //public static Type PreloaderUIType = TargetAssembly.GetTypes().Single(x => x.Name == "PreloaderUI");
        //public static Type BaseLocalGameType = TargetAssembly.GetTypes().Single(x => x.Name.ToLower().Contains("baselocalgame"));
        //public static Type MatchmakerOfflineRaidType = TargetAssembly.GetTypes().Single(x => x.Name == "MatchmakerOfflineRaid");
        //public static Type MenuControllerType = TargetAssembly.GetTypes().Single(x => x.GetProperty("QuestController") != null);

        public static Type MenuControllerType = TargetAssemblyTypes.Single(x => x.GetProperty("QuestController") != null);

        public static Type BackendInterfaceType = Constants.TargetAssemblyTypes.Single(
            x => x.GetMethods().Select(y => y.Name).Contains("CreateClientSession") && x.IsInterface);
        public static Type SessionInterfaceType = Constants.TargetAssemblyTypes.Single(
            x => x.GetMethods().Select(y => y.Name).Contains("GetPhpSessionId") && x.IsInterface);

        public static Type ProfileInfoType = Constants.TargetAssemblyTypes.Single(x => x.GetMethod("GetExperience") != null);
        public static Type ProfileType = Constants.TargetAssemblyTypes.Single(x => x.GetMethod("AddToCarriedQuestItems") != null);
        public static Type FenceTraderInfoType = Constants.TargetAssemblyTypes.Single(x => x.GetMethod("NewExfiltrationPrice") != null);

        public static Type FirearmControllerType = typeof(EFT.Player.FirearmController).GetNestedTypes().Single(x => x.GetFields(DefaultBindingFlags).
            Count(y => y.Name.Contains("gclass")) > 0 && x.GetFields(DefaultBindingFlags).Count(y => y.Name.Contains("callback")) > 0 && x.GetMethod("UseSecondMagForReload", DefaultBindingFlags) != null);
        public static string WeaponControllerFieldName = FirearmControllerType.GetFields(DefaultBindingFlags).
                Single(x => x.Name.Contains("gclass")).Name;




        public static ConstructorInfo[] MainApplicationConstructorInfo {
            get 
            {
                return MainApplicationType.GetConstructors();
            }
        }
    }
}
