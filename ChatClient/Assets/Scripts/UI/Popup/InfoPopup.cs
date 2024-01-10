using Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class InfoPopupContext : UIContext
{
    public UIObject BackgroundBlur_Panel = new();
    public UIObject<TextMeshProUGUI> UserNameText = new();
    public UIObject<TMP_InputField> UserNameEditInputField = new();
    public UIObject<Button> UserNameEditButton = new();
    public UIObject<TextMeshProUGUI> UserNameEditText = new();
    public UIObject<Button> UserNameEditCancelButton = new();
    public UIObject<TextMeshProUGUI> UserIdText = new();
    public UIObject<TextMeshProUGUI> AppInfoText = new();
    public UIObject EditErrorText = new();
}

public class InfoPopup : BaseReusableUIPopup
{
    [SerializeField]
    InfoPopupContext Context = new();

    bool editMode = false;

    readonly Property<string, TextMeshProUGUI> UserName = new();
    readonly Property<string, TextMeshProUGUI> UserId = new();
    readonly Property<string, TextMeshProUGUI> AppInfo = new();

    // TEMP
    const string EDIT_TEXT = "Edit";
    const string OK_TEXT = "OK";

    public override void Init()
    {
        base.Init();

        UserName.AddUI(Context.UserNameText);
        UserId.AddUI(Context.UserIdText);
        AppInfo.AddUI(Context.AppInfoText);

        Context.UserNameEditText.Component.text = EDIT_TEXT;

        // hide ui
        Context.UserNameEditInputField.BindObject.SetActive(false);
        Context.UserNameEditCancelButton.BindObject.SetActive(false);
        Context.EditErrorText.BindObject.SetActive(false);

        Context.UserNameEditButton.Component.onClick.AddListener(OnEditButtonClicked);
        Context.UserNameEditCancelButton.Component.onClick.AddListener(SetNameViewMode);

        Context.BackgroundBlur_Panel.BindObject.BindEventOnUI(Hide);

        RefreshData();
    }

    public void RefreshData()
    {
        UserName.Data = ManagerCore.Network.UserInfo.UserName;
        UserId.Data = ManagerCore.Network.UserInfo.UserLoginId;
    }

    public void SuccessChangeUserName(string username)
    {
        NotificationUI.Show("Username is changed successfully!");
        SetNameViewMode();
        Context.UserNameEditInputField.Component.text = string.Empty;

        RefreshData();
    }

    public void FailChaneUserName()
    {
        NotificationUI.Show("Failed to edit user name.");
    }

    void OnEditButtonClicked()
    {
        // req edit
        if (editMode == true)
        {
            string newName = Context.UserNameEditInputField.Component.text;

            if (Validations.UserName(newName) == false)
            {
                Context.EditErrorText.BindObject.SetActive(true);
                return;
            }
            else Context.EditErrorText.BindObject.SetActive(false);

            ManagerCore.Scene.GetScene<MainScene>().ReqEditUserName(newName);

        }
        else
        {
            SetNameEditMode();
        }
    }

    void SetNameEditMode()
    {
        editMode = true;

        Context.UserNameEditInputField.Component.text = UserName.Data;
        Context.UserNameEditText.Component.text = OK_TEXT;
        
        Context.UserNameText.BindObject.SetActive(false);
        Context.UserNameEditInputField.BindObject.SetActive(true);
        Context.UserNameEditCancelButton.BindObject.SetActive(true);
    }

    void SetNameViewMode()
    {
        editMode = false;

        Context.UserNameEditText.Component.text = EDIT_TEXT;

        Context.UserNameText.BindObject.SetActive(true);
        Context.UserNameEditInputField.BindObject.SetActive(false);
        Context.UserNameEditCancelButton.BindObject.SetActive(false);
    }


#if UNITY_EDITOR
    public override Type GetContextType()
    {
        return typeof(InfoPopupContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
