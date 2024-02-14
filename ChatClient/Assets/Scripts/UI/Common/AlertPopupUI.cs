using Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
class AlertPopupUIContext : UIContext
{
    public UIObject<TextMeshProUGUI> TitleText = new();
    public UIObject<Button> OkButton = new();
    public UIObject<TextMeshProUGUI> OkText = new();
}

public class AlertPopupUI : BaseUIPopup
{
    [SerializeField]
    AlertPopupUIContext Context = new();


    public void Setup(string title, string okText, UnityAction buttonCallback = null)
    {
        Context.TitleText.Component.text = title;
        Context.OkButton.Component.onClick.AddListener(buttonCallback ?? Close);
        Context.OkText.Component.text = okText;
    }

#if UNITY_EDITOR
    public override Type GetContextType()
    {
        return typeof(AlertPopupUIContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
