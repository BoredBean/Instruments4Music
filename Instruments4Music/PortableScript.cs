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

            var canUseObject =
                Player != null
                && (Player is { IsOwner: true, isPlayerControlled: true } &&
                    (!Player.IsServer || Player.isHostPlayerObject) || Player.isTestingPlayer)
                && Player.isHoldingObject &&
                !(Player.currentlyHeldObjectServer == null) &&
                !Player.quickMenuManager.isMenuOpen && !Player.isPlayerDead &&
                (Player.currentlyHeldObjectServer.itemProperties.usableInSpecialAnimations ||
                 Player is { isGrabbingObjectAnimation: false, inTerminalMenu: false, isTypingChat: false } &&
                 (!Player.inSpecialInteractAnimation || Player.inShockingMinigame));
            if (!canUseObject)
            {
                instrumentObj = default;
                return false;
            }

            instrumentObj = Player.currentlyHeldObjectServer?.gameObject;

            return instrumentObj != null;
        }

        public static void LetShowBegins(GameObject instrumentObj)
        {
            var cushion = instrumentObj.GetComponent<WhoopieCushionItem>();
            if (cushion != null)
            {
                Instruments4MusicPlugin.AddLog($"Playing {cushion.name}");
                TuneAudioScript.RegisterInstrClip(instrumentObj, 17,
                    cushion.fartAudios[Random.Range(0, cushion.fartAudios.Length)], false);
                return;
            }

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

                if (clip != null)
                {
                    TuneAudioScript.RegisterInstrClip(instrumentObj, 17, clip, false);
                    return;
                }
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
                else if (obj.itemProperties.grabSFX != null)
                {
                    clip = obj.itemProperties.grabSFX;
                }
                else if (obj.itemProperties.pocketSFX != null)
                {
                    clip = obj.itemProperties.pocketSFX;
                }
                else if (obj.itemProperties.throwSFX != null)
                {
                    clip = obj.itemProperties.throwSFX;
                }

                if (clip != null)
                {
                    TuneAudioScript.RegisterInstrClip(instrumentObj, 17, clip, false);
                    return;
                }
            }
        }

        public void Update()
        {
            if (Player == null || TuneAudioScript.theShowIsOn || !IsHoldingInstrument(out var instrumentObj)) return;

            var obj = instrumentObj?.GetComponent<GrabbableObject>();
            if (obj == null) return;
            var newText =
                $"Showtime!: [{InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.Showtime)}] (Hold)";
            HUDManager.Instance.ChangeControlTipMultiple(obj.itemProperties.toolTips.AddToArray(newText), true,
                obj.itemProperties);
        }
    }
}
