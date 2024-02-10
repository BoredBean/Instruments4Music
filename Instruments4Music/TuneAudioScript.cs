using GameNetcodeStuff;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Instruments4Music
{
    public class TuneAudioScript : MonoBehaviour
    {
        const double TuneCoeff = 1.059463f; 
        static float lastTimer = 3.0f;
        static float softModifier = 0.5f;
        static float sustainModifier = 4.0f;

        static Dictionary<string, (AudioClip, int)> InstrDictionary = new Dictionary<string, (AudioClip, int)>();
        static Dictionary<(string, int), AudioSource> TunedDictionary = new Dictionary<(string, int), AudioSource>();
        static ConcurrentDictionary<(string, int), float> TimerDictionary = new ConcurrentDictionary<(string, int), float>();

        public static bool theShowIsOn = false;
        public static bool movableMode = false;
        public static string activeClipName = "";
        public static GameObject activeInstrObject;

        private static PlayerControllerB Player => GameNetworkManager.Instance?.localPlayerController;

        public static void RegisterInstrClip(GameObject gameObject, int sourceNoteNumber, AudioClip clip)
        {
            if (!InstrDictionary.ContainsKey(clip.name))
            {
                InstrDictionary.Add(clip.name, (clip, sourceNoteNumber));
            }

            activeClipName = clip.name;
            activeInstrObject = gameObject;
            theShowIsOn = true;
            OnDisable();
            Player.inTerminalMenu = true;
            Instruments4MusicPlugin.LOGGER.LogInfo($"Playing {activeClipName}.");
        }

        public static void DeactiveInstrument()
        {
            theShowIsOn = false;
            OnEnable();
            Player.inTerminalMenu = false;
            Instruments4MusicPlugin.LOGGER.LogInfo($"Deactive {activeClipName}.");
        }

        /* NoteNumber: 0~35 corresponding to the Note names of 3 octaves */
        public static void PlayTunedAudio(int targetNoteNumber, bool isSoft)
        {
            Instruments4MusicPlugin.LOGGER.LogInfo($"Playing note {targetNoteNumber}.");

            TimerDictionary[(activeClipName, targetNoteNumber)] = lastTimer;
            var volume = 1.0f;
            if (isSoft)
            {
                volume -= softModifier;
            }

            if (TunedDictionary.TryGetValue((activeClipName, targetNoteNumber), out var audioSource))
            {

                audioSource.volume = volume;
                audioSource.Play();
                return;
            }

            if (!InstrDictionary.TryGetValue(activeClipName, out var value))
            {
                return;
            }

            var clip = value.Item1;
            var sourceNoteNumber = value.Item2;
            var power = targetNoteNumber - sourceNoteNumber;

            audioSource = activeInstrObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.pitch = (float)Math.Pow(TuneCoeff, power);
            audioSource.volume = volume;
            audioSource.Play();
            TunedDictionary.Add((activeClipName, targetNoteNumber), audioSource);

            return;
        }

        public static void AudioCountDown(bool isSustain)
        {
            foreach (var timerDictionaryKey in TimerDictionary.Keys)
            {
                float timeLast = TimerDictionary[timerDictionaryKey];
                if (TunedDictionary.TryGetValue(timerDictionaryKey, out var audioSource) && timeLast >= 0.0 && audioSource.volume > 0.0)
                {
                    if (isSustain)
                    {
                        TimerDictionary[timerDictionaryKey] = timeLast - Time.deltaTime / sustainModifier; 
                        audioSource.volume -= Time.deltaTime / sustainModifier / lastTimer;

                    }
                    else
                    {
                        TimerDictionary[timerDictionaryKey] = timeLast - Time.deltaTime;
                        audioSource.volume -= Time.deltaTime / lastTimer;
                    }
                }
                else
                {
                    TimerDictionary.Remove(timerDictionaryKey, out _);
                    audioSource.Stop();
                }
            }
        }
        static void OnEnable()
        {
            try
            {
                Player.playerActions.Movement.Enable();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)string.Format("Error while subscribing to input in PlayerController!: {0}", (object)ex));
            }
        }

        static void OnDisable()
        {
            try
            {
                Player.playerActions.Movement.Enable();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)string.Format("Error while unsubscribing from input in PlayerController!: {0}", (object)ex));
            }
        }

        public void Update()
        {
            if (!theShowIsOn) return;

            int semiTone = 0;
            bool isSoft = Instruments4MusicPlugin.InputActionsInstance.Soft.triggered;
            bool isSustain = Instruments4MusicPlugin.InputActionsInstance.Sustain.triggered;
            if (Instruments4MusicPlugin.InputActionsInstance.Semitone.triggered)
            {
                semiTone = 1;
            }
            if (Instruments4MusicPlugin.InputActionsInstance.LowCKey.triggered)
            {
                PlayTunedAudio(0 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.LowDKey.triggered)
            {
                PlayTunedAudio(2 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.LowEKey.triggered)
            {
                PlayTunedAudio(4, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.LowFKey.triggered)
            {
                PlayTunedAudio(5 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.LowGKey.triggered)
            {
                PlayTunedAudio(7 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.LowAKey.triggered)
            {
                PlayTunedAudio(9 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.LowBKey.triggered)
            {
                PlayTunedAudio(11, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.MidCKey.triggered)
            {
                PlayTunedAudio(12 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.MidDKey.triggered)
            {
                PlayTunedAudio(14 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.MidEKey.triggered)
            {
                PlayTunedAudio(16, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.MidFKey.triggered)
            {
                PlayTunedAudio(17 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.MidGKey.triggered)
            {
                PlayTunedAudio(19 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.MidAKey.triggered)
            {
                PlayTunedAudio(21 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.MidBKey.triggered)
            {
                PlayTunedAudio(23, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.HighCKey.triggered)
            {
                PlayTunedAudio(24 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.HighDKey.triggered)
            {
                PlayTunedAudio(26 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.HighEKey.triggered)
            {
                PlayTunedAudio(28, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.HighFKey.triggered)
            {
                PlayTunedAudio(29 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.HighGKey.triggered)
            {
                PlayTunedAudio(31 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.HighAKey.triggered)
            {
                PlayTunedAudio(33 + semiTone, isSoft);
            }

            if (Instruments4MusicPlugin.InputActionsInstance.HighBKey.triggered)
            {
                PlayTunedAudio(35, isSoft);
            }

            AudioCountDown(isSustain);
        }
    }
}