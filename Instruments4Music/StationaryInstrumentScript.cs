using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Instruments4Music
{
    public class StationaryInstrumentScript : MonoBehaviour
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
            if (!Physics.Raycast(interactRay, out var hit, Player.grabDistance, _interactableShipObjectsMask, QueryTriggerInteraction.Ignore) && !Physics.Raycast(Player.gameplayCamera.transform.position + Vector3.up * 5f, Vector3.down, out hit, 5f, _interactableShipObjectsMask, QueryTriggerInteraction.Ignore))
            {
                instrumentObj = default;
                return false;
            }

            instrumentObj = hit.collider.gameObject;
            if (instrumentObj == null)
            {
                return false;
            }

            var horn = instrumentObj.GetComponent<ShipAlarmCord>();
            if (horn != null)
            {
                return true;
            }

            return false;
        }

        private static void LetShowBegins(GameObject instrumentObj)
        {
            var horn = instrumentObj.GetComponent<ShipAlarmCord>();
            if (horn != null)
            {
                Instruments4MusicPlugin.LOGGER.LogInfo("Playing ShipAlarm");
                TuneAudioScript.RegisterInstrClip(instrumentObj, 8, horn.hornClose.clip);
                return;
            }
        }

        public void Awake()
        {
            SetupKeybindCallbacks();
        }

        public void SetupKeybindCallbacks()
        {
            Instruments4MusicPlugin.InputActionsInstance.Showtime.performed += OnShowtimePressed;
            Instruments4MusicPlugin.InputActionsInstance.CurtainCall.performed += OnCurtainCallPressed;
        }

        public void OnShowtimePressed(InputAction.CallbackContext showtimeContext)
        {
            if (TuneAudioScript.theShowIsOn) return;
            if (!showtimeContext.performed) return;
            if (!IsLookingAtInstrument(out var instrumentObj)) return;

            Instruments4MusicPlugin.LOGGER.LogInfo("It's show time!");
            LetShowBegins(instrumentObj);
        }

        public void OnCurtainCallPressed(InputAction.CallbackContext curtainCallContext)
        {
            if (!TuneAudioScript.theShowIsOn) return;
            if (!curtainCallContext.performed) return;

            Instruments4MusicPlugin.LOGGER.LogInfo("Maybe next time.");
            TuneAudioScript.DeactiveInstrument();
        }

        public void Update()
        {
            if (!TuneAudioScript.theShowIsOn)
            {
                if (IsLookingAtInstrument(out var instrumentObj))
                {
                    //Player.cursorIcon.enabled = true;
                    //Player.cursorIcon.sprite = Instruments4Music.HOVER_ICON;
                    //if (Instruments4Music.CONFIG_SHOW_TOOLTIP.Value)
                    //{
                        //Player.cursorTip.text = String.Format("[{0}] Showtime!",
                        //    Instruments4Music.InputActionsInstance.Showtime.bindings
                        //);
                    //}
                }
            }
        }
    }
}
