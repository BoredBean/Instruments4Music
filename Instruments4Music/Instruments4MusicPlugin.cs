using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Instruments4Music
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("Lethal Company.exe")]
    public class Instruments4MusicPlugin : BaseUnityPlugin
    {
        internal static ManualLogSource LOGGER;
        public static ConfigFile config;
        public static Sprite HOVER_ICON { get; private set; }
        public static ConfigEntry<bool> CONFIG_SHOW_TOOLTIP { get; private set; }

        internal static InstrumentInputActions InputActionsInstance = new InstrumentInputActions();
        public void AddLog(string str)
        {
            Logger.LogInfo(str);
        }

        private void Awake()
        {
            Instruments4MusicPlugin.LOGGER = this.Logger;

            SceneManager.sceneLoaded += OnSceneLoaded;
            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameObject obj = StartOfRound.Instance?.mapScreen?.mesh.gameObject;
            if (obj != null && obj.GetComponent<StationaryInstrumentScript>() == null)
            {
                obj.AddComponent<StationaryInstrumentScript>();
            }
            if (obj != null && obj.GetComponent<TuneAudioScript>() == null)
            {
                obj.AddComponent<TuneAudioScript>();
            }
        }
    }
}
