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
    //public UIObject UserNamePanel;
    public UIObject<TextMeshProUGUI> UserNameText = new();
    public UIObject<Image> ContentBackground = new();
    public UIObject<TextMeshProUGUI> ContentText = new();
    public UIObject<TextMeshProUGUI> TimeText = new();
}

public class ChatLeftItemUI : BaseUIItem
{
    [SerializeField]
    ChatLeftItemUIContext Context = new();

    const float ContentLRPadding = 10f; // 5 each
    const float ContentUDPadding = 10f; // 5 each
    const float TimePosPadding = 3f; // padding value beside the contentbackground

    public UIObject<TextMeshProUGUI> UserNameText => Context.UserNameText;

    // not changable

    public override void Init()
    {
        base.Init();

    }

    //public void SetMessage(string msg, DateTime time)
    //{
    //    Context.ContentText.Component.text = msg;
    //    Context.TimeText.Component.text = GetTimeOrYesterday(time);

    //    FitConent();
    //}

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
        float contentWidth = Context.ContentText.BindObject.GetComponent<RectTransform>().rect.width;
        float contentHeight = Context.ContentText.BindObject.GetComponent<RectTransform>().rect.height;

        Context.ContentBackground.BindObject.GetComponent<RectTransform>().sizeDelta 
            = new Vector2 (contentWidth + ContentLRPadding, contentHeight + ContentUDPadding);

        Context.TimeText.BindObject.GetComponent<RectTransform>().sizeDelta
            = new Vector2(contentHeight + ContentLRPadding + TimePosPadding, contentHeight + ContentUDPadding);
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
