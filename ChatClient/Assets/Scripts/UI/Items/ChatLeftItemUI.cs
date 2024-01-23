using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class ChatLeftItemUIContext : UIContext
{
    public UIObject UserNamePanel = new();
    public UIObject<TextMeshProUGUI> UserNameText = new();
    public UIObject<Image> ContentBackground = new();
    public UIObject<TextMeshProUGUI> ContentText = new();
    public UIObject<TextMeshProUGUI> TimeText = new();
}

public class ChatLeftItemUI : BaseUIItem
{
    [SerializeField]
    ChatLeftItemUIContext Context = new();


    public UIObject<TextMeshProUGUI> UserNameText => Context.UserNameText;

    public override void Init()
    {
        base.Init();

    }

    public void SetMessage(string msg, string time, string userName = null)
    {
        Context.ContentText.Component.text = msg;
        Context.TimeText.Component.text = time;

        if (userName != null)
        {
            Context.UserNameText.Component.text = userName;
        }

        FitConent();
    }

    public void SetYesterDayTime()
    {
        Context.TimeText.Component.text = "Yesterday";
    }

    void FitConent()
    {
        float contentWidth = Context.ContentText.Component.preferredWidth;
        float contentHeight = Context.ContentText.Component.preferredHeight;

        Context.ContentBackground.BindObject.GetComponent<RectTransform>().sizeDelta 
            = new Vector2 (contentWidth + UIValues.ContentLRPadding, contentHeight + UIValues.ContentUDPadding);

        Context.TimeText.BindObject.GetComponent<RectTransform>().anchoredPosition
                    = new Vector2(contentWidth + UIValues.ContentLRPadding + UIValues.TimePosPadding, Context.ContentBackground.BindObject.GetComponent<RectTransform>().anchoredPosition.y);
        Context.TimeText.BindObject.GetComponent<RectTransform>().sizeDelta
            = new Vector2(Context.TimeText.BindObject.GetComponent<RectTransform>().sizeDelta.x, contentHeight + UIValues.ContentUDPadding);

        var width = GetComponent<RectTransform>().sizeDelta.x;
        var height = Context.ContentBackground.BindObject.GetComponent<RectTransform>().sizeDelta.y + Context.UserNamePanel.BindObject.GetComponent<RectTransform>().sizeDelta.y + UIValues.ContentHeightBottomPadding;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
    }

    string GetTimeOrYesterday(DateTime time)
    {
        if (time.Date < DateTime.Now.Date)
        {
            return "Yesterday";
        }
        else
        {
            return time.ToString("HH:mm");
        }
    }

#if UNITY_EDITOR

    public override Type GetContextType()
    {
        return typeof(ChatLeftItemUIContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
