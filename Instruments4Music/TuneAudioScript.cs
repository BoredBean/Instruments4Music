using GameNetcodeStuff;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Instruments4Music
{
    public  class TuneAudioScript:MonoBehaviour
    {
        private const double TuneCoeff = 1.059463f;
        private static float lastTimer = 1.0f;

        static Dictionary<string, (AudioClip, int)> InstrDictionary = new Dictionary<string, (AudioClip, int)>();
        static Dictionary<(string, int), AudioSource> TunedDictionary = new Dictionary<(string, int), AudioSource>();
        static ConcurrentDictionary<(string, int), float> TimerDictionary = new ConcurrentDictionary<(string, int), float>();

        public static bool theShowIsOn = false;
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
        public static void PlayTunedAudio(int targetNoteNumber)
        {
            Instruments4MusicPlugin.LOGGER.LogInfo($"Playing note {targetNoteNumber}.");

            TimerDictionary[(activeClipName, targetNoteNumber)] = lastTimer;
            if (TunedDictionary.TryGetValue((activeClipName, targetNoteNumber), out var audioSource))
            {
                audioSource.volume = 1.0f;
                audioSource.Play();
                return ;
            }

            if (!InstrDictionary.TryGetValue(activeClipName, out var value))
            {
                return ;
            }

            var clip = value.Item1;
            var sourceNoteNumber = value.Item2;
            var power = targetNoteNumber - sourceNoteNumber;

            audioSource = activeInstrObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.pitch = (float)Math.Pow(TuneCoeff, power);
            audioSource.volume = 1.0f;
            audioSource.Play();
            TunedDictionary.Add((activeClipName, targetNoteNumber), audioSource);

            return;
        }

        public static void AudioCountDown()
        {
            foreach (var timerDictionaryKey in TimerDictionary.Keys)
            {
                float timeLast = TimerDictionary[timerDictionaryKey];
                if (TunedDictionary.TryGetValue(timerDictionaryKey, out AudioSource audioSource))
                {
                    if (timeLast >= 0.0)
                    {
                        TimerDictionary[timerDictionaryKey] = timeLast - Time.deltaTime;
                        audioSource.volume = timeLast / lastTimer;
                    }
                    else
                    {
                        TimerDictionary.Remove(timerDictionaryKey, out float t);
                        audioSource.Stop();
                    }
                }
                else
                {
                    TimerDictionary.Remove(timerDictionaryKey, out float t);
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
            if (theShowIsOn)
            {
                if (Instruments4MusicPlugin.InputActionsInstance.HighCKey.triggered)
                {
                    PlayTunedAudio(0);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.HighDKey.triggered)
                {
                    PlayTunedAudio(2);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.HighEKey.triggered)
                {
                    PlayTunedAudio(4);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.HighFKey.triggered)
                {
                    PlayTunedAudio(5);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.HighGKey.triggered)
                {
                    PlayTunedAudio(7);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.HighAKey.triggered)
                {
                    PlayTunedAudio(9);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.HighBKey.triggered)
                {
                    PlayTunedAudio(11);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.MidCKey.triggered)
                {
                    PlayTunedAudio(12);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.MidDKey.triggered)
                {
                    PlayTunedAudio(14);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.MidEKey.triggered)
                {
                    PlayTunedAudio(16);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.MidFKey.triggered)
                {
                    PlayTunedAudio(17);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.MidGKey.triggered)
                {
                    PlayTunedAudio(19);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.MidAKey.triggered)
                {
                    PlayTunedAudio(21);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.MidBKey.triggered)
                {
                    PlayTunedAudio(23);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.LowCKey.triggered)
                {
                    PlayTunedAudio(24);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.LowDKey.triggered)
                {
                    PlayTunedAudio(26);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.LowEKey.triggered)
                {
                    PlayTunedAudio(28);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.LowFKey.triggered)
                {
                    PlayTunedAudio(29);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.LowGKey.triggered)
                {
                    PlayTunedAudio(31);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.LowAKey.triggered)
                {
                    PlayTunedAudio(33);
                }

                if (Instruments4MusicPlugin.InputActionsInstance.LowBKey.triggered)
                {
                    PlayTunedAudio(35);
                }

                AudioCountDown();
            }
        }
    }
}
