using Instruments4Music;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicHUD : MonoBehaviour
{
    public static MusicHUD instance;

    List<Image> frameRectangles = [];
    List<Image> textRectangles = [];
    static List<Text> buttonTips = [];
    static List<Image> menuRectangles = [];
    static List<Text> menuTips = [];

    private InputField inputField;
    private bool isInputing = false;

    private void Awake()
    {
        if (instance != null)
            throw new System.Exception("More than one instance of UI!");
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject hud = Instruments4MusicPlugin.instance.hudInstance;
        Button[] buttons = hud.GetComponentsInChildren<Button>();
        Image[] frameImages = Instruments4MusicPlugin.instance.hudManager.itemSlotIconFrames;
        foreach (Button button in buttons)
        {
            RectTransform buttonRectTransform = button.GetComponent<RectTransform>();

            int randomIndex = Random.Range(0, frameImages.Length);
            Image imageToDisplay = frameImages[randomIndex];

            GameObject frameRectangle = new GameObject("FrameRectangle");
            frameRectangle.transform.SetParent(button.transform, false);

            Image frameImage = frameRectangle.AddComponent<Image>();
            frameImage.color = new Color32(0xFE, 0x49, 0x00, 0xFF);
            frameImage.sprite = imageToDisplay.sprite;

            RectTransform frameRectTransform = frameRectangle.GetComponent<RectTransform>();
            frameRectTransform.anchorMin = new Vector2(0, 0);
            frameRectTransform.anchorMax = new Vector2(1, 1);
            frameRectTransform.sizeDelta = new Vector2(0, 0);
            frameRectTransform.anchoredPosition = new Vector2(0, 0);

            frameRectangles.Add(frameImage);

            GameObject textRectangle = new GameObject("TextRectangle");
            textRectangle.transform.SetParent(button.transform, false);

            Image textImage = textRectangle.AddComponent<Image>();
            textImage.color = new Color32(0xFE, 0x49, 0x00, 0x80);

            RectTransform textRectTransform = textRectangle.GetComponent<RectTransform>();

            textRectTransform.anchorMin = new Vector2(0.3f, 0.3f);
            textRectTransform.anchorMax = new Vector2(0.7f, 0.7f);
            textRectTransform.offsetMin = new Vector2(0, 0);
            textRectTransform.offsetMax = new Vector2(0, 0);
            textRectTransform.anchoredPosition = new Vector2(0, -0.5f * buttonRectTransform.rect.height);

            textRectangles.Add(textImage);

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

            buttonTips.Add(tipText);
        }

        RectTransform parentRectTransform = hud.GetComponent<RectTransform>();

        for (int i = 0; i < 5; i++)
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

            menuRectangles.Add(menuImage);

            GameObject menuTip = new GameObject("MenuTip");
            menuTip.transform.SetParent(menuRectangle.transform, false);

            Text menuText = menuTip.AddComponent<Text>();
            menuText.fontSize = (int)(0.04f * parentRectTransform.rect.height);
            menuText.text = "Text";
            menuText.color = new Color(1, 1, 1, 0.5f);
            menuText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            menuText.alignment = TextAnchor.MiddleCenter;

            RectTransform menuTipRectTransform = menuTip.GetComponent<RectTransform>();
            menuTipRectTransform.anchorMin = new Vector2(0, 0);
            menuTipRectTransform.anchorMax = new Vector2(1, 1);
            menuTipRectTransform.sizeDelta = new Vector2(0, 0);
            menuTipRectTransform.anchoredPosition = new Vector2(0, 0);

            menuTips.Add(menuText);
        }

        GameObject inputFieldObject = new GameObject("InputField");

        inputFieldObject.transform.SetParent(hud.transform, false);

        Image borderImage = inputFieldObject.AddComponent<Image>();
        borderImage.color = new Color32(0xFE, 0x49, 0x00, 0x50);

        RectTransform inputRectTransform = inputFieldObject.GetComponent<RectTransform>();
        inputRectTransform.anchorMin = new Vector2(0.2f, 0.46f);
        inputRectTransform.anchorMax = new Vector2(0.8f, 0.54f);
        inputRectTransform.offsetMin = new Vector2(0, 0);
        inputRectTransform.offsetMax = new Vector2(0, 0);
        inputRectTransform.anchoredPosition = new Vector2(0f, 0.4f * parentRectTransform.rect.height);

        inputField = inputFieldObject.AddComponent<InputField>();

        GameObject textObject = new GameObject("InputText");
        textObject.transform.SetParent(inputFieldObject.transform, false);

        Text inputText = textObject.AddComponent<Text>();
        inputField.text = "Press Enter to input music note...";
        inputText.color = Color.white;
        inputText.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); 
        
        RectTransform inputTextRectTransform = textObject.GetComponent<RectTransform>();

        inputTextRectTransform.anchorMin = new Vector2(0, 0);
        inputTextRectTransform.anchorMax = new Vector2(1, 1);
        inputTextRectTransform.offsetMin = new Vector2(10, 10);
        inputTextRectTransform.offsetMax = new Vector2(-10, -10);

        inputField.textComponent = inputText;
    }

    public static void ShowUserInterface()
    {
        UpdateButtonTips();
        menuTips[0].text =
            $"Quit: {InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.CurtainCall)}";
        menuTips[1].text =
            $"Soft: {InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.Soft)}";
        menuTips[2].text =
            $"Semi: {InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.Semitone)}";
        menuTips[3].text =
            $"Sustain: {InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.Sustain)}";
        menuTips[4].text =
            $"Switch: {InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.ChangeMode)}";

        Instruments4MusicPlugin.instance.hudManager.PingHUDElement(Instruments4MusicPlugin.instance.hudElement, 0f, 0f,
            1f);
    }

    public static void UpdateButtonTips()
    {
        string[] tipStrings;
        if (!TuneAudioScript.secondaryKeyBind)
        {
            tipStrings =
            [
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowCKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowDKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowEKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowFKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowGKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowAKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowBKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidCKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidDKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidEKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidFKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidGKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidAKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidBKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.HighCKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.HighDKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.HighEKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.HighFKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.HighGKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.HighAKey),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.HighBKey)
            ];
        }
        else
        {
            tipStrings =
            [
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowCKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowDKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowEKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowFKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowGKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowAKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.LowBKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidCKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidDKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidEKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidFKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidGKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidAKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.MidBKey2),
                InputActions.GetButtonDescription(Instruments4MusicPlugin.inputActionsInstance.HighCKey2),
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
            buttonTips[i].text = tipStrings[i];
        }
    }

    public static void HideUserInterface()
    {
        Instruments4MusicPlugin.instance.hudManager.PingHUDElement(Instruments4MusicPlugin.instance.hudElement, 0f, 1f,
            0f);
    }

    private Dictionary<int, IEnumerator> currentCoroutines = [];
    private Dictionary<int, bool> rotateFinished = [];

    IEnumerator RotateOverTime(int noteNumber, RectTransform rectTransform)
    {
        Image textImage = textRectangles[noteNumber];
        Color textImageColor = textImage.color;
        textImageColor.a = 1f;
        textImage.color = textImageColor;

        var endRotation = 180f;
        var originRotation = 0f;
        var speed = 360f;

        while (rectTransform.rotation.eulerAngles.y < endRotation)
        {
            float step = speed * Time.deltaTime;
            float newAngle = Mathf.Min(rectTransform.rotation.eulerAngles.y + step, endRotation);
            rectTransform.rotation =
                Quaternion.Euler(rectTransform.eulerAngles.x, newAngle, rectTransform.eulerAngles.z);
            yield return null;
        }

        textImageColor.a = 0.5f;
        textImage.color = textImageColor;
        rotateFinished[noteNumber] = true;

        while (rectTransform.rotation.eulerAngles.y > originRotation)
        {
            float step = speed * Time.deltaTime;
            float newAngle = Mathf.Max(rectTransform.rotation.eulerAngles.y - step, originRotation);
            rectTransform.rotation =
                Quaternion.Euler(rectTransform.eulerAngles.x, newAngle, rectTransform.eulerAngles.z);
            yield return null;
        }
    }

    public void OnButtonClicked(int num)
    {
        var noteNumber = NoteNumberConvert(num);
        if (rotateFinished.ContainsKey(noteNumber) && !rotateFinished[noteNumber]) return;
        if (currentCoroutines.TryGetValue(noteNumber, out var currentCoroutine))
        {
            StopCoroutine(currentCoroutine);
        }

        rotateFinished[noteNumber] = false;
        currentCoroutines[noteNumber] = RotateOverTime(noteNumber, frameRectangles[noteNumber].rectTransform);
        StartCoroutine(currentCoroutines[noteNumber]);
    }

    public void OnMenuClicked(int num, bool isHolding)
    {
        var newColor = isHolding ? 1f : 0f;

        Color32 color = menuRectangles[num].color;
        color.a = (byte)(newColor * 255);
        menuRectangles[num].color = color;
    }


    int NoteNumberConvert(int num)
    {
        int[] map = [0, 0, 1, 1, 2, 3, 3, 4, 4, 5, 5, 6];

        return num / 12 * 7 + map[num % 12];
    }

    public void TriggerInputNote()
    {
        if (isInputing)
        {
            inputField.DeactivateInputField();
            isInputing = false;
            Instruments4MusicPlugin.AddLog($"Input stop, get note: {inputField.text}.");
        }
        else
        {
            Instruments4MusicPlugin.AddLog("Input start.");
            inputField.ActivateInputField();
            isInputing = true;
        }
    }
}
