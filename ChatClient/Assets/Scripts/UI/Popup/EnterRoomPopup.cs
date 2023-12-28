using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EnterRoomPopupContext : UIContext
{
    public UIObject<TMP_InputField> NumberInputField = new();
    public UIObject<Button> EnterButton = new();
    public UIObject<Button> CloseButton = new();
}

public class EnterRoomPopup : BaseReusableUIPopup
{
    [SerializeField]
    EnterRoomPopupContext Context = new();

    public override void Init()
    {
        base.Init();

        Context.EnterButton.Component.onClick.AddListener(OnEnterButtonClicked);
        Context.CloseButton.Component.onClick.AddListener(OnCloseButtonClicked);
    }

    void OnEnterButtonClicked()
    {
        string number = Context.NumberInputField.Component.text.Trim();
        if (ValidateNumber(number) == false)
        {
            // TODO : wrong popup
            return;
        }

        ManagerCore.Network.ReqEnterRoom(ulong.Parse(number));
    }

    void OnCloseButtonClicked()
    {
        Hide();
    }

    bool ValidateNumber(string number)
    {
        // TODO : validate the room number
        // ex - range and is ulong
        return true;
    }

#if UNITY_EDITOR
    public override Type GetContextType()
    {
        return typeof(EnterRoomPopupContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
