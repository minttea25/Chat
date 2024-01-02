using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class ChatRightItemUIContext : UIContext
{
    public UIObject<Image> ContentBackground = new();
    public UIObject<TextMeshProUGUI> ContentText = new();
    public UIObject<TextMeshProUGUI> TimeText = new();
}

public class ChatRightItemUI : BaseUIItem
{
    [SerializeField]
    ChatRightItemUIContext Context = new();

    const float ContentLRPadding = 10f; // 5 each
    const float ContentUDPadding = 10f; // 5 each
    const float TimePosPadding = 3f; // padding value beside the contentbackground


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

    public void SetMessage(string msg, string time)
    {
        Context.ContentText.Component.text = msg;
        Context.TimeText.Component.text = time;

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
            = new Vector2(contentWidth + ContentLRPadding, contentHeight + ContentUDPadding);

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
        return typeof(ChatRightItemUIContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
