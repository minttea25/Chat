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

    private void OnDisable()
    {
        Context.NumberInputField.Component.text = string.Empty;
    }

    void OnCreateButtonClicked()
    {
        string number = Context.NumberInputField.Component.text.Trim();
        if (Validations.RoomNumber(number, out var num) == false)
        {
            // TODO : wrong popup
            return;
        }

        ManagerCore.Network.ReqCreateRoom(num);
    }

    void OnCloseButtonClicked()
    {
        Hide();
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
