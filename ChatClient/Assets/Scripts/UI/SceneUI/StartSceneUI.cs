using Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class StartUIContext : UIContext
{
    public UIObject LoginCardPanel = new();
    public UIObject<TMP_InputField> LoginIdInput = new();
    public UIObject<TMP_InputField> LoginPwInput = new();
    public UIObject<Button> LoginButton = new();
    public UIObject<Button> LoginToRegisterButton = new();

    public UIObject RegisterCardPanel = new();
    public UIObject<TMP_InputField> RegisterIdInput = new();
    public UIObject<TMP_InputField> RegisterPwInput = new();
    public UIObject<TMP_InputField> RegisterPwInput2 = new();
    public UIObject<Button> RegisterButton = new();
    public UIObject<Button> RegisterToLoginButton = new();
}

public class StartSceneUI : BaseUIScene
{
    [SerializeField]
    StartUIContext Context = new();


    public override void Init()
    {
        base.Init();

        SetLoginCard();

        SetButtonActions();
    }

    void SetButtonActions()
    {
        Context.RegisterToLoginButton.Component.onClick.AddListener(SetLoginCard);
        Context.LoginToRegisterButton.Component.onClick.AddListener(SetRegisterCard);

        Context.RegisterButton.Component.onClick.AddListener(RegisterReq);
        Context.LoginButton.Component.onClick.AddListener(LoginReq);
    }


    void RegisterReq()
    {
        string id = Context.RegisterIdInput.Component.text;
        string password = Context.RegisterPwInput.Component.text;
        string passwoord2 = Context.RegisterPwInput2.Component.text;

        if (password.Equals(passwoord2) == false)
        {
            NotificationUI.Show("The two passwords are not same.");
            Debug.Log("The two passwords are not same.");
            return;
        }

        if (ValidateId(id) == false)
        {
            NotificationUI.Show("Invalid Id...");
            Debug.Log("Invalid Id");
            return;
        }

        if (ValidatePassword(password) == false)
        {
            NotificationUI.Show("Invalid Password...");
            Debug.Log("Invalid Password");
            return;
        }

        ManagerCore.Scene.GetScene<StartScene>().ReqAccountRegister(id, password);
    }

    void LoginReq()
    {
        string id = Context.LoginIdInput.Component.text;
        string password = Context.LoginPwInput.Component.text;

        if (ValidateId(id) == false)
        {
            NotificationUI.Show("Invalid Id...");
            Debug.Log("Invalid Id");
            return;
        }

        if (ValidatePassword(password) == false)
        {
            // TODO : Show Popup
            NotificationUI.Show("Invalid Password...");
            Debug.Log("Invalid Password");
            return;
        }

        ManagerCore.Scene.GetScene<StartScene>().ReqAccountLogin(id, password);
    }

    bool ValidateId(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;

        // TODO : check the string with regex
        return true;
    }

    bool ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password)) return false;

        // TODO : check the string with regex
        return true;
    }


    void SetLoginCard()
    {
        Context.LoginCardPanel.BindObject.SetActive(true);
        Context.RegisterCardPanel.BindObject.SetActive(false);
    }

    void SetRegisterCard()
    {
        Context.LoginCardPanel.BindObject.SetActive(false);
        Context.RegisterCardPanel.BindObject.SetActive(true);
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
