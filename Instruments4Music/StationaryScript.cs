using System;
using System.Reflection;
using System.Text.RegularExpressions;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Instruments4Music
{
    public class StationaryScript : MonoBehaviour
    {
        private const int InteractableObjectsMask = 1 << 9; //interactableObjects

        private static PlayerControllerB? Player => GameNetworkManager.Instance?.localPlayerController;

        public static bool IsLookingAtInstrument(out GameObject? instrumentObj)
        {
            if (Player == null)
            {
                instrumentObj = default;
                return false;
            }

            //When facing the control panel, the X-axis increases forward, the Y-axis increases upwards, and the Z-axis increases to the left.
            var interactRay = new Ray(Player.gameplayCamera.transform.position,
                Player.gameplayCamera.transform.forward);

            if (Player.isTypingChat)
            {
                instrumentObj = default;
                return false;
            }

            if (!Physics.Raycast(interactRay, out var hit, Player.grabDistance, InteractableObjectsMask,
                    QueryTriggerInteraction.Ignore) && !Physics.Raycast(
                    Player.gameplayCamera.transform.position + Vector3.up * 5f, Vector3.down, out hit, 5f,
                    InteractableObjectsMask, QueryTriggerInteraction.Ignore))
            {
                instrumentObj = default;
                return false;
            }

            instrumentObj = hit.collider.gameObject;
            if (instrumentObj == null)
            {
                instrumentObj = default;
                return false;
            }

            var horn = instrumentObj.GetComponent<ShipAlarmCord>();
            if (horn != null)
            {
                return true;
            }

            var charger = instrumentObj.GetComponent<ItemCharger>();
            if (charger != null)
            {
                return true;
            }

            var obj = instrumentObj.GetComponent<AnimatedObjectTrigger>();
            if (obj != null)
            {
                return true;
            }

            return false;
        }

        public static void LetShowBegins(GameObject instrumentObj)
        {
            var horn = instrumentObj.GetComponent<ShipAlarmCord>();
            if (horn != null)
            {
                Instruments4MusicPlugin.AddLog($"Playing {horn.name}");
                TuneAudioScript.RegisterInstrClip(instrumentObj, InstrumentsConfig.AlarmHornBasePitch.Value, horn.hornClose.clip, InstrumentsConfig.AlarmHornLoop.Value);
                return;
            }
            var charger = instrumentObj.GetComponent<ItemCharger>();
            if (charger != null)
            {
                Instruments4MusicPlugin.AddLog($"Playing {charger.name}");
                TuneAudioScript.RegisterInstrClip(instrumentObj, InstrumentsConfig.StationaryPitch.Value, charger.zapAudio.clip, InstrumentsConfig.StationaryLoop.Value);
                return;
            }

            var obj = instrumentObj.GetComponent<AnimatedObjectTrigger>();
            if (obj != null)
            {
                Instruments4MusicPlugin.AddLog($"Playing {obj.name}");
                AudioClip? clip = default;
                if(InstrumentsConfig.StationaryAudio.Value == 0)
                {
                    clip = obj.thisAudioSource.clip;
                }
                else if(InstrumentsConfig.StationaryAudio.Value == 1)
                {
                    clip = obj.playWhileTrue;
                }
                else if (InstrumentsConfig.StationaryAudio.Value == 2)
                {
                    clip = obj.boolTrueAudios[0];
                }
                else if (InstrumentsConfig.StationaryAudio.Value == 3)
                {
                    clip = obj.boolFalseAudios[0];
                }
                else if (InstrumentsConfig.StationaryAudio.Value == 4)
                {
                    clip = obj.secondaryAudios[0];

                }
                if (clip != null)
                {
                    TuneAudioScript.RegisterInstrClip(instrumentObj, InstrumentsConfig.StationaryPitch.Value, clip, InstrumentsConfig.StationaryLoop.Value);
                    return;
                }

                if (obj.thisAudioSource.clip != null)
                {
                    clip = obj.thisAudioSource.clip;
                }
                else if (obj.playWhileTrue != null)
                {
                    clip = obj.playWhileTrue;
                }
                else if (obj.boolTrueAudios.Length > 0)
                {
                    clip = obj.boolTrueAudios[0];
                }
                else if (obj.boolFalseAudios.Length > 0)
                {
                    clip = obj.boolFalseAudios[0];
                }
                else if (obj.secondaryAudios.Length > 0)
                {
                    clip = obj.secondaryAudios[0];
                }
                if (clip != null)
                {
                    TuneAudioScript.RegisterInstrClip(instrumentObj, InstrumentsConfig.StationaryPitch.Value, clip, InstrumentsConfig.StationaryLoop.Value);
                    return;
                }
            }
        }

        public void Update()
        {
            if (TuneAudioScript.theShowIsOn || !IsLookingAtInstrument(out var instrumentObj) || Player == null) return;
            var newText =
                $"Showtime!: [{InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.Showtime)}] (Hold)";

            var trigger = instrumentObj?.GetComponent<InteractTrigger>();

            if (trigger == null || trigger.hoverTip.EndsWith(newText)) return;
            trigger.hoverTip += "\n" + newText;

            Regex regex = new(@"Showtime!: \[.*?] \(Hold\)$");
            if (regex.IsMatch(trigger.hoverTip))
            {
                trigger.hoverTip = regex.Replace(trigger.hoverTip, newText);
            }
            else
            {
                trigger.hoverTip += "\n" + newText;
            }
        }
    }
}
