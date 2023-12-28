using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class CreateRoomPopupContext : UIContext
{
    public UIObject<TMP_InputField> NumberInputField = new();
    public UIObject<Button> CreateButton = new();
    public UIObject<Button> CloseButton = new();
}

public class CreateRoomPopup : BaseReusableUIPopup
{
    [SerializeField]
    CreateRoomPopupContext Context = new();

    public override void Init()
    {
        base.Init();

        Context.CreateButton.Component.onClick.AddListener(OnCreateButtonClicked);
        Context.CloseButton.Component.onClick.AddListener(OnCloseButtonClicked);
    }

    void OnCreateButtonClicked()
    {
        string number = Context.NumberInputField.Component.text.Trim();
        if (ValidateNumber(number) == false)
        {
            // TODO : wrong popup
            return;
        }

        ManagerCore.Network.ReqCreateRoom(ulong.Parse(number));
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
        return typeof(CreateRoomPopupContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
