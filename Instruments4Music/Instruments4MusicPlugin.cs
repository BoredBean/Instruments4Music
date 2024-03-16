using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;

namespace Instruments4Music
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("Lethal Company.exe")]
    public class Instruments4MusicPlugin : BaseUnityPlugin
    {
        public static Instruments4MusicPlugin? Instance;

        internal static ManualLogSource? logger;
        public static ConfigFile? config;

        internal static InputActions InputActionsInstance = new();
        public AssetBundle? Assets;

        // patch game
        private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

        public GameObject? HudPrefab;
        public GameObject? HudInstance;
        public HUDManager? HudManager;
        public HUDElement? HudElement;
        public AudioMixer? TuneMixer;

        public static void AddLog(string str)
        {
            logger?.LogInfo(str);
        }

        private void Awake()
        {
            if (Instance != null)
            {
                throw new System.Exception("More than 1 plugin instance.");
            }
            Instance = this;

            logger = Logger;

            config = Config;

            InstrumentsConfig.Load();

            // Plugin startup logic
            AddLog($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            // load hud
            Assets = AssetUtils.LoadAssetBundleFromResources("instrumentassets", typeof(Instruments4MusicPlugin).Assembly);
            HudPrefab = Assets.LoadAsset<GameObject>("MusicPanel");
            TuneMixer = Assets.LoadAsset<AudioMixer>("Tuner");

            _harmony.PatchAll(Assembly.GetExecutingAssembly());
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

            if (obj.GetComponent<MusicHud>() == null)
            {
                obj.AddComponent<MusicHud>();
                Instruments4MusicPlugin.AddLog("MusicHUD Added");
            }
        }
    }

    [HarmonyPatch(typeof(HUDManager))]
    public class HudPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        static void Awake_Postfix(HUDManager __instance)
        {
            Instruments4MusicPlugin.Instance.HudManager = __instance;

            var elements = __instance.GetPrivateField<HUDElement[]>("HUDElements");

            var hud = Object.Instantiate(Instruments4MusicPlugin.Instance.HudPrefab,
                elements[0].canvasGroup.transform.parent);
            Instruments4MusicPlugin.Instance.HudInstance = hud;
            if (hud == null) return;

            hud.transform.localScale = new Vector3(1f, 1f, 1f) * (InstrumentsConfig.ConfigHudScale?.Value != null
                ? InstrumentsConfig.ConfigHudScale.Value
                : 1.0f);
            Instruments4MusicPlugin.AddLog("MusicHUD Instantiated");

            HUDElement newElement = new();
            Instruments4MusicPlugin.Instance.HudElement = newElement;
            var canvasGroup = hud.GetComponent<CanvasGroup>() ?? hud.AddComponent<CanvasGroup>();
            newElement.canvasGroup = canvasGroup;
            __instance.PingHUDElement(newElement, 0f, 0f, 0f);

            List<HUDElement> elementList = [.. elements, newElement];
            elements = [.. elementList];

            __instance.GetType().GetField("HUDElements", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(__instance, elements);
        }
    }
}