using GameNetcodeStuff;
using MonoMod.Cil;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace Instruments4Music
{
    public class StationaryScript : MonoBehaviour
    {
        private int placeableShipObjectsMask = 67108864;

        private static PlayerControllerB player => GameNetworkManager.Instance?.localPlayerController;
        private bool _lookingAtInstrument = false;

        private bool IsLookingAtInstrument(out PlaceableShipObject instrumentObj)
        {
            //When facing the control panel, the X-axis increases forward, the Y-axis increases upwards, and the Z-axis increases to the left.
            Ray interactRay = new Ray(player.gameplayCamera.transform.position, player.gameplayCamera.transform.forward);
            RaycastHit hit;

            if (player == null || player.isTypingChat || !player.isInHangarShipRoom)
            {
                instrumentObj = default;
                return false;
            }
            if ((!Physics.Raycast(interactRay, out hit, player.grabDistance, placeableShipObjectsMask, QueryTriggerInteraction.Ignore) && !Physics.Raycast(player.gameplayCamera.transform.position + Vector3.up * 5f, Vector3.down, out hit, 5f, placeableShipObjectsMask, QueryTriggerInteraction.Ignore)) || !hit.collider.gameObject.CompareTag("PlaceableObject"))
            {
                instrumentObj = default;
                return false;
            }
            if (hit.collider.gameObject.layer == 8)
            {
                Instruments4MusicPlugin.LOGGER.LogInfo("Layer 8 is: " + hit.transform.name);
            }

            instrumentObj = hit.collider.gameObject.GetComponent<PlaceableShipObject>();

            Instruments4MusicPlugin.LOGGER.LogInfo("ObjName: " + hit.transform.name);
            Instruments4MusicPlugin.LOGGER.LogInfo("ObjType: " + hit.transform.gameObject.GetType());

            return instrumentObj != null;
        }

        private bool isCoroutineRunning = false;
        private void Update()
        {
            if (!isCoroutineRunning)
            {
                IsLookingAtInstrument(out PlaceableShipObject instrumentObj);
                StartCoroutine(DelayedAction());
            }

            IEnumerator DelayedAction()
            {
                isCoroutineRunning = true;

                // 等待1秒钟
                yield return new WaitForSeconds(0.5f);

                // 在这里执行你想要延时的操作
                Debug.Log("One second has passed!");

                isCoroutineRunning = false;
            }
        }
    }
}
