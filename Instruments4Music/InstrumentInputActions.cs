using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace Instruments4Music
{
    public class InstrumentInputActions : LcInputActions
    {
        [InputAction("<Keyboard>/alt", Name = "Showtime!", ActionId = "Showtime", KbmInteractions = "hold(duration = 2)")]
        public InputAction? Showtime { get; set; }
        [InputAction("<Keyboard>/esc", Name = "Curtain Call", ActionId = "CurtainCall")]
        public InputAction? CurtainCall { get; set; }
        [InputAction("<Keyboard>/shift", Name = "Semitone", ActionId = "Semitone")]
        public InputAction? Semitone { get; set; }
        [InputAction("<Keyboard>/space", Name = "Sustain", ActionId = "Sustain")]
        public InputAction? Sustain { get; set; }
        [InputAction("<Keyboard>/ctrl", Name = "Soft", ActionId = "Soft")]
        public InputAction? Soft { get; set; }
        [InputAction("<Keyboard>/tab", Name = "Change Mode", ActionId = "ChangeMode")]
        public InputAction? ChangeMode { get; set; }
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
    }
}