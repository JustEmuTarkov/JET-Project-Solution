using JET.Utility;
using JET.Utility.Logger;
using System.Linq;

namespace JET.Mono
{
    using UnityEngine; // << using is here because namespace is the same and messing with monobehaviour

    public class JET_Instance : MonoBehaviour
    {

        public static GameObject Instance;
        #region OnApplicationQuit Event
        public delegate void Void();
        public static event Void ApplicationQuitEvent;
        public void OnApplicationQuit() => ApplicationQuitEvent?.Invoke();
        #endregion
        /// <summary>
        /// Awake method that is called on Mono Start before any other methods
        /// </summary>
        private void Awake()
        {
            IngameLogger.CheckAndSet();
            Validator.IsGameFound();
            if (Validator.isFullLoggerEnabled)
            {
                PatchRunner.ExecuteLoggerPatches();
            }
            PatchRunner.ExecuteCorePatches();
        }
        /// <summary>
        /// Start method that is called on Mono Start after Awake method
        /// </summary>
        private void Start()
        {
            IngameLogger.CheckAndSet();
            PerformWatermarking();
            // load mods from CustomMods folder
            new ModsLoader();
            IngameLogger.CheckAndSet();
        }
        /// <summary>
        /// LateUpdate method that is called in ticks after all Update methods are called
        /// </summary>
        private void LateUpdate()
        {
            PerformWatermarking();
        }
        /// <summary>
        /// Override the BetaVersion Text in left bottom corner of the screen
        /// </summary>
        void PerformWatermarking()
        {
            if (ClientAccesor.BetaVersionLabel != null)
            {
                //ClientAccesor.BetaVersionLabel.LocalizationKey = ClientAccesor.GameVersion.Split('.').Last() + " | JET" + Validator.OhWellFuck;
                // Lets get it almost like LIVE? Aki does this, so why don't we? 
                ClientAccesor.BetaVersionLabel.LocalizationKey = string.Empty;

            }
        }
    }
}
