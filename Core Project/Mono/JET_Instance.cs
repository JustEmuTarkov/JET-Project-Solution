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

        private void Awake()
        {
            IngameLogger.CheckAndSet();
            Validator.IsGameFound();
            PatchRunner.ExecuteCorePatches();
        }
        private void Start()
        {
            IngameLogger.CheckAndSet();
            if (Validator.isFullLoggerEnabled) {
                PatchRunner.ExecuteLoggerPatches();
            }
            PerformWatermarking();
            // load mods from CustomMods folder
            new ModsLoader();
            IngameLogger.CheckAndSet();
        }
        private void LateUpdate()
        {
            PerformWatermarking();
        }

        void PerformWatermarking() {
            if (ClientAccesor.BetaVersionLabel != null)
            {
                ClientAccesor.BetaVersionLabel.LocalizationKey = ClientAccesor.GameVersion.Split('.').Last() + " | JET" + Validator.OhWellFuck;
            }
        }
    }
}
