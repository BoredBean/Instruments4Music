using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instruments4Music
{
    public class InstrumentsConfig
    {
        internal static ConfigEntry<float>? ConfigHUDScale;
        internal static ConfigEntry<float>? ConfigHUDTransparency;

        internal static ConfigEntry<float>? TuneLastTime;
        internal static ConfigEntry<float>? VolumeAttenuationCoeff;
        internal static ConfigEntry<float>? SoftTuneModifier;
        internal static ConfigEntry<float>? SustainTuneModifier;

        internal static ConfigEntry<int>? AlarmHornBasePitch;
        internal static ConfigEntry<bool>? AlarmHornLoop;

        internal static ConfigEntry<int>? NoisyPropPitch;
        internal static ConfigEntry<bool>? NoisyPropLoop;
        internal static ConfigEntry<int>? StationaryPitch;
        internal static ConfigEntry<bool>? StationaryLoop;
        internal static ConfigEntry<int>? StationaryAudio;
        internal static ConfigEntry<int>? PotablePitch;
        internal static ConfigEntry<bool>? PotableLoop;
        internal static ConfigEntry<int>? PotableAudio;

        public static void Load()
        {
            ConfigHUDScale = Instruments4MusicPlugin.config?.Bind("HUDConfig", "HUDScale", 0.45f, 
                "Scale the music HUD.");
            ConfigHUDTransparency = Instruments4MusicPlugin.config?.Bind("HUDConfig", "HUDTransparency", 1.0f, 
                "Transparency of the music HUD.");

            TuneLastTime = Instruments4MusicPlugin.config?.Bind("TuneModifiers", "TuneLastTime", 3.0f, 
                "The audio will stop playing at TuneLastTime.(unit: seconds)");
            VolumeAttenuationCoeff = Instruments4MusicPlugin.config?.Bind("TuneModifiers", "VolumeAttenuationCoeff", 2.0f, 
                "Volume = exp(-alpha * t), alpha is this coefficient, t is time.");
            SoftTuneModifier = Instruments4MusicPlugin.config?.Bind("TuneModifiers", "SoftTuneModifier",0.5f, 
                "When you hit the soft padel. Volume = Volume * SoftModifier");
            SustainTuneModifier = Instruments4MusicPlugin.config?.Bind("TuneModifiers", "SustainTuneModifier", 4.0f,
                "When you hit the sustain padel. TuneLastTime = TuneLastTime * SustainModifier, and the Volume also attenuate slower.");

            AlarmHornBasePitch = Instruments4MusicPlugin.config?.Bind("AlarmHornConfig", "AlarmHornBasePitch", 8, 
                "The defalt audio \"ShipAlarmHornConstant.ogg\" is approximately C1#\n" +
                "[0, 35] referes to [Low C, High B]");
            AlarmHornLoop = Instruments4MusicPlugin.config?.Bind("AlarmHornConfig", "AlarmHornLoop", true, 
                "Playback the audio repeatedly utill you stop pressing.");

            NoisyPropPitch = Instruments4MusicPlugin.config?.Bind("NoisyPropDefault", "NoisyPropPitch", 12, 
                "For activable props like Air Horn or Clawn Horn.\n" +
                "12 means Mid C.\n[0, 35] referes to [Low C, High B]");
            NoisyPropLoop = Instruments4MusicPlugin.config?.Bind("NoisyPropDefault", "NoisyPropLoop", false, "" +
                "For activable props like Air Horn or Clawn Horn.\n" +
                "Playback the audio repeatedly utill you stop pressing.");
            StationaryPitch = Instruments4MusicPlugin.config?.Bind("StationaryDefault", "StationaryPitch", 12,
                "12 means Mid C.\n[0, 35] referes to [Low C, High B]");
            StationaryLoop = Instruments4MusicPlugin.config?.Bind("StationaryDefault", "StationaryLoop", false, 
                "Playback the audio repeatedly utill you stop pressing.");
            StationaryAudio = Instruments4MusicPlugin.config?.Bind("StationaryDefault", "StationaryAudio", 0, 
                "I'm not quite sure what these audio means, but anyway it's configurable.\n" +
                "0-thisAudioSourceClip, 1-playWhileTrue, 2-boolTrueAudios, 3-boolFalseAudios, 4-secondaryAudios.");
            PotablePitch = Instruments4MusicPlugin.config?.Bind("PotableDefault", "PotablePitch", 12, 
                "12 means Mid C.\n[0, 35] referes to [Low C, High B]");
            PotableLoop = Instruments4MusicPlugin.config?.Bind("PotableDefault", "PotableLoop", false, 
                "Playback the audio repeatedly utill you stop pressing.");
            PotableAudio = Instruments4MusicPlugin.config?.Bind("PotableDefault", "PotableAudio", 0,
                "I think I know what these audio means literally.\n" +
                "0-dropSFX, 1-grabSFX, 2-pocketSFX, 3-throwSFX.");

            Instruments4MusicPlugin.AddLog($"All configs loaded!");
        }
    }
}
