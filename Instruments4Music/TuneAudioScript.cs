using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using static Unity.Audio.Handle;

namespace Instruments4Music
{
    public class TuneAudioScript : MonoBehaviour
    {
        private const double TuneCoeff = 1.0594631f;
        static float lastTimer = InstrumentsConfig.TuneLastTime.Value;
        static float attenuationCoeff = InstrumentsConfig.VolumeAttenuationCoeff.Value;
        static float softModifier = InstrumentsConfig.SoftTuneModifier.Value;
        static float sustainModifier = InstrumentsConfig.SustainTuneModifier.Value;

        static Dictionary<string, (AudioClip, int, bool)> InstrDictionary = [];
        static Dictionary<(string, int), AudioSource> TunedDictionary = [];

        static ConcurrentDictionary<(string, int), float> TimerDictionary =
            new();
        static List<List<(int, bool, int)>> tuneList = [];
        static List<List<(int, bool, int)>> keepTuneList = [];

        public static bool theShowIsOn = false;
        public static bool secondaryKeyBind = false;
        public static bool keyBindInit = false;
        public static string activeClipName = "";
        public static bool isSustaining = false;
        public static bool isAutoPlayOn = false;
        public static float autoPlaySpeed = 1;
        public static float autoPlayCount = 0;
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

            //StartAutoPlay("2.7,01030,,,,030,,,,032,,,,042,,,,0b030,,,,030,,,,030,,,," +
            //    "010,,020,,0a030,,,,030,,,,032,,,,042,,,,0g052,,,,040,,,,030,,,,020,,,," +
            //    "0f010,,,,010,,,,010,,,,022,,,,0e010,,,,010,,,,010,,,,050,,,," +
            //    "0d010,,,,010,,020,,030,,020,,010,,,,0g010,,,,010,,020,,030,,020,,010,,020,," +
            //    "0c030,,0g0,,0e030,,0g0,,0c032,,0g0,,0d042,,0g0,,0b030,,0g0,,0d030,,0g0,,0b030,,0g0,," +
            //    "0c010,,020,,0a030,,0e0g0,,0c030,,0g0,,0a032,,0e0g0,,0c032,,040,,0g052,,0e0g0,,0c010,,0e0g0,,0g050,,040,,0c030,,0e020,," +
            //    "0f010,,0c0g0,,0a010,,0c0g0,,0f010,,0c0g0,,0a052,,0c0g0,,0e0C0,,0c010,,0g050,,0c040,,0e030,,020,,0g010,,0c0g0,," +
            //    "0d010,,0c0g0,,0f010,,0a020,,0d030,,020,,0f010,,0c0g0,,0g010,,0c0g0,,0c010,,0f050,,0g040,,030,,0b020,,,," +
            //    "0c030,,0g0,,0e030,,0c0g0,,0c032,,0c0g0,,0f042,,0g0,,0b030,,0c0g0,,0d030,,0c0g0,,0e030,,0d0g0,," +
            //    "0g010,,0d020,,0a030,,0e0a0,,0c030,,0e0a0,,0a032,,0e0a0,,0d042,,0e0a0,,0g052,,0c010,,0a0C0,,0f0,,0c050,,040,,0e030,,0d020,," +
            //    "0f010,,0c0g0,,0f010,,0c0g0,,0g010,,0c0g0,,0a052,,,,0e0C0,,0d0,,0g050,,0c040,,0e030,,020,,0g010,,0c0g0,," +
            //    "0d010,,0c0g0,,0f010,,0a020,,0d030,,020,,010,,0g0,,0g010,,0c0g0,,0c010,,0g050,,0f040,,0e033,,0c024,,,,,," +
            //    "0c017,,0g0,,,,2g0,,,,2g0,,,,,,0c0");
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
            if (isSoft && newPulse)
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

            audioSource = Player?.GetComponent<Transform>()?.gameObject?.AddComponent<AudioSource>();
            if (audioSource == null) return;
            Instruments4MusicPlugin.AddLog($"Apply tune mixer group Tune{power}.");
            audioSource.outputAudioMixerGroup = Instruments4MusicPlugin.instance.tuneMixer.FindMatchingGroups($"Tune{power}")[0];
            //audioSource.pitch = (float)Math.Pow(TuneCoeff, power);
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.loop = isLoop;
            audioSource.spatialBlend = 0.5f;
            TunedDictionary.Add((activeClipName, targetNoteNumber), audioSource);
            audioSource.Play();
        }

        public static void StartAutoPlay(string musicNotes)
        {
            isAutoPlayOn = false;
            if (!theShowIsOn) return;

            int[] lowerCaseMapping = [9, 11, 0, 2, 4, 5, 7];
            int[] digitMapping = [12, 14, 16, 17, 19, 21, 23];
            int[] upperCaseMapping = [33, 35, 24, 26, 28, 29, 31];

            string[] noteArray = Regex.Replace(musicNotes, @"\s", "").Split(',');
            var maxKeep = 0;
            tuneList.Clear();

            if (noteArray.Length < 2 || !float.TryParse(noteArray[0], out autoPlaySpeed)) return;
            for (var i = 1; i < noteArray.Length; i++)
            {
                var subList = new List<(int, bool, int)>();
                if (noteArray[i].Length % 2 != 1 || noteArray[i].Length < 2)
                {
                    tuneList.Add(subList);
                    continue;
                }
                var param = noteArray[i][0];
                if (!char.IsDigit(param) && param is (< 'A' or > 'F') and (< 'a' or > 'f'))
                {
                    tuneList.Add(subList);
                    continue;
                }
                var paramHex = Convert.ToInt32(param.ToString(), 16);

                isSustaining = (paramHex & 0x1) != 0;
                var isSoft = (paramHex & 0x2) != 0;

                for (var j = 1; j < noteArray[i].Length; j += 2)
                {
                    var noteChar = noteArray[i][j];
                    int noteNumber;
                    switch (noteChar)
                    {
                        case >= 'a' and <= 'g':
                            noteNumber = lowerCaseMapping[noteChar - 'a'];
                            break;
                        case >= '1' and <= '7':
                            noteNumber = digitMapping[noteChar - '1'];
                            break;
                        case >= 'A' and <= 'G':
                            noteNumber = upperCaseMapping[noteChar - 'A'];
                            break;
                        default:
                            continue;
                    }

                    var modifierChar = noteArray[i][j + 1];
                    if (!char.IsDigit(modifierChar) && modifierChar is (< 'A' or > 'F') and (< 'a' or > 'f')) continue;

                    noteNumber += (modifierChar >= '8') ? 1 : 0;
                    var keep = Convert.ToInt32(modifierChar.ToString(), 16) & 0x7;
                    maxKeep = keep > maxKeep ? keep : maxKeep;
                    subList.Add((noteNumber, isSoft, keep));
                    Instruments4MusicPlugin.AddLog($"set ({i},{j}) {noteNumber} {isSoft} {keep}");
                }
                tuneList.Add(subList);
            }

            autoPlayCount = 0;
            keepTuneList.Clear();
            for (var i = 0; i < tuneList.Count + maxKeep; i++)
            {
                keepTuneList.Add([]);
            }

            isAutoPlayOn = true;
        }

        public static void DoAutoPlay()
        {
            if (!isAutoPlayOn) return;
            autoPlayCount += Time.deltaTime;
            var num = (int)(autoPlayCount / 0.5f * autoPlaySpeed);
            //num = num - 30;
            if (num < 0) return;
            if (num >= tuneList.Count)
            {
                isAutoPlayOn = false;
                return;
            }
            Instruments4MusicPlugin.AddLog($"num = {num}");
            foreach (var tuple in tuneList[num])
            {
                var (noteNumber, isSoft, keep) = tuple;
                Instruments4MusicPlugin.AddLog($"get {noteNumber} {isSoft} {keep}");
                if (keepTuneList[num].All(tup => tup.Item1 != noteNumber))
                {
                    Instruments4MusicPlugin.AddLog($"play {num}");
                    PlayTunedAudio(noteNumber, isSoft, true);
                    keepTuneList[num].Add((noteNumber, isSoft, 0));
                }
                if (keep > 0 && tuneList[num + 1].All(tup => tup.Item1 != noteNumber) && keepTuneList[num + 1].All(tup => tup.Item1 != noteNumber))
                {
                    keepTuneList[num + 1].Add((noteNumber, isSoft, keep - 1));
                }
            }

            foreach (var tuple in keepTuneList[num])
            {
                var (noteNumber, isSoft, keep) = tuple;
                PlayTunedAudio(noteNumber, isSoft, false);
                if (keep > 0 && tuneList[num + 1].All(tup => tup.Item1 != noteNumber) && keepTuneList[num + 1].All(tup => tup.Item1 != noteNumber))
                {
                    keepTuneList[num + 1].Add((noteNumber, isSoft, keep - 1));
                }
            }
        }

        public static void AudioCountDown()
        {
            foreach (var timerDictionaryKey in TimerDictionary.Keys)
            {
                var timeLast = TimerDictionary[timerDictionaryKey];
                if (TunedDictionary.TryGetValue(timerDictionaryKey, out var audioSource) && timeLast >= 0.0f &&
                    audioSource.volume > 0.01f)
                {
                    if (isSustaining)
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
                Player?.playerActions.Movement.Enable();
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
                Player?.playerActions.Movement.Enable();
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

            TimerDictionary = new ConcurrentDictionary<(string, int), float>();

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
            if (TimerDictionary.Count > 0)
            {
                if (!isAutoPlayOn)
                {
                    isSustaining = Instruments4MusicPlugin.inputActionsInstance.Sustain.ReadValue<float>() > 0.5f;
                }
                AudioCountDown();
            }

            if (!theShowIsOn) return;
            if (isAutoPlayOn)
            {
                DoAutoPlay();
            }
            var semiTone = 0;
            var isSoft = Instruments4MusicPlugin.inputActionsInstance.Soft.ReadValue<float>() > 0.5f;
            if (Instruments4MusicPlugin.inputActionsInstance.Semitone.ReadValue<float>() > 0.5f && !secondaryKeyBind)
            {
                semiTone = 1;
            }

            if (secondaryKeyBind)
            {
                var key = Instruments4MusicPlugin.inputActionsInstance.LowCKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(0 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.LowDKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(2 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.LowEKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(4, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.LowFKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(5 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.LowGKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(7 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.LowAKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(9 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.LowBKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(11, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidCKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(12 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidDKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(14 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidEKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(16, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidFKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(17 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidGKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(19 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidAKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(21 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidBKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(23, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.HighCKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(24 + semiTone, isSoft, key.triggered);
                }
            }
            else
            {
                var key = Instruments4MusicPlugin.inputActionsInstance.LowCKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(0 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.LowDKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(2 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.LowEKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(4, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.LowFKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(5 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.LowGKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(7 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.LowAKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(9 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.LowBKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(11, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidCKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(12 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidDKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(14 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidEKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(16, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidFKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(17 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidGKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(19 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidAKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(21 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.MidBKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(23, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.inputActionsInstance.HighCKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(24 + semiTone, isSoft, key.triggered);
                }

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
            MusicHUD.instance.OnMenuClicked(3, isSustaining);
        }
    }
}