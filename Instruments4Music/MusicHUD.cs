using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Instruments4Music
{
    public class MusicHud : MonoBehaviour
    {
        public static MusicHud? Instance;

        private readonly List<Image> _frameRectangles = [];
        private readonly List<Image> _textRectangles = [];
        private static readonly List<Text> ButtonTips = [];
        private static readonly List<Image> MenuRectangles = [];
        private static readonly List<Text> MenuTips = [];

        private InputField inputField;
        public bool IsInputing = false;

        private void Awake()
        {
            if (Instance != null)
                throw new System.Exception("More than one instance of UI!");
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            var transparency = (InstrumentsConfig.ConfigHudTransparency == null)
                ? 1.0f
                : InstrumentsConfig.ConfigHudTransparency.Value;
            var hud = Instruments4MusicPlugin.Instance.HudInstance;
            var buttons = hud.GetComponentsInChildren<Button>();
            Image[] frameImages = Instruments4MusicPlugin.Instance.HudManager.itemSlotIconFrames;
            Instruments4MusicPlugin.AddLog("Draw MusicHUD");
            _frameRectangles.Clear();
            _textRectangles.Clear();
            ButtonTips.Clear();
            MenuRectangles.Clear();
            MenuTips.Clear();
            foreach (var button in buttons)
            {
                RectTransform buttonRectTransform = button.GetComponent<RectTransform>();

                int randomIndex = UnityEngine.Random.Range(0, frameImages.Length);
                Image imageToDisplay = frameImages[randomIndex];

                GameObject frameRectangle = new GameObject("FrameRectangle");
                frameRectangle.transform.SetParent(button.transform, false);

                Image frameImage = frameRectangle.AddComponent<Image>();
                frameImage.color = new Color32(0xFE, 0x49, 0x00, (byte)Math.Round(transparency * 0xFF));
                frameImage.sprite = imageToDisplay.sprite;

                RectTransform frameRectTransform = frameRectangle.GetComponent<RectTransform>();
                frameRectTransform.anchorMin = new Vector2(0, 0);
                frameRectTransform.anchorMax = new Vector2(1, 1);
                frameRectTransform.sizeDelta = new Vector2(0, 0);
                frameRectTransform.anchoredPosition = new Vector2(0, 0);

                _frameRectangles.Add(frameImage);

                GameObject textRectangle = new GameObject("TextRectangle");
                textRectangle.transform.SetParent(button.transform, false);

                Image textImage = textRectangle.AddComponent<Image>();
                textImage.color = new Color32(0xFE, 0x49, 0x00, (byte)Math.Round(transparency * 0xFF));

                RectTransform textRectTransform = textRectangle.GetComponent<RectTransform>();

                textRectTransform.anchorMin = new Vector2(0.3f, 0.3f);
                textRectTransform.anchorMax = new Vector2(0.7f, 0.7f);
                textRectTransform.offsetMin = new Vector2(0, 0);
                textRectTransform.offsetMax = new Vector2(0, 0);
                textRectTransform.anchoredPosition = new Vector2(0, -0.5f * buttonRectTransform.rect.height);

                _textRectangles.Add(textImage);

                GameObject buttonTip = new GameObject("ButtonTip");
                buttonTip.transform.SetParent(textRectangle.transform, false);


                Text tipText = buttonTip.AddComponent<Text>();
                tipText.fontSize = (int)(0.8f * textRectTransform.rect.height);
                tipText.text = "A";
                tipText.color = Color.white;
                tipText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                tipText.alignment = TextAnchor.MiddleCenter;
                tipText.fontStyle = FontStyle.Bold;

                RectTransform tipRectTransform = buttonTip.GetComponent<RectTransform>();
                tipRectTransform.anchorMin = new Vector2(0, 0);
                tipRectTransform.anchorMax = new Vector2(1, 1);
                tipRectTransform.sizeDelta = new Vector2(0, 0);
                tipRectTransform.anchoredPosition = new Vector2(0, 0);

                ButtonTips.Add(tipText);
            }

            var parentRectTransform = hud.GetComponent<RectTransform>();

            for (var i = 0; i < 5; i++)
            {
                GameObject menuRectangle = new GameObject("MenuRectangle");
                menuRectangle.transform.SetParent(hud.transform, false);

                Image menuImage = menuRectangle.AddComponent<Image>();
                menuImage.color = new Color32(0xFF, 0xFF, 0xFF, 0x00);

                RectTransform menuRectTransform = menuRectangle.GetComponent<RectTransform>();

                menuRectTransform.anchorMin = new Vector2(0.41f, 0.46f);
                menuRectTransform.anchorMax = new Vector2(0.59f, 0.54f);
                menuRectTransform.offsetMin = new Vector2(0, 0);
                menuRectTransform.offsetMax = new Vector2(0, 0);
                menuRectTransform.anchoredPosition = new Vector2((i - 2) * 0.2f * parentRectTransform.rect.width,
                    -0.4f * parentRectTransform.rect.height);

                MenuRectangles.Add(menuImage);

                GameObject menuTip = new GameObject("MenuTip");
                menuTip.transform.SetParent(menuRectangle.transform, false);

                Text menuText = menuTip.AddComponent<Text>();
                menuText.fontSize = (int)(0.04f * parentRectTransform.rect.height);
                menuText.text = "Text";
                menuText.color = new Color(1, 1, 1, transparency);
                menuText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                menuText.alignment = TextAnchor.MiddleCenter;

                RectTransform menuTipRectTransform = menuTip.GetComponent<RectTransform>();
                menuTipRectTransform.anchorMin = new Vector2(0, 0);
                menuTipRectTransform.anchorMax = new Vector2(1, 1);
                menuTipRectTransform.sizeDelta = new Vector2(0, 0);
                menuTipRectTransform.anchoredPosition = new Vector2(0, 0);

                MenuTips.Add(menuText);
            }

            var inputFieldObject = new GameObject("InputField");

            inputFieldObject.transform.SetParent(hud.transform, false);

            var borderImage = inputFieldObject.AddComponent<Image>();
            borderImage.color = new Color32(0xFE, 0x49, 0x00, (byte)Math.Round(transparency * 0xFF));

            var inputRectTransform = inputFieldObject.GetComponent<RectTransform>();
            inputRectTransform.anchorMin = new Vector2(0.2f, 0.46f);
            inputRectTransform.anchorMax = new Vector2(0.8f, 0.54f);
            inputRectTransform.offsetMin = new Vector2(0, 0);
            inputRectTransform.offsetMax = new Vector2(0, 0);
            inputRectTransform.anchoredPosition = new Vector2(0f, 0.4f * parentRectTransform.rect.height);

            inputField = inputFieldObject.AddComponent<InputField>();

            var textObject = new GameObject("InputText");
            textObject.transform.SetParent(inputFieldObject.transform, false);

            var inputText = textObject.AddComponent<Text>();
            inputField.text = "Press Enter to input music note...";
            inputText.color = Color.white;
            inputText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            var inputTextRectTransform = textObject.GetComponent<RectTransform>();

            inputTextRectTransform.anchorMin = new Vector2(0, 0);
            inputTextRectTransform.anchorMax = new Vector2(1, 1);
            inputTextRectTransform.offsetMin = new Vector2(10, 10);
            inputTextRectTransform.offsetMax = new Vector2(-10, -10);

            inputField.textComponent = inputText;
            Instruments4MusicPlugin.AddLog("Draw MusicHUD success");
        }

        public static void ShowUserInterface()
        {
            var transparency = InstrumentsConfig.ConfigHudTransparency.Value;
            UpdateButtonTips();
            MenuTips[0].text =
                $"Quit: {InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.CurtainCall)}";
            MenuTips[1].text =
                $"Soft: {InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.Soft)}";
            MenuTips[2].text =
                $"Semi: {InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.Semitone)}";
            MenuTips[3].text =
                $"Sustain: {InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.Sustain)}";
            MenuTips[4].text =
                $"Switch: {InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.ChangeMode)}";

            Instruments4MusicPlugin.Instance.HudManager.PingHUDElement(Instruments4MusicPlugin.Instance.HudElement, 0f,
                0f,
                transparency);
        }

        public static void UpdateButtonTips()
        {
            string[] tipStrings;
            Instruments4MusicPlugin.AddLog("Load key binds");
            if (!TuneAudioScript.SecondaryKeyBind)
            {
                tipStrings =
                [
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowCKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowDKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowEKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowFKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowGKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowAKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowBKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidCKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidDKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidEKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidFKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidGKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidAKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidBKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.HighCKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.HighDKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.HighEKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.HighFKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.HighGKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.HighAKey),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.HighBKey)
                ];
            }
            else
            {
                tipStrings =
                [
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowCKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowDKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowEKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowFKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowGKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowAKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.LowBKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidCKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidDKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidEKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidFKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidGKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidAKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.MidBKey2),
                    InputActions.GetButtonDescription(Instruments4MusicPlugin.InputActionsInstance.HighCKey2),
                    "",
                    "",
                    "",
                    "",
                    "",
                    ""
                ];
            }

            for (int i = 0; i < tipStrings.Length; i++)
            {
                ButtonTips[i].text = tipStrings[i];
            }

            Instruments4MusicPlugin.AddLog("Load key binds success");
        }

        public static void HideUserInterface()
        {
            Instruments4MusicPlugin.Instance.HudManager.PingHUDElement(Instruments4MusicPlugin.Instance.HudElement, 0f,
                1f,
                0f);
        }

        public Dictionary<int, IEnumerator> CurrentCoroutines = [];
        private readonly Dictionary<int, bool> _rotateFinished = [];

        private IEnumerator RotateOverTime(int noteNumber, RectTransform rectTransform)
        {
            var textImage = _textRectangles[noteNumber];
            var textImageColor = textImage.color;
            textImageColor.a = 1f;
            textImage.color = textImageColor;

            const float endRotation = 180f;
            const float originRotation = 0f;
            const float speed = 360f;

            while (rectTransform.rotation.eulerAngles.y < endRotation)
            {
                var step = speed * Time.deltaTime;
                var newAngle = Mathf.Min(rectTransform.rotation.eulerAngles.y + step, endRotation);
                rectTransform.rotation =
                    Quaternion.Euler(rectTransform.eulerAngles.x, newAngle, rectTransform.eulerAngles.z);
                yield return null;
            }

            textImageColor.a = 0.5f;
            textImage.color = textImageColor;
            _rotateFinished[noteNumber] = true;

            while (rectTransform.rotation.eulerAngles.y > originRotation)
            {
                var step = speed * Time.deltaTime;
                var newAngle = Mathf.Max(rectTransform.rotation.eulerAngles.y - step, originRotation);
                rectTransform.rotation =
                    Quaternion.Euler(rectTransform.eulerAngles.x, newAngle, rectTransform.eulerAngles.z);
                yield return null;
            }
        }

        public void OnButtonClicked(int num)
        {
            var noteNumber = NoteNumberConvert(num);
            if (_rotateFinished.ContainsKey(noteNumber) && !_rotateFinished[noteNumber]) return;
            if (CurrentCoroutines.TryGetValue(noteNumber, out var currentCoroutine))
            {
                StopCoroutine(currentCoroutine);
            }

            _rotateFinished[noteNumber] = false;
            CurrentCoroutines[noteNumber] = RotateOverTime(noteNumber, _frameRectangles[noteNumber].rectTransform);
            StartCoroutine(CurrentCoroutines[noteNumber]);
        }

        public void OnMenuClicked(int num, bool isHolding)
        {
            var newColor = isHolding ? 1f : 0f;

            Color32 color = MenuRectangles[num].color;
            color.a = (byte)(newColor * 255);
            MenuRectangles[num].color = color;
        }


        private static int NoteNumberConvert(int num)
        {
            int[] map = [0, 0, 1, 1, 2, 3, 3, 4, 4, 5, 5, 6];

            return num / 12 * 7 + map[num % 12];
        }

        public void TriggerInputNote()
        {
            if (IsInputing)
            {
                inputField.DeactivateInputField();
                IsInputing = false;
                Instruments4MusicPlugin.AddLog($"Input stop, get note: {inputField.text}.");
                TuneAudioScript.StartAutoPlay(inputField.text);
            }
            else
            {
                Instruments4MusicPlugin.AddLog("Input start.");
                inputField.ActivateInputField();
                IsInputing = true;
            }
        }
    }
}