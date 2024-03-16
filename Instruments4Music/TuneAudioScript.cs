using GameNetcodeStuff;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Instruments4Music
{
    public class TuneAudioScript : MonoBehaviour
    {
        private const double TuneCoeff = 1.0594631f;
        private static readonly float LastTimer = (InstrumentsConfig.TuneLastTime?.Value != null) ? InstrumentsConfig.TuneLastTime.Value : 3.0f;
        public static readonly float AttenuationCoeff = (InstrumentsConfig.VolumeAttenuationCoeff?.Value != null) ? InstrumentsConfig.VolumeAttenuationCoeff.Value : 2.0f;
        public static readonly float SoftModifier = (InstrumentsConfig.SoftTuneModifier?.Value != null) ? InstrumentsConfig.SoftTuneModifier.Value : 0.5f;
        private static readonly float SustainModifier = (InstrumentsConfig.SustainTuneModifier?.Value != null) ? InstrumentsConfig.SustainTuneModifier.Value : 4.0f;

        private static AudioClip? _instrClip;
        private static int _sourceNoteNumber;
        private static bool _loopAudio;
        private static Dictionary<int, AudioSource> _tunedDictionary = [];

        private static ConcurrentDictionary<int, float> _timerDictionary = new();

        private static readonly List<List<(int, bool, int)>> TuneList = [];
        private static readonly List<List<(int, bool, int)>> KeepTuneList = [];

        public static bool TheShowIsOn;
        public static bool SecondaryKeyBind;
        public static bool KeyBindInit;
        public static bool IsSustaining;
        public static bool IsAutoPlayOn;
        public static float AutoPlaySpeed = 1;
        public static float AutoPlayCount;
        public static GameObject? ActiveInstrObject;

        private static PlayerControllerB? Player => GameNetworkManager.Instance?.localPlayerController;

        public static void RegisterInstrClip(GameObject gameObject, int originNoteNumber, AudioClip clip, bool isLoop)
        {
            _timerDictionary.Clear();
            foreach (var audioSource in _tunedDictionary.Values)
            {
                Instruments4MusicPlugin.AddLog($"destroy {audioSource.GetInstanceID()}.");
                audioSource.outputAudioMixerGroup = null;
                Destroy(audioSource);
            }
            _tunedDictionary.Clear();
            Instruments4MusicPlugin.AddLog($"TunedDictionary cleared.");

            _instrClip = clip;
            _sourceNoteNumber = originNoteNumber;
            _loopAudio = isLoop;

            ActiveInstrObject = gameObject;
            TheShowIsOn = true;
            DisableController();
            MusicHud.ShowUserInterface();
            if (MusicHud.Instance.IsInputing) MusicHud.Instance.TriggerInputNote();
            Instruments4MusicPlugin.AddLog($"Playing {_instrClip.name}.");

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
            TheShowIsOn = false;
            EnableController();
            MusicHud.HideUserInterface();
            Instruments4MusicPlugin.AddLog($"DeActive {_instrClip.name}.");
        }

        /* NoteNumber: 0~35 corresponding to the Note names of 3 octaves */
        public static void PlayTunedAudio(int targetNoteNumber, bool isSoft, bool newPulse)
        {
            //Instruments4MusicPlugin.AddLog($"Playing note {targetNoteNumber}, softer: {isSoft}.");
            MusicHud.Instance.OnButtonClicked(targetNoteNumber);
            _timerDictionary[targetNoteNumber] = LastTimer;
            var volume = 1.0f;
            if (isSoft && newPulse)
            {
                volume -= SoftModifier;
            }

            if (_tunedDictionary.TryGetValue(targetNoteNumber, out var audioSource))
            {
                audioSource.volume = volume;
                if (newPulse)
                {
                    audioSource.Play();
                }

                return;
            }

            var power = targetNoteNumber - _sourceNoteNumber;

            audioSource = ActiveInstrObject?.AddComponent<AudioSource>();
            //audioSource = Player?.GetComponent<Transform>()?.gameObject?.AddComponent<AudioSource>(); //conflict to CustomSounds
            if (audioSource == null) return;
            Instruments4MusicPlugin.AddLog($"Apply mixer group Tune{power}, {audioSource.GetInstanceID()}, clip: {_instrClip.name}.");
            audioSource.outputAudioMixerGroup = Instruments4MusicPlugin.Instance.TuneMixer.FindMatchingGroups($"Tune{power}")[0];
            //audioSource.pitch = (float)Math.Pow(TuneCoeff, power);
            audioSource.clip = _instrClip;
            audioSource.volume = volume;
            audioSource.loop = _loopAudio;
            audioSource.spatialBlend = 0.5f;
            _tunedDictionary[targetNoteNumber] = audioSource;
            Instruments4MusicPlugin.AddLog($"{audioSource.GetInstanceID()} {audioSource.clip} to {_instrClip.name}.");
            audioSource.Play();
            Instruments4MusicPlugin.AddLog($"{audioSource.GetInstanceID()} {audioSource.clip} to {_instrClip.name}.");
        }

        public static void StartAutoPlay(string musicNotes)
        {
            IsAutoPlayOn = false;
            if (!TheShowIsOn) return;

            int[] lowerCaseMapping = [9, 11, 0, 2, 4, 5, 7];
            int[] digitMapping = [12, 14, 16, 17, 19, 21, 23];
            int[] upperCaseMapping = [33, 35, 24, 26, 28, 29, 31];

            string[] noteArray = Regex.Replace(musicNotes, @"\s", "").Split(',');
            var maxKeep = 0;
            TuneList.Clear();

            if (noteArray.Length < 2 || !float.TryParse(noteArray[0], out AutoPlaySpeed)) return;
            for (var i = 1; i < noteArray.Length; i++)
            {
                var subList = new List<(int, bool, int)>();
                if (noteArray[i].Length % 2 != 1 || noteArray[i].Length < 2)
                {
                    TuneList.Add(subList);
                    continue;
                }
                var param = noteArray[i][0];
                if (!char.IsDigit(param) && param is (< 'A' or > 'F') and (< 'a' or > 'f'))
                {
                    TuneList.Add(subList);
                    continue;
                }
                var paramHex = Convert.ToInt32(param.ToString(), 16);

                IsSustaining = (paramHex & 0x1) != 0;
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
                }
                TuneList.Add(subList);
            }

            AutoPlayCount = 0;
            KeepTuneList.Clear();
            for (var i = 0; i < TuneList.Count + maxKeep; i++)
            {
                KeepTuneList.Add([]);
            }

            IsAutoPlayOn = true;
        }

        public static void DoAutoPlay()
        {
            if (!IsAutoPlayOn) return;
            AutoPlayCount += Time.deltaTime;
            var num = (int)(AutoPlayCount / 0.5f * AutoPlaySpeed);
            num = num - 4;
            if (num < 0) return;
            if (num >= TuneList.Count)
            {
                IsAutoPlayOn = false;
                return;
            }
            foreach (var tuple in TuneList[num])
            {
                var (noteNumber, isSoft, keep) = tuple;
                if (KeepTuneList[num].All(tup => tup.Item1 != noteNumber))
                {
                    PlayTunedAudio(noteNumber, isSoft, true);
                    KeepTuneList[num].Add((noteNumber, isSoft, 0));
                }
                if (keep > 0 && TuneList[num + 1].All(tup => tup.Item1 != noteNumber) && KeepTuneList[num + 1].All(tup => tup.Item1 != noteNumber))
                {
                    KeepTuneList[num + 1].Add((noteNumber, isSoft, keep - 1));
                }
            }

            foreach (var tuple in KeepTuneList[num])
            {
                var (noteNumber, isSoft, keep) = tuple;
                PlayTunedAudio(noteNumber, isSoft, false);
                if (keep > 0 && TuneList[num + 1].All(tup => tup.Item1 != noteNumber) && KeepTuneList[num + 1].All(tup => tup.Item1 != noteNumber))
                {
                    KeepTuneList[num + 1].Add((noteNumber, isSoft, keep - 1));
                }
            }
        }

        public static void AudioCountDown()
        {
            foreach (var timerDictionaryKey in _timerDictionary.Keys)
            {
                var timeLast = _timerDictionary[timerDictionaryKey];
                if (_tunedDictionary.TryGetValue(timerDictionaryKey, out var audioSource) && timeLast >= 0.0f &&
                    audioSource.volume > 0.01f)
                {
                    if (IsSustaining)
                    {
                        _timerDictionary[timerDictionaryKey] = timeLast - Time.deltaTime / SustainModifier;
                        audioSource.volume = (float)(audioSource.volume * Math.Exp(-AttenuationCoeff * Time.deltaTime / SustainModifier));
                    }
                    else
                    {
                        _timerDictionary[timerDictionaryKey] = timeLast - Time.deltaTime;
                        audioSource.volume = (float)(audioSource.volume * Math.Exp(-AttenuationCoeff * Time.deltaTime));
                    }
                }
                else
                {
                    audioSource?.Stop();
                    _timerDictionary.Remove(timerDictionaryKey, out _);
                }
            }
        }

        private static void EnableController()
        {
            try
            {
                if (Player != null) Player.inTerminalMenu = false;
                Player?.playerActions.Movement.Enable();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error while subscribing to input in PlayerController!: {ex}");
            }
        }

        private static void DisableController()
        {
            try
            {
                if (Player != null) Player.inTerminalMenu = true;
                Player?.playerActions.Movement.Enable();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)$"Error while unsubscribing from input in PlayerController!: {(object)ex}");
            }
        }

        public void Awake()
        {
            _tunedDictionary = [];

            _timerDictionary = new ConcurrentDictionary<int, float>();

            TheShowIsOn = false;
            SecondaryKeyBind = false;

            if (KeyBindInit) return;
            SetupKeyBindCallbacks();
            KeyBindInit = !KeyBindInit;
        }

        public void SetupKeyBindCallbacks()
        {
            Instruments4MusicPlugin.InputActionsInstance.Showtime.performed += OnShowtimePressed;
            Instruments4MusicPlugin.InputActionsInstance.CurtainCall.performed += OnCurtainCallPressed;
            Instruments4MusicPlugin.InputActionsInstance.ChangeMode.performed += OnChangeModePressed;
            Instruments4MusicPlugin.InputActionsInstance.InputNote.performed += OnInputNotePressed;
        }

        public void OnShowtimePressed(InputAction.CallbackContext showtimeContext)
        {
            if (TheShowIsOn) return;
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
            if (!TheShowIsOn) return;
            if (!curtainCallContext.performed) return;

            Instruments4MusicPlugin.AddLog("Maybe next time.");
            DeActiveInstrument();
        }

        public void OnChangeModePressed(InputAction.CallbackContext changeModeContext)
        {
            if (!TheShowIsOn) return;
            if (!changeModeContext.performed) return;

            Instruments4MusicPlugin.AddLog("Change key bind mode.");
            SecondaryKeyBind = !SecondaryKeyBind;
            MusicHud.UpdateButtonTips();
        }

        public void OnInputNotePressed(InputAction.CallbackContext inputNoteContext)
        {
            if (!TheShowIsOn) return;
            if (!inputNoteContext.performed) return;

            Instruments4MusicPlugin.AddLog("Input music note.");
            MusicHud.Instance.TriggerInputNote();
        }

        public void Update()
        {
            if (_timerDictionary.Count > 0)
            {
                if (!IsAutoPlayOn)
                {
                    IsSustaining = Instruments4MusicPlugin.InputActionsInstance.Sustain.ReadValue<float>() > 0.5f;
                }
                AudioCountDown();
            }

            if (!TheShowIsOn) return;
            if (IsAutoPlayOn)
            {
                DoAutoPlay();
            }
            var semiTone = 0;
            var isSoft = Instruments4MusicPlugin.InputActionsInstance.Soft.ReadValue<float>() > 0.5f;
            if (Instruments4MusicPlugin.InputActionsInstance.Semitone.ReadValue<float>() > 0.5f && !SecondaryKeyBind)
            {
                semiTone = 1;
            }

            if (SecondaryKeyBind)
            {
                var key = Instruments4MusicPlugin.InputActionsInstance.LowCKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(0 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.LowDKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(2 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.LowEKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(4, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.LowFKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(5 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.LowGKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(7 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.LowAKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(9 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.LowBKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(11, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidCKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(12 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidDKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(14 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidEKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(16, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidFKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(17 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidGKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(19 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidAKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(21 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidBKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(23, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.HighCKey2;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(24 + semiTone, isSoft, key.triggered);
                }
            }
            else
            {
                var key = Instruments4MusicPlugin.InputActionsInstance.LowCKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(0 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.LowDKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(2 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.LowEKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(4, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.LowFKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(5 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.LowGKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(7 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.LowAKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(9 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.LowBKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(11, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidCKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(12 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidDKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(14 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidEKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(16, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidFKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(17 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidGKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(19 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidAKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(21 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.MidBKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(23, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.HighCKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(24 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.HighDKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(26 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.HighEKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(28, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.HighFKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(29 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.HighGKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(31 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.HighAKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(33 + semiTone, isSoft, key.triggered);
                }

                key = Instruments4MusicPlugin.InputActionsInstance.HighBKey;
                if (key != null && (key.ReadValue<float>() > 0.5f || key.triggered))
                {
                    PlayTunedAudio(35, isSoft, key.triggered);
                }
            }

            MusicHud.Instance.OnMenuClicked(1, isSoft);
            MusicHud.Instance.OnMenuClicked(2, semiTone == 1);
            MusicHud.Instance.OnMenuClicked(3, IsSustaining);
        }
    }
}