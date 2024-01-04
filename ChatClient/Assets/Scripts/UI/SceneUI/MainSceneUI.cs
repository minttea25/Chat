using Chat;
using Chat.Utils;
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

// Note: It is only controlled by MainScene.

public class MainSceneUI : BaseUIScene
{
    [SerializeField]
    MainSceneUIContext Context = new();

    public Transform RoomListTransform => Context.RoomList.BindObject.transform;
    public Transform ChatPanelTransform => Context.ChatPanel.BindObject.transform;

    public readonly Property<string, TextMeshProUGUI> RoomListRefreshTime = new();
    public readonly Property<string, TextMeshProUGUI> Ping = new();

    public MainScene Scene { get; set; }

    readonly MemoryQueue<ulong, ChatPanelItem> chatPanels = new(10);
    readonly Dictionary<ulong, RoomListItemUI> roomList = new Dictionary<ulong, RoomListItemUI>();
    ulong openedChatId = 0;


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
                Core.Utils.AssertCrash(failed.Count == 0);
                OnLoaded(); // hide loading ui here
            },
            AddrKeys.RoomListItemUI,
            AddrKeys.CreateRoomPopupUI,
            AddrKeys.EnterRoomPopupUI,
            AddrKeys.InfoPopupUI,
            AddrKeys.LogoutPopupUI,
            AddrKeys.ChatPanelItemUI,
            AddrKeys.ChatLeftItemUI,
            AddrKeys.ChatRightItemUI,
            AddrKeys.ChatContentEtcItemUI);
    }

    private void OnDisable()
    {
        ManagerCore.Resource.ReleaseAll(
            AddrKeys.RoomListItemUI,
            AddrKeys.CreateRoomPopupUI,
            AddrKeys.EnterRoomPopupUI,
            AddrKeys.InfoPopupUI,
            AddrKeys.LogoutPopupUI,
            AddrKeys.ChatPanelItemUI,
            AddrKeys.ChatLeftItemUI,
            AddrKeys.ChatRightItemUI,
            AddrKeys.ChatContentEtcItemUI);
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

    public void AddRoomList(RoomInfo room)
    {
        AddRoomList(room.RoomName, room.RoomNumber);
    }

    public void AddRoomList(string roomName, uint roomNumber)
    {
        RoomListItemUI item = ManagerCore.UI.AddItemUI<RoomListItemUI>(AddrKeys.RoomListItemUI, RoomListTransform);
        item.SetRoomName(roomName);
        item.SetRoomNumber(roomNumber);
        item.BindEventUnityAction(() => ShowChat(roomNumber));
        roomList.Add(roomNumber, item);
    }

    public void RemoveRoom(ulong roomNumber)
    {
        if (roomList.TryGetValue(roomNumber, out var room))
        {
            Destroy(room.gameObject);
            roomList.Remove(roomNumber);
        }
    }

    public void RefreshRoomList(DateTime time)
    {
        // TODO
        List<Room> list = ManagerCore.Room.GetRooms();
        foreach (var room in list)
        {
            AddRoomList(room.RoomName, room.RoomNumber);
        }
    }

    public void CloseRoomPopups()
    {
        // TODO : check the dotween
        if (enterRoomPopup != null) enterRoomPopup.Hide();
        if (createRoomPopup != null) createRoomPopup.Hide();
    }

    public void SetRefreshTime(DateTime time)
    {
        // TODO : change the format const.
        RoomListRefreshTime.Data = time.ToLocalTimeFormat();
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

    public void ShowChat(uint roomId)
    {
        Debug.Log($"ShowChat: {roomId}");

        if (openedChatId == roomId) return;

        if (chatPanels.TryGetValue(openedChatId, out var opened))
        {
            opened.gameObject.SetActive(false);
        }

        if (chatPanels.TryGetValue(roomId, out var chatPanel))
        {
            chatPanel.gameObject.SetActive(true);
        }
        else
        {
            var chat = ManagerCore.UI.AddItemUI<ChatPanelItem>(AddrKeys.ChatPanelItemUI, ChatPanelTransform);
            Core.Utils.AssertCrash(chat != null);

            chat.SetRoom(roomId);

            chatPanels.Add(roomId, chat, out var removedChat);
            if(removedChat != null) ManagerCore.Resource.Destroy(removedChat.gameObject);
        }
        openedChatId = roomId;
    }

    public void OpenEnterRoomPopup()
    {
        if (enterRoomPopup == null)
        {
            enterRoomPopup = ManagerCore.UI.ShowPopupUI<EnterRoomPopup>(AddrKeys.EnterRoomPopupUI);
            Core.Utils.AssertCrash(enterRoomPopup != null);
        }
        else enterRoomPopup.Show();
    }

    public void OpenCreateRoomPopup()
    {
        if (createRoomPopup == null)
        {
            createRoomPopup = ManagerCore.UI.ShowPopupUI<CreateRoomPopup>(AddrKeys.CreateRoomPopupUI);
            Core.Utils.AssertCrash(createRoomPopup != null);
        }
        else createRoomPopup.Show();
    }

    public void OpenInfoPopup()
    {
        if (infoPopup == null)
        {
            infoPopup = ManagerCore.UI.ShowPopupUI<InfoPopup>(AddrKeys.InfoPopupUI);
            Core.Utils.AssertCrash(infoPopup != null);
        }
        else infoPopup.Show();
    }

    public void OpenLogoutPopup()
    {
        if (logoutPopup == null)
        {
            logoutPopup = ManagerCore.UI.ShowPopupUI<LogoutPopup>(AddrKeys.LogoutPopupUI);
            Core.Utils.AssertCrash(logoutPopup != null);
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
