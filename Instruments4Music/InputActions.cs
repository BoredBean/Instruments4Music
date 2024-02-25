using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

namespace Instruments4Music
{
    public class InputActions : LcInputActions
    {
        [InputAction("<Keyboard>/backspace", Name = "Showtime!", ActionId = "Showtime", KbmInteractions = "hold(duration = 2)")]
        public InputAction? Showtime { get; set; }
        [InputAction("<Keyboard>/escape", Name = "Curtain Call", ActionId = "CurtainCall")]
        public InputAction? CurtainCall { get; set; }
        [InputAction("<Keyboard>/leftShift", Name = "Semitone", ActionId = "Semitone")]
        public InputAction? Semitone { get; set; }
        [InputAction("<Keyboard>/space", Name = "Sustain", ActionId = "Sustain")]
        public InputAction? Sustain { get; set; }
        [InputAction("<Keyboard>/leftCtrl", Name = "Soft", ActionId = "Soft")]
        public InputAction? Soft { get; set; }
        [InputAction("<Keyboard>/tab", Name = "Change Mode", ActionId = "ChangeMode")]
        public InputAction? ChangeMode { get; set; }
        [InputAction("<Keyboard>/enter", Name = "Input Note", ActionId = "InputNote")]
        public InputAction? InputNote { get; set; }
        [InputAction("<Keyboard>/q", Name = "High C", ActionId = "HighC")]
        public InputAction? HighCKey { get; set; }
        [InputAction("<Keyboard>/w", Name = "High D", ActionId = "HighD")]
        public InputAction? HighDKey { get; set; }
        [InputAction("<Keyboard>/e", Name = "High E", ActionId = "HighE")]
        public InputAction? HighEKey { get; set; }
        [InputAction("<Keyboard>/r", Name = "High F", ActionId = "HighF")]
        public InputAction? HighFKey { get; set; }
        [InputAction("<Keyboard>/t", Name = "High G", ActionId = "HighG")]
        public InputAction? HighGKey { get; set; }
        [InputAction("<Keyboard>/y", Name = "High A", ActionId = "HighA")]
        public InputAction? HighAKey { get; set; }
        [InputAction("<Keyboard>/u", Name = "High B", ActionId = "HighB")]
        public InputAction? HighBKey { get; set; }
        [InputAction("<Keyboard>/a", Name = "Mid C", ActionId = "MidC")]
        public InputAction? MidCKey { get; set; }
        [InputAction("<Keyboard>/s", Name = "Mid D", ActionId = "MidD")]
        public InputAction? MidDKey { get; set; }
        [InputAction("<Keyboard>/d", Name = "Mid E", ActionId = "MidE")]
        public InputAction? MidEKey { get; set; }
        [InputAction("<Keyboard>/f", Name = "Mid F", ActionId = "MidF")]
        public InputAction? MidFKey { get; set; }
        [InputAction("<Keyboard>/g", Name = "Mid G", ActionId = "MidG")]
        public InputAction? MidGKey { get; set; }
        [InputAction("<Keyboard>/h", Name = "Mid A", ActionId = "MidA")]
        public InputAction? MidAKey { get; set; }
        [InputAction("<Keyboard>/j", Name = "Mid B", ActionId = "MidB")]
        public InputAction? MidBKey { get; set; }
        [InputAction("<Keyboard>/z", Name = "Low C", ActionId = "LowC")]
        public InputAction? LowCKey { get; set; }
        [InputAction("<Keyboard>/x", Name = "Low D", ActionId = "LowD")]
        public InputAction? LowDKey { get; set; }
        [InputAction("<Keyboard>/c", Name = "Low E", ActionId = "LowE")]
        public InputAction? LowEKey { get; set; }
        [InputAction("<Keyboard>/v", Name = "Low F", ActionId = "LowF")]
        public InputAction? LowFKey { get; set; }
        [InputAction("<Keyboard>/b", Name = "Low G", ActionId = "LowG")]
        public InputAction? LowGKey { get; set; }
        [InputAction("<Keyboard>/n", Name = "Low A", ActionId = "LowA")]
        public InputAction? LowAKey { get; set; }
        [InputAction("<Keyboard>/m", Name = "Low B", ActionId = "LowB")]
        public InputAction? LowBKey { get; set; }

        //secondary plan
        [InputAction("<Keyboard>/y", Name = "Low C2", ActionId = "LowC2")]
        public InputAction? LowCKey2 { get; set; }
        [InputAction("<Keyboard>/u", Name = "Low D2", ActionId = "LowD2")]
        public InputAction? LowDKey2 { get; set; }
        [InputAction("<Keyboard>/i", Name = "Low E2", ActionId = "LowE2")]
        public InputAction? LowEKey2 { get; set; }
        [InputAction("<Keyboard>/o", Name = "Low F2", ActionId = "LowF2")]
        public InputAction? LowFKey2 { get; set; }
        [InputAction("<Keyboard>/p", Name = "Low G2", ActionId = "LowG2")]
        public InputAction? LowGKey2 { get; set; }
        [InputAction("<Keyboard>/h", Name = "Low A2", ActionId = "LowA2")]
        public InputAction? LowAKey2 { get; set; }
        [InputAction("<Keyboard>/j", Name = "Low B2", ActionId = "LowB2")]
        public InputAction? LowBKey2 { get; set; }
        [InputAction("<Keyboard>/k", Name = "Mid C2", ActionId = "MidC2")]
        public InputAction? MidCKey2 { get; set; }
        [InputAction("<Keyboard>/l", Name = "Mid D2", ActionId = "MidD2")]
        public InputAction? MidDKey2 { get; set; }
        [InputAction("<Keyboard>/semicolon", Name = "Mid E2", ActionId = "MidE2")]
        public InputAction? MidEKey2 { get; set; }
        [InputAction("<Keyboard>/n", Name = "Mid F2", ActionId = "MidF2")]
        public InputAction? MidFKey2 { get; set; }
        [InputAction("<Keyboard>/m", Name = "Mid G2", ActionId = "MidG2")]
        public InputAction? MidGKey2 { get; set; }
        [InputAction("<Keyboard>/comma", Name = "Mid A2", ActionId = "MidA2")]
        public InputAction? MidAKey2 { get; set; }
        [InputAction("<Keyboard>/period", Name = "Mid B2", ActionId = "MidB2")]
        public InputAction? MidBKey2 { get; set; }
        [InputAction("<Keyboard>/slash", Name = "High C2", ActionId = "HighC2")]
        public InputAction? HighCKey2 { get; set; }

        public static string GetButtonDescription(InputAction action)
        {
            var isController = StartOfRound.Instance.localPlayerUsingController;
            InputBinding? binding = null;
            foreach (var x in action.bindings)
            {
                if (isController ||
                    (!x.effectivePath.StartsWith("<Keyboard>") && !x.effectivePath.StartsWith("<Mouse>"))) return "";
                binding = x;
                break;
            }

            var path = binding != null ? binding.Value.effectivePath : "";
            string[] splits = path.Split("/");
            return (splits.Length > 1 ? path : "") switch
            {
                // Mouse
                "<Mouse>/leftButton" => "LΜB",  // Uses 'Greek Capital Letter Mu' for M
                "<Mouse>/rightButton" => "RΜB", // Uses 'Greek Capital Letter Mu' for M
                // Keyboard
                "<Keyboard>/escape" => "ESC",
                "<Keyboard>/semicolon" => ";",
                "<Keyboard>/comma" => ",",
                "<Keyboard>/period" => ".",
                "<Keyboard>/slash" => "/",
                _ => splits.Length > 1 ? splits[1][..1].ToUpper() + splits[1][1..] : "?"
            };
        }
    }
}