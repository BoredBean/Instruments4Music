using GameNetcodeStuff;
using MonoMod.Cil;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem.HID;

namespace Instruments4Music
{
    public class StationaryScript : MonoBehaviour
    {
        private int _interactableShipObjectsMask = 1 << 9;   //interactableObjects

        private static PlayerControllerB Player => GameNetworkManager.Instance?.localPlayerController;
        
        private bool IsLookingAtInstrument(out GameObject instrumentObj)
        {
            //When facing the control panel, the X-axis increases forward, the Y-axis increases upwards, and the Z-axis increases to the left.
            var interactRay = new Ray(Player.gameplayCamera.transform.position, Player.gameplayCamera.transform.forward);

            if (Player == null || Player.isTypingChat || !Player.isInHangarShipRoom)
            {
                instrumentObj = default;
                return false;
            }
            if (!Physics.Raycast(interactRay, out var hit, Player.grabDistance, _interactableShipObjectsMask, QueryTriggerInteraction.Ignore) && !Physics.Raycast(Player.gameplayCamera.transform.position + Vector3.up * 5f, Vector3.down, out hit, 5f, interactableShipObjectsMask, QueryTriggerInteraction.Ignore))
            {
                instrumentObj = default;
                return false;
            }

            instrumentObj = hit.collider.gameObject;

            return instrumentObj != null;
        }

        private void LetShowBegins()
        {
            if (!IsLookingAtInstrument(out var instrumentObj))
            {
                return;
            }
            ;
            ShipAlarmCord horn = instrumentObj.GetComponent<ShipAlarmCord>();
            if (horn != null)
            {
                Instruments4MusicPlugin.LOGGER.LogInfo("Playing ShipAlarm");
                TunedAudio.RegisterInstrClip("ShipAlarm", 8, horn.hornClose.clip);
                return;
            }
        }

        private void Update()
        {

        }
    }
}
