using Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class StartUIContext : UIContext
{
    public UIObject IdPanel = new();
    public UIObject PasswordPanel = new();
    public UIObject ButtonPanel = new();
    
    public UIObject<TMP_InputField> IdInput = new();
    public UIObject<TMP_InputField> PasswordInput = new();

    public UIObject<Button> RegisterButton = new();
    public UIObject<TextMeshProUGUI> RegisterText = new();
    public UIObject<Button> LoginButton = new();
    public UIObject<TextMeshProUGUI> LoginText = new();
}

public class StartSceneUI : BaseUIScene
{
    [SerializeField]
    StartUIContext Context = new();

    string Id => Context.IdInput.Component.text;
    string Password => Context.PasswordInput.Component.text;

    public override void Init()
    {
        base.Init();



        SetButtonActions();


    }

    void SetButtonActions()
    {
        Context.RegisterButton.Component.onClick.AddListener(RegisterReq);
        Context.LoginButton.Component.onClick.AddListener(LoginReq);
    }


    void RegisterReq()
    {
        Debug.Log("RegisterReq");

        if (ValidateId() == false)
        {
            // TODO : Show Popup
            Debug.Log("Invalid Id");
            return;
        }

        if (ValidatePassword() == false)
        {
            // TODO : Show Popup
            Debug.Log("Invalid Password");
            return;
        }

        ManagerCore.Scene.GetScene<StartScene>().ReqAccountRegister(Id, Password);
    }

    void LoginReq()
    {
        Debug.Log("Login Req");


        if (ValidateId() == false)
        {
            // TODO : Show Popup
            Debug.Log("Invalid Id");
            return;
        }

        if (ValidatePassword() == false)
        {
            // TODO : Show Popup
            Debug.Log("Invalid Password");
            return;
        }

        ManagerCore.Scene.GetScene<StartScene>().ReqAccountLogin(Id, Password);
    }

    bool ValidateId()
    {
        if (string.IsNullOrEmpty(Id)) return false;

        // TODO : check the string with regex
        return true;
    }

    bool ValidatePassword()
    {
        if (string.IsNullOrEmpty(Password)) return false;

        // TODO : check the string with regex
        return true;
    }

#if UNITY_EDITOR
    public override Type GetContextType()
    {
        return typeof(StartUIContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
