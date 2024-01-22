using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class ChatLeftIconItemUIContext : UIContext
{
    public UIObject<TextMeshProUGUI> UserNameText = new();
    public UIObject<Image> IconImage = new();
    public UIObject<TextMeshProUGUI> TimeText = new();
}

public class ChatLeftIconItemUI : BaseUIItem
{
    [SerializeField]
    ChatLeftIconItemUIContext Context = new();

    public UIObject<TextMeshProUGUI> UserNameText => Context.UserNameText;
    //Property<Sprite, Image> Icon = new();

    public void SetMessage(uint iconId, string time, string userName = null)
    {
        Context.IconImage.Component.sprite = ManagerCore.Data.Emoticons?.GetEmoticon(iconId);
        Context.TimeText.Component.text = time;

        if (userName != null)
        {
            Context.UserNameText.Component.text = userName;
        }

    }


#if UNITY_EDITOR

    public override Type GetContextType()
    {
        return typeof(ChatLeftIconItemUIContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
