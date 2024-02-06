using System;
using System.Collections.Generic;
using UnityEngine;

namespace Instruments4Music
{
    public static class TunedAudio
    {
        private const double TuneCoeff = 1.059463f;

        static Dictionary<string, (AudioClip, int)> InstrDictionary = new Dictionary<string, (AudioClip, int)>();
        static Dictionary<(string, int), AudioSource> TunedDictionary = new Dictionary<(string, int), AudioSource>();

        public static bool theShowIsOn = false;
        public static string activeClipName = "";
        public static GameObject activeInstrObject;

        public static void RegisterInstrClip(GameObject gameObject, int sourceNoteNumber, AudioClip clip)
        {
            if (!InstrDictionary.ContainsKey(clip.name))
            {
                InstrDictionary.Add(clip.name, (clip, sourceNoteNumber));
            }

            activeClipName = clip.name;
            activeInstrObject = gameObject;
            theShowIsOn = true;
        }

        public static void DeactiveInstrument()
        {
            theShowIsOn = false;
        }

        /* NoteNumber: 0~35 corresponding to the Note names of 3 octaves */
        public static AudioSource GetTunedAudio(int targetNoteNumber)
        {
            if (TunedDictionary.TryGetValue((activeClipName, targetNoteNumber), out var audioSource))
            {
                return audioSource;
            }

            if (!InstrDictionary.TryGetValue(activeClipName, out var value))
            {
                return null;
            }

            var clip = value.Item1;
            var sourceNoteNumber = value.Item2;
            var power = targetNoteNumber - sourceNoteNumber;

            audioSource = activeInstrObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = 1.0f;
            audioSource.pitch = (float)Math.Pow(TuneCoeff, power);
            TunedDictionary.Add((activeClipName, targetNoteNumber), audioSource);

            return audioSource;
        }
    }
}
