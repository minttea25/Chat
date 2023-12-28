using Chat;
using Core;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public UIObject<TextMeshProUGUI> PingText = new();

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
    public readonly Property<string, TextMeshProUGUI> Ping = new();

    public MainScene Scene { get; set; }


    CreateRoomPopup createRoomPopup = null;
    EnterRoomPopup enterRoomPopup = null;
    InfoPopup infoPopup = null;
    LogoutPopup logoutPopup = null;

    private void OnEnable()
    {
        LoadingUI.Show();

        // pre-load
        ManagerCore.Resource.LoadAllAsync(
            (failed) =>
            {
                Utils.AssertCrash(failed.Count == 0);
                OnLoaded(); // hide loading ui here
            },
            AddrKeys.RoomListItemUI,
            AddrKeys.CreateRoomPopupUI,
            AddrKeys.EnterRoomPopupUI,
            AddrKeys.InfoPopupUI,
            AddrKeys.LogoutPopupUI);
    }

    private void OnDisable()
    {
        ManagerCore.Resource.ReleaseAll(
            AddrKeys.RoomListItemUI,
            AddrKeys.CreateRoomPopupUI,
            AddrKeys.EnterRoomPopupUI,
            AddrKeys.InfoPopupUI,
            AddrKeys.LogoutPopupUI);
    }

    public override void Init()
    {
        base.Init();

        Clear();

        RoomListRefreshTime.AddUI(Context.RefreshedTimeText);
        Ping.AddUI(Context.PingText);


        Context.EnterRoomButton.Component.onClick.AddListener(OpenEnterRoomPopup);
        Context.CreateRoomButton.Component.onClick.AddListener(OpenCreateRoomPopup);
        Context.InfoButton.Component.onClick.AddListener(OpenInfoPopup);
        Context.LogoutButton.Component.onClick.AddListener(OpenLogoutPopup);
        Context.SettingButton.Component.onClick.AddListener(OpenSettingPopup);
    }

    public void RefreshRoomList(List<RoomInfo> rooms)
    {
        // TODO : loading?
        foreach (var r in rooms)
        {
            RoomListItemUI item = ManagerCore.UI.AddItemUI<RoomListItemUI>(AddrKeys.RoomListItemUI, RoomListTransform);
            item.SetRoomName(r.RoomName);
            item.SetRoomNumber(r.RoomNumber);
        }
    }

    public void SetRefreshTime(DateTime time)
    {
        // TODO : change the format const.
        RoomListRefreshTime.Data = time.ToString("HH:mm:ss");
    }

    public void SetPing(long ping)
    {
        Ping.Data = $"{ping} ms";
    }

    public void Clear()
    {
        Context.RoomList.BindObject.transform.DestroyAllItems();
        Context.ChatPanel.BindObject.transform.DestroyAllItems();
    }

    public void OpenEnterRoomPopup()
    {
        if (enterRoomPopup == null)
        {
            enterRoomPopup = ManagerCore.UI.ShowPopupUI<EnterRoomPopup>(AddrKeys.EnterRoomPopupUI);
            Utils.AssertCrash(enterRoomPopup != null);
        }
        else enterRoomPopup.Show();
    }

    public void OpenCreateRoomPopup()
    {
        if (createRoomPopup == null)
        {
            createRoomPopup = ManagerCore.UI.ShowPopupUI<CreateRoomPopup>(AddrKeys.CreateRoomPopupUI);
            Utils.AssertCrash(createRoomPopup != null);
        }
        else createRoomPopup.Show();
    }

    public void OpenInfoPopup()
    {
        if (infoPopup == null)
        {
            infoPopup = ManagerCore.UI.ShowPopupUI<InfoPopup>(AddrKeys.InfoPopupUI);
            Utils.AssertCrash(infoPopup != null);
        }
        else infoPopup.Show();
    }

    public void OpenLogoutPopup()
    {
        if (logoutPopup == null)
        {
            logoutPopup = ManagerCore.UI.ShowPopupUI<LogoutPopup>(AddrKeys.LogoutPopupUI);
            Utils.AssertCrash(logoutPopup != null);
        }
        else logoutPopup.Show();
    }

    public void OpenSettingPopup()
    {
        Debug.Log("OpenSettingPopup");
    }

    void OnLoaded()
    {
        LoadingUI.Hide();
        StartCoroutine(nameof(CheckLoad));
    }

    IEnumerator CheckLoad()
    {
        while (ManagerCore.Network.Connected == NetworkManager.ConnectState.Connecting)
        {
            yield return null;
        }

        if (ManagerCore.Network.Connected == NetworkManager.ConnectState.FailedToConnect)
        {
            // TODO : 연결 실패

        }

        if (ManagerCore.Network.Connected == NetworkManager.ConnectState.Disconnected)
        {
            // TODO : 오류
        }

        // UI 로드 완료 시 로그인 요청
        Scene.TryLogin();
    }


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
