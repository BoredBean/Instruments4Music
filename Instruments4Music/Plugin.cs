using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using LethalCompanyInputUtils.Api;
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
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

        internal static InstrInputClass InputActionsInstance = new InstrInputClass();
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
            if (obj != null && obj.GetComponent<StationaryScript>() == null)
            {
                obj.AddComponent<StationaryScript>();
            }
        }
    }

    //public class PlayInstrumentBehavior
    //{
    //    public void Update()
    //    {
    //        DoSomething();
    //    }

    //    public void DoSomething()
    //    {
    //        if (Instruments4MusicPlugin.InputActionsInstance.HighCKey.triggered) return;

    //        //Play HighC
    //    }
    //}

    //public class StartConcertBehavior
    //{
    //    public void Awake()
    //    {
    //        SetupKeybindCallbacks();
    //    }

    //    public void SetupKeybindCallbacks()
    //    {
    //        Instruments4MusicPlugin.InputActionsInstance.Showtime.performed += ItsShowTime;
    //        Instruments4MusicPlugin.InputActionsInstance.CurtainCall.performed += AnswerCurtainCall;
    //    }

    //    public void ItsShowTime(InputAction.CallbackContext instrumentConext)
    //    {
    //        if (!instrumentConext.performed) return;
    //        // Add more context checks if desired

    //        // Your executing code here
    //    }

    //    public void AnswerCurtainCall(InputAction.CallbackContext instrumentConext)
    //    {
    //        if (!instrumentConext.performed) return;
    //        // Add more context checks if desired

    //        // Your executing code here
    //    }
    //}
}
