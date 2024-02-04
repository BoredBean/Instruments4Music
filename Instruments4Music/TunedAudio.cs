using System;
using System.Collections.Generic;
using UnityEngine;

namespace Instruments4Music
{
    public static class TunedAudio
    {
        private const double TuneCoeff = 1.059463f;

        static readonly Dictionary<string, (AudioClip, int)> InstrDictionary = [];
        static readonly Dictionary<(string, int), AudioSource> TunedDictionary = [];

        public static bool theShowIsOn = false;
        public static string activeInstrName = "";

        public static void RegisterInstrClip(string instrName, int sourceNoteNumber, AudioClip clip)
        {
            if (!InstrDictionary.ContainsKey(instrName))
            {
                InstrDictionary.Add(instrName, (clip, sourceNoteNumber));
            }
            activeInstrName = instrName;
            theShowIsOn = true;
        }

        /* NoteNumber: 0~35 corresponding to the Note names of 3 octaves */
        public static AudioSource GetTunedAudio(int targetNoteNumber, GameObject gameObject)
        {
            if (TunedDictionary.TryGetValue((activeInstrName, targetNoteNumber), out var audioSource))
            {
                return audioSource;
            }

            if (!InstrDictionary.TryGetValue(activeInstrName, out var value))
            {
                return null;
            }

            var clip = value.Item1;
            var sourceNoteNumber = value.Item2;
            var power = targetNoteNumber - sourceNoteNumber;

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = 1.0f;
            audioSource.pitch = (float)Math.Pow(TuneCoeff, power);
            TunedDictionary.Add((activeInstrName, targetNoteNumber), audioSource);

            return audioSource;
        }
    }
}
