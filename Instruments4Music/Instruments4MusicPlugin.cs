using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Instruments4Music
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("Lethal Company.exe")]
    public class Instruments4MusicPlugin : BaseUnityPlugin
    {
        public static Instruments4MusicPlugin instance;

        internal static ManualLogSource? logger;
        public static ConfigFile? config;
        public static InstrumentsConfig configInstance;

        internal static InputActions inputActionsInstance = new();
        public AssetBundle assets;

        // patch game
        private readonly Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);

        public GameObject hudPrefab;
        public GameObject hudInstance;
        public HUDManager hudManager;
        public HUDElement hudElement;

        public float hudScale = 0.45f;

        public static void AddLog(string str)
        {
            logger?.LogInfo(str);
        }

        private void Awake()
        {
            if (instance != null)
            {
                throw new System.Exception("More than 1 plugin instance.");
            }
            instance = this;

            logger = this.Logger;

            config = this.Config;

            InstrumentsConfig.Load();

            // Plugin startup logic
            AddLog($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            // load hud
            assets = AssetUtils.LoadAssetBundleFromResources("musichud", typeof(HUDPatches).Assembly);
            hudPrefab = assets.LoadAsset<GameObject>("MusicPanel");

            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPatch(ref StartOfRound __instance)
        {

            var obj = __instance.mapScreen?.mesh?.gameObject;
            if (obj == null) return;
            if (obj.GetComponent<StationaryScript>() == null)
            {
                obj.AddComponent<StationaryScript>();
                Instruments4MusicPlugin.AddLog("StationaryScript Added");
            }

            if (obj.GetComponent<PortableScript>() == null)
            {
                obj.AddComponent<PortableScript>();
                Instruments4MusicPlugin.AddLog("PortableScript Added");
            }

            if (obj.GetComponent<TuneAudioScript>() == null)
            {
                obj.AddComponent<TuneAudioScript>();
                Instruments4MusicPlugin.AddLog("TuneAudioScript Added");
            }

            if (obj.GetComponent<MusicHUD>() == null)
            {
                obj.AddComponent<MusicHUD>();
                Instruments4MusicPlugin.AddLog("MusicHUD Added");
            }
        }
    }

    [HarmonyPatch(typeof(HUDManager))]
    public class HUDPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        static void Awake_Postfix(HUDManager __instance)
        {
            Instruments4MusicPlugin.instance.hudManager = __instance;

            var elements = __instance.GetPrivateField<HUDElement[]>("HUDElements");

            var HUD = Object.Instantiate(Instruments4MusicPlugin.instance.hudPrefab, elements[0].canvasGroup.transform.parent);
            Instruments4MusicPlugin.instance.hudInstance = HUD;
            HUD.transform.localScale = new Vector3(1f, 1f, 1f) * InstrumentsConfig.ConfigHUDScale.Value;
            Instruments4MusicPlugin.AddLog("MusicHUD Instantiated");

            HUDElement newElement = new();
            Instruments4MusicPlugin.instance.hudElement = newElement;
            var canvasGroup = HUD.GetComponent<CanvasGroup>() ?? HUD.AddComponent<CanvasGroup>();
            newElement.canvasGroup = canvasGroup;
            __instance.PingHUDElement(newElement, 0f, 0f, 0f);

            List<HUDElement> elementList = [..elements, newElement];
            elements = [.. elementList];

            __instance.GetType().GetField("HUDElements", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(__instance, elements);
        }
    }
}