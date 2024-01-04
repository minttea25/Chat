using Core;
using DG.Tweening;
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
    public UIObject StatePanel = new();
    public UIObject CheckImage = new();
    public UIObject<Image> LoadingImage = new();
    public UIObject FailImage = new();
}

public class ChatRightItemUI : BaseUIItem
{
    [SerializeField]
    ChatRightItemUIContext Context = new();

    Tweener sendChecking = null;


    public override void Init()
    {
        base.Init();

        Context.CheckImage.BindObject.SetActive(false);
        Context.FailImage.BindObject.SetActive(false);
        Context.LoadingImage.BindObject.SetActive(true);

        sendChecking = Context.LoadingImage.Component.rectTransform
            .DORotate(new Vector3(0f, 0f, -360f), UIValues.SendCheckingTimeOut, RotateMode.FastBeyond360);
        sendChecking.onComplete += () => SetSuccess(false);
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

    public void SetSuccess(bool success)
    {
        sendChecking.Pause();
        Destroy(Context.LoadingImage.BindObject);

        if (success == true) SetChecked();
        else SetFailed();
    }

    void FitConent()
    {
        float contentWidth = Context.ContentText.Component.preferredWidth;
        float contentHeight = Context.ContentText.Component.preferredHeight;

        Context.ContentBackground.BindObject.GetComponent<RectTransform>().sizeDelta
            = new Vector2(contentWidth + UIValues.ContentLRPadding, contentHeight + UIValues.ContentUDPadding);

        Context.TimeText.BindObject.GetComponent<RectTransform>().anchoredPosition
            = new Vector2(-1f * (contentWidth + UIValues.ContentLRPadding + UIValues.TimePosPadding), 0f);
        Context.TimeText.BindObject.GetComponent<RectTransform>().sizeDelta
            = new Vector2(Context.TimeText.BindObject.GetComponent<RectTransform>().sizeDelta.x, contentHeight + UIValues.ContentUDPadding);
    }

    public void SetChecked()
    {
        Context.CheckImage.BindObject.SetActive(true);
        Context.FailImage.BindObject.SetActive(false);
    }

    public void SetFailed()
    {
        Context.CheckImage.BindObject.SetActive(false);
        Context.FailImage.BindObject.SetActive(true);
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
