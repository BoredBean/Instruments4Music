using GameNetcodeStuff;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Instruments4Music
{
    public class TuneAudioScript : MonoBehaviour
    {
        private const double TuneCoeff = 1.059463f;
        static float lastTimer = 3f;
        static float attenuationCoeff = 2f;
        static float softModifier = 0.5f;
        static float sustainModifier = 4.0f;

        static Dictionary<string, (AudioClip, int, bool)> InstrDictionary = [];
        static Dictionary<(string, int), AudioSource> TunedDictionary = [];

        static ConcurrentDictionary<(string, int), float> TimerDictionary =
            new();

        public static bool theShowIsOn = false;
        public static bool secondaryKeyBind = false;
        public static bool keyBindInit = false;
        public static string activeClipName = "";
        public static GameObject? activeInstrObject;

        private static PlayerControllerB? Player => GameNetworkManager.Instance?.localPlayerController;

        public static void RegisterInstrClip(GameObject gameObject, int sourceNoteNumber, AudioClip clip, bool isLoop)
        {
            foreach (var tup in TunedDictionary)
            {
                tup.Value?.Stop();
            }
            TunedDictionary.Clear();
            Instruments4MusicPlugin.AddLog($"TunedDictionary clear.");

            InstrDictionary[clip.name] = (clip, sourceNoteNumber, isLoop);

            activeClipName = clip.name;
            activeInstrObject = gameObject;
            theShowIsOn = true;
            OnDisable();
            if (Player != null) Player.inTerminalMenu = true;
            MusicHUD.ShowUserInterface();
            Instruments4MusicPlugin.AddLog($"Playing {activeClipName}.");
        }

        public static void DeActiveInstrument()
        {
            theShowIsOn = false;
            OnEnable();
            if (Player != null) Player.inTerminalMenu = false;
            MusicHUD.HideUserInterface();
            Instruments4MusicPlugin.AddLog($"DeActive {activeClipName}.");
        }

        /* NoteNumber: 0~35 corresponding to the Note names of 3 octaves */
        public static void PlayTunedAudio(int targetNoteNumber, bool isSoft, bool newPulse)
        {
            Instruments4MusicPlugin.AddLog($"Playing note {targetNoteNumber}, softer: {isSoft}.");
            MusicHUD.instance.OnButtonClicked(targetNoteNumber);
            TimerDictionary[(activeClipName, targetNoteNumber)] = lastTimer;
            var volume = 1.0f;
            if (isSoft)
            {
                volume -= softModifier;
            }

            if (TunedDictionary.TryGetValue((activeClipName, targetNoteNumber), out var audioSource))
            {
                audioSource.volume = volume;
                if (newPulse)
                {
                    audioSource.Play();
                }

                return;
            }

            if (!InstrDictionary.TryGetValue(activeClipName, out var value)) return;

            var clip = value.Item1;
            var sourceNoteNumber = value.Item2;
            var isLoop = value.Item3;
            var power = targetNoteNumber - sourceNoteNumber;

            audioSource = activeInstrObject?.AddComponent<AudioSource>();
            if (audioSource == null) return;
            audioSource.clip = clip;
            audioSource.pitch = (float)Math.Pow(TuneCoeff, power);
            audioSource.volume = volume;
            audioSource.loop = isLoop;
            TunedDictionary.Add((activeClipName, targetNoteNumber), audioSource);
            audioSource.Play();
        }

        public static void AudioCountDown(bool isSustain)
        {
            foreach (var timerDictionaryKey in TimerDictionary.Keys)
            {
                var timeLast = TimerDictionary[timerDictionaryKey];
                if (TunedDictionary.TryGetValue(timerDictionaryKey, out var audioSource) && timeLast >= 0.0f &&
                    audioSource.volume > 0.01f)
                {
                    if (isSustain)
                    {
                        TimerDictionary[timerDictionaryKey] = timeLast - Time.deltaTime / sustainModifier;
                        audioSource.volume = (float)(audioSource.volume * Math.Exp(-attenuationCoeff * Time.deltaTime / sustainModifier));
                    }
                    else
                    {
                        TimerDictionary[timerDictionaryKey] = timeLast - Time.deltaTime;
                        audioSource.volume = (float)(audioSource.volume * Math.Exp(-attenuationCoeff * Time.deltaTime));
                    }
                }
                else
                {
                    audioSource?.Stop();
                    TimerDictionary.Remove(timerDictionaryKey, out _);
                }
            }
        }

        private static void OnEnable()
        {
            try
            {
                Player.playerActions.Movement.Enable();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)$"Error while subscribing to input in PlayerController!: {(object)ex}");
            }
        }

        private static void OnDisable()
        {
            try
            {
                Player.playerActions.Movement.Enable();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)$"Error while unsubscribing from input in PlayerController!: {(object)ex}");
            }
        }

        public void Awake()
        {

            InstrDictionary = [];
            TunedDictionary = [];

            TimerDictionary = new();

            theShowIsOn = false;
            secondaryKeyBind = false;
            activeClipName = "";

            if (keyBindInit) return;
            SetupKeyBindCallbacks();
            keyBindInit = !keyBindInit;
        }

        public void SetupKeyBindCallbacks()
        {
            Instruments4MusicPlugin.inputActionsInstance.Showtime.performed += OnShowtimePressed;
            Instruments4MusicPlugin.inputActionsInstance.CurtainCall.performed += OnCurtainCallPressed;
            Instruments4MusicPlugin.inputActionsInstance.ChangeMode.performed += OnChangeModePressed;
            Instruments4MusicPlugin.inputActionsInstance.InputNote.performed += OnInputNotePressed;
        }

        public void OnShowtimePressed(InputAction.CallbackContext showtimeContext)
        {
            if (theShowIsOn) return;
            if (!showtimeContext.performed) return;
            if (StationaryScript.IsLookingAtInstrument(out var instrumentObj))
            {
                if (instrumentObj == null) return;
                Instruments4MusicPlugin.AddLog("It's show time!");
                StationaryScript.LetShowBegins(instrumentObj);
            }
            else if (PortableScript.IsHoldingInstrument(out instrumentObj))
            {
                if (instrumentObj == null) return;
                Instruments4MusicPlugin.AddLog("It's show time!");
                PortableScript.LetShowBegins(instrumentObj);
            }
        }

        public void OnCurtainCallPressed(InputAction.CallbackContext curtainCallContext)
        {
            if (!theShowIsOn) return;
            if (!curtainCallContext.performed) return;

            Instruments4MusicPlugin.AddLog("Maybe next time.");
            DeActiveInstrument();
        }

        public void OnChangeModePressed(InputAction.CallbackContext changeModeContext)
        {
            if (!theShowIsOn) return;
            if (!changeModeContext.performed) return;

            Instruments4MusicPlugin.AddLog("Change key bind mode.");
            secondaryKeyBind = !secondaryKeyBind;
            MusicHUD.UpdateButtonTips();
        }

        public void OnInputNotePressed(InputAction.CallbackContext inputNoteContext)
        {
            if (!theShowIsOn) return;
            if (!inputNoteContext.performed) return;

            Instruments4MusicPlugin.AddLog("Input music note.");
            MusicHUD.instance.TriggerInputNote();
        }

        public void Update()
        {
            var isSustain = Instruments4MusicPlugin.inputActionsInstance.Sustain.ReadValue<float>() > 0.5f;
            AudioCountDown(isSustain);

            if (!theShowIsOn) return;
            var semiTone = 0;
            var isSoft = Instruments4MusicPlugin.inputActionsInstance.Soft.ReadValue<float>() > 0.5f;
            if (Instruments4MusicPlugin.inputActionsInstance.Semitone.ReadValue<float>() > 0.5f && !secondaryKeyBind)
            {
                semiTone = 1;
            }

            var key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.LowCKey : Instruments4MusicPlugin.inputActionsInstance.LowCKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(0 + semiTone, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.LowDKey : Instruments4MusicPlugin.inputActionsInstance.LowDKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(2 + semiTone, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.LowEKey : Instruments4MusicPlugin.inputActionsInstance.LowEKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(4, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.LowFKey : Instruments4MusicPlugin.inputActionsInstance.LowFKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(5 + semiTone, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.LowGKey : Instruments4MusicPlugin.inputActionsInstance.LowGKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(7 + semiTone, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.LowAKey : Instruments4MusicPlugin.inputActionsInstance.LowAKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(9 + semiTone, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.LowBKey : Instruments4MusicPlugin.inputActionsInstance.LowBKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(11, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.MidCKey : Instruments4MusicPlugin.inputActionsInstance.MidCKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(12 + semiTone, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.MidDKey : Instruments4MusicPlugin.inputActionsInstance.MidDKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(14 + semiTone, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.MidEKey : Instruments4MusicPlugin.inputActionsInstance.MidEKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(16, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.MidFKey : Instruments4MusicPlugin.inputActionsInstance.MidFKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(17 + semiTone, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.MidGKey : Instruments4MusicPlugin.inputActionsInstance.MidGKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(19 + semiTone, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.MidAKey : Instruments4MusicPlugin.inputActionsInstance.MidAKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(21 + semiTone, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.MidBKey : Instruments4MusicPlugin.inputActionsInstance.MidBKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(23, isSoft, key.triggered);
            }

            key = !secondaryKeyBind ? Instruments4MusicPlugin.inputActionsInstance.HighCKey : Instruments4MusicPlugin.inputActionsInstance.HighCKey2;
            if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
            {
                PlayTunedAudio(24 + semiTone, isSoft, key.triggered);
            }

            if (!secondaryKeyBind)
            {
                key = Instruments4MusicPlugin.inputActionsInstance.HighDKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(26 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.HighEKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(28, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.HighFKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(29 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.HighGKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(31 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.HighAKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(33 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.HighBKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(35, isSoft, key.triggered);
                }
            }

            MusicHUD.instance.OnMenuClicked(1, isSoft);
            MusicHUD.instance.OnMenuClicked(2, semiTone == 1);
            MusicHUD.instance.OnMenuClicked(3, isSustain);
        }
    }
}