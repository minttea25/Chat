using Chat;
using Core;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class MainSceneUIContext : UIContext
{
    public UIObject LeftPanel = new();
    public UIObject RightPanel = new();

    public UIObject RoomList = new();

    public UIObject<TextMeshProUGUI> RefreshedTimeText = new();

    public UIObject<Button> EnterRoomButton = new();
    public UIObject<Button> CreateRoomButton = new();
    public UIObject<Button> InfoButton = new();
    public UIObject<Button> LogoutButton = new();
    public UIObject<Button> SettingButton = new();

    public UIObject NoSelectedImage = new();
    public UIObject ChatPanel = new();
}

public class MainSceneUI : BaseUIScene
{
    [SerializeField]
    MainSceneUIContext Context = new();

    public Transform RoomListTransform => Context.RoomList.BindObject.transform;

    public readonly Property<string, TextMeshProUGUI> RoomListRefreshTime = new();

    public override void Init()
    {
        base.Init();

        Clear();

        Context.EnterRoomButton.Component.onClick.AddListener(OpenEnterRoomPopup);
        Context.CreateRoomButton.Component.onClick.AddListener(OpenCreateRoomPopup);
        Context.InfoButton.Component.onClick.AddListener(OpenInfoPopup);
        Context.LogoutButton.Component.onClick.AddListener(OpenLogoutPopup);
        Context.SettingButton.Component.onClick.AddListener(OpenSettingPopup);


    }

    public void RefreshRoomList(List<RoomInfo> rooms)
    {
        // TEMP
        foreach (var r in rooms)
        {
            Debug.Log(r);
        }

        // TODO : loading?
        foreach (var r in rooms)
        {
            ManagerCore.UI.AddItemUIAsync<RoomListItemUI>(
                AddrKeys.RoomListItemUI,
                RoomListTransform,
                (ui) =>
                {
                    ui.SetRoomName(r.RoomName);
                    ui.SetRoomNumber(r.RoomId);
                });
        }
    }

    public void SetRefreshTime(Timestamp time)
    {
        // TODO : change the format const.
        RoomListRefreshTime.Data = time.ToDateTime().ToString("HH-mm-ss");
    }

    public void Clear()
    {
        Context.RoomList.BindObject.transform.DestroyAllItems();
        Context.ChatPanel.BindObject.transform.DestroyAllItems();
    }

    public void OpenEnterRoomPopup()
    {
        Debug.Log("OpenEnterRoomPopup");
    }

    public void OpenCreateRoomPopup()
    {
        Debug.Log("OpenCreateRoomPopup");
    }

    public void OpenInfoPopup()
    {
        Debug.Log("OpenInfoPopup");
    }

    public void OpenLogoutPopup()
    {
        Debug.Log("OpenLogoutPopup");
    }

    public void OpenSettingPopup()
    {
        Debug.Log("OpenSettingPopup");
    }

    #region Network

    #endregion


#if UNITY_EDITOR
    public override System.Type GetContextType()
    {
        return typeof(MainSceneUIContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
