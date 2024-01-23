using Core;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class ChatRightIconItemUIContext : UIContext
{
    public UIObject<Image> IconImage = new();
    public UIObject<TextMeshProUGUI> TimeText = new();
    public UIObject StatePanel = new();
    public UIObject CheckImage = new();
    public UIObject<Image> LoadingImage = new();
    public UIObject FailImage = new();
}

public class ChatRightIconItemUI : BaseUIItem, IMyChat
{
    [SerializeField]
    ChatRightIconItemUIContext Context = new();

    Tweener sendChecking = null;

    //Property<Sprite, Image> Icon = new();

    public void SetMessage(uint iconId, string time)
    {

        Context.IconImage.Component.sprite = ManagerCore.Data.Emoticons?.GetEmoticon(iconId);
        Context.TimeText.Component.text = time;

    }

    public void SetSuccess(bool success)
    {
        sendChecking.Pause();
        Destroy(Context.LoadingImage.BindObject);
        sendChecking = null;

        if (success == true) SetChecked();
        else SetFailed();
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

#if UNITY_EDITOR

    public override Type GetContextType()
    {
        return typeof(ChatRightIconItemUIContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
