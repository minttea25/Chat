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

        ManagerCore.Scene.GetScene<StartScene>().ReqRegister();
    }

    void LoginReq()
    {
        Debug.Log("LoginReq");

        string id = Context.IdInput.Component.text;
        string password = Context.PasswordInput.Component.text;

        //ManagerCore.Scene.GetScene<StartScene>().ReqLogin(id, password);

        // TEMP
        ManagerCore.Scene.GetScene<StartScene>().TestLogin();
    }

    bool ValidateId()
    {
        // TODO : check the string with regex
        return false;
    }

    bool ValidatePassword()
    {
        // TODO : check the string with regex
        return false;
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
