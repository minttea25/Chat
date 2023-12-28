using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class LogoutPopupContext : UIContext
{
    public UIObject<Button> OkButton = new();
    public UIObject<Button> CloseButton = new();
}

public class LogoutPopup : BaseReusableUIPopup
{
    [SerializeField]
    LogoutPopupContext Context;

    public override void Init()
    {
        base.Init();

        Context.OkButton.Component.onClick.AddListener(OkButtonClicked);
        Context.CloseButton.Component.onClick.AddListener(Hide);
    }

    void OkButtonClicked()
    {
        ManagerCore.Scene.GetScene<MainScene>().Logout();
    }

#if UNITY_EDITOR
    public override Type GetContextType()
    {
        return typeof(LogoutPopupContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
