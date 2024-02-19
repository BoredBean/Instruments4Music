using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace Instruments4Music
{
    public class PortableScript : MonoBehaviour
    {
        private const int InteractableObjectsMask = 1 << 9; //interactableObjects

        private static PlayerControllerB? Player => GameNetworkManager.Instance?.localPlayerController;

        public static bool IsHoldingInstrument(out GameObject? instrumentObj)
        {
            if (Player == null || Player is { IsOwner: true, isPlayerDead: true } &&
                (!Player.IsServer || Player.isHostPlayerObject))
            {
                instrumentObj = default;
                return false;
            }
            instrumentObj = Player.currentlyHeldObjectServer?.gameObject;

            return instrumentObj != null;
        }

        public static void LetShowBegins(GameObject instrumentObj)
        {
            var prop = instrumentObj.GetComponent<NoisemakerProp>();
            if (prop != null)
            {
                Instruments4MusicPlugin.AddLog($"Playing {prop.name}");
                AudioClip? clip = default;
                if (prop.noiseAudio.clip != null)
                {
                    clip = prop.noiseAudio.clip;
                }
                else if (prop.noiseSFX.Length > 0)
                {
                    clip = prop.noiseSFX[0];
                }

                if (clip == null) return;
                TuneAudioScript.RegisterInstrClip(instrumentObj, 17, clip, false);
            }

            var obj = instrumentObj.GetComponent<GrabbableObject>();
            if (obj != null)
            {
                Instruments4MusicPlugin.AddLog($"Playing {obj.name}");
                AudioClip? clip = default;
                if (obj.itemProperties.dropSFX != null)
                {
                    clip = obj.itemProperties.dropSFX;
                }

                if (clip == null) return;
                TuneAudioScript.RegisterInstrClip(instrumentObj, 17, clip, false);
            }
        }

        public void Update()
        {
            if (Player == null || TuneAudioScript.theShowIsOn || !IsHoldingInstrument(out var instrumentObj)) return;

            var prop = instrumentObj?.GetComponent<NoisemakerProp>();
            if (prop == null) return;
            var newText =
                $"Showtime!: [{InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.Showtime)}] (Hold)";
            HUDManager.Instance.ChangeControlTipMultiple(prop.itemProperties.toolTips.AddToArray(newText), true,
                prop.itemProperties);
        }
    }
}
