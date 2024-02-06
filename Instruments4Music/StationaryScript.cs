using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.InputSystem;

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
                TunedAudio.RegisterInstrClip(instrumentObj, 8, horn.hornClose.clip);
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
            if (!showtimeContext.performed) return;
            if (!IsLookingAtInstrument(out var instrumentObj)) return;

            LetShowBegins(instrumentObj);
            TunedAudio.DeactiveInstrument();
        }

        public void OnCurtainCallPressed(InputAction.CallbackContext curtainCallContext)
        {
            if (!curtainCallContext.performed) return;

            TunedAudio.DeactiveInstrument();
        }

        public void Update()
        {
            if (TunedAudio.theShowIsOn)
            {
                if (IsLookingAtInstrument(out var instrumentObj))
                {
                    Player.cursorIcon.enabled = true;
                    Player.cursorIcon.sprite = Instruments4MusicPlugin.HOVER_ICON;
                    if (Instruments4MusicPlugin.CONFIG_SHOW_TOOLTIP.Value)
                    {
                        //Player.cursorTip.text = String.Format("[{0}] Showtime!",
                        //    Instruments4MusicPlugin.InputActionsInstance.Showtime.bindings
                        //);
                    }
                }
            }
            else
            {
                if (Instruments4MusicPlugin.InputActionsInstance.HighCKey.triggered)
                {
                    TunedAudio.GetTunedAudio(0).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.HighDKey.triggered)
                {
                    TunedAudio.GetTunedAudio(2).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.HighEKey.triggered)
                {
                    TunedAudio.GetTunedAudio(4).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.HighFKey.triggered)
                {
                    TunedAudio.GetTunedAudio(5).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.HighGKey.triggered)
                {
                    TunedAudio.GetTunedAudio(7).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.HighAKey.triggered)
                {
                    TunedAudio.GetTunedAudio(9).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.HighBKey.triggered)
                {
                    TunedAudio.GetTunedAudio(11).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.MidCKey.triggered)
                {
                    TunedAudio.GetTunedAudio(12).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.MidDKey.triggered)
                {
                    TunedAudio.GetTunedAudio(14).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.MidEKey.triggered)
                {
                    TunedAudio.GetTunedAudio(16).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.MidFKey.triggered)
                {
                    TunedAudio.GetTunedAudio(17).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.MidGKey.triggered)
                {
                    TunedAudio.GetTunedAudio(19).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.MidAKey.triggered)
                {
                    TunedAudio.GetTunedAudio(21).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.MidBKey.triggered)
                {
                    TunedAudio.GetTunedAudio(23).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.LowCKey.triggered)
                {
                    TunedAudio.GetTunedAudio(24).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.LowDKey.triggered)
                {
                    TunedAudio.GetTunedAudio(26).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.LowEKey.triggered)
                {
                    TunedAudio.GetTunedAudio(28).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.LowFKey.triggered)
                {
                    TunedAudio.GetTunedAudio(29).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.LowGKey.triggered)
                {
                    TunedAudio.GetTunedAudio(31).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.LowAKey.triggered)
                {
                    TunedAudio.GetTunedAudio(33).Play();
                }
                if (Instruments4MusicPlugin.InputActionsInstance.LowBKey.triggered)
                {
                    TunedAudio.GetTunedAudio(35).Play();
                }
            }
        }
    }
}
