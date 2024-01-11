using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
class SimplePopupUIContext : UIContext
{
    public UIObject<TextMeshProUGUI> TitleText = new();

    public UIObject<TextMeshProUGUI> OkText1 = new();
    public UIObject<TextMeshProUGUI> OkText2 = new();
    public UIObject<TextMeshProUGUI> OkText3 = new();

    public UIObject<Button> OkButton1 = new();
    public UIObject<Button> OkButton2 = new();
    public UIObject<Button> OkButton3 = new();
}

public class SimplePopupUI : BaseUIPopup
{
    [SerializeField]
    SimplePopupUIContext Context = new();

    public override void Init()
    {
        base.Init();

        Context.OkButton1.BindObject.SetActive(false);
        Context.OkButton2.BindObject.SetActive(false);
        Context.OkButton3.BindObject.SetActive(false);
    }

    public void Setup(string content, int numberOfButtons, string[] buttonTexts, UnityAction[] buttonCallbacks)
    {
#if UNITY_EDITOR
        Core.Utils.Assert(numberOfButtons <= 3 && numberOfButtons >= 0, "Number of buttons is 0 to 3. Exceeing buttons are ignored.");
        Core.Utils.AssertCrash(numberOfButtons == buttonTexts.Length, "Number of buttons must be same as number of buttonTexts options");
        Core.Utils.AssertCrash(numberOfButtons == buttonCallbacks.Length, "Number of buttons must be same as number of buttonCallback options");
#endif

        Context.TitleText.Component.text = content;

        if (numberOfButtons == 1)
        {
            Setup(content, buttonTexts[0], buttonCallbacks[0]);
            return;
        }

        Context.OkText1.Component.text = buttonTexts[0];
        Context.OkText2.Component.text = buttonTexts[1];

        Context.OkButton1.Component.onClick.AddListener(buttonCallbacks[0] ?? Close);
        Context.OkButton2.Component.onClick.AddListener(buttonCallbacks[1] ?? Close);

        Context.OkButton1.BindObject.SetActive(true);
        Context.OkButton2.BindObject.SetActive(true);

        if (numberOfButtons == 3)
        {
            Context.OkText3.Component.text = buttonTexts[2];
            Context.OkButton3.Component.onClick.AddListener(buttonCallbacks[2] ?? Close);
            Context.OkButton3.BindObject.SetActive(true);
        }
    }

    /// <summary>
    /// If okText is null, the default string would be 'Ok' in set language; If okCallback is null, the default would be 'ClosePopupUI()'.
    /// </summary>
    /// <param name="content">the message of popup</param>
    /// <param name="okText">the text of ok-button</param>
    /// <param name="okCallback">UnityAction for callback of ok-button</param>
    public void Setup(string content, string okText = null, UnityAction okCallback = null)
    {
        Context.TitleText.Component.text = content;

        Context.OkButton1.BindObject.SetActive(true);
        Context.OkButton2.BindObject.SetActive(false);
        Context.OkButton3.BindObject.SetActive(false);

        Context.OkText1.Component.text = okText ?? "OK";
        Context.OkButton1.Component.onClick.AddListener(okCallback ?? Close);
    }

#if UNITY_EDITOR
    public override Type GetContextType()
    {
        return typeof(SimplePopupUIContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
