using Chat;
using Chat.Utils;
using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

    public Dictionary<uint, RoomListItemUI> RoomListItems => m_roomList;

    public ChatPanelItem SelectedChatPanel => GetChatPanel(openedChatId);

    readonly MemoryQueue<uint, ChatPanelItem> chatPanels = new(10);
    readonly Dictionary<uint, RoomListItemUI> m_roomList = new Dictionary<uint, RoomListItemUI>();
    uint openedChatId = 0;

    bool LabelChatLoaded = false;
    bool LabelEmoticonLoaded = false;

    CreateRoomPopup createRoomPopup = null;
    EnterRoomPopup enterRoomPopup = null;
    InfoPopup infoPopup = null;
    LogoutPopup logoutPopup = null;

    private void OnEnable()
    {
        LoadingUI.Show();

        // pre-load
        ManagerCore.Resource.LoadWithLabelAsync(
            AddrKeys.Label_Chat,
            (failed) =>
            {
                LabelChatLoaded = true;
                Core.Utils.AssertCrash(failed.Count == 0);
                OnLoaded(); // hide loading ui here
            });

        ManagerCore.Resource.LoadWithLabelAsync<Sprite>(
            AddrKeys.Label_Emoticon, (failed) =>
            {
                LabelEmoticonLoaded = true;
                Core.Utils.AssertCrash(failed.Count == 0);
                OnLoaded(); // hide loading ui here
            });


        //ManagerCore.Resource.LoadAllAsync(
        //    (failed) =>
        //    {
        //        Core.Utils.AssertCrash(failed.Count == 0);
        //        OnLoaded(); // hide loading ui here
        //    },
        //    AddrKeys.RoomListItemUI,
        //    AddrKeys.CreateRoomPopupUI,
        //    AddrKeys.EnterRoomPopupUI,
        //    AddrKeys.InfoPopupUI,
        //    AddrKeys.LogoutPopupUI,
        //    AddrKeys.ChatPanelItemUI,
        //    AddrKeys.ChatLeftItemUI,
        //    AddrKeys.ChatRightItemUI,
        //    AddrKeys.ChatContentEtcItemUI);
    }

    private void OnDisable()
    {
        //ManagerCore.Resource.ReleaseWithLabel(AddrKeys.Label_Chat);

        ManagerCore.Resource.ReleaseWithLabels(AddrKeys.Label_Chat, AddrKeys.Label_Emoticon);

        //ManagerCore.Resource.Release(
        //    AddrKeys.RoomListItemUI,
        //    AddrKeys.CreateRoomPopupUI,
        //    AddrKeys.EnterRoomPopupUI,
        //    AddrKeys.InfoPopupUI,
        //    AddrKeys.LogoutPopupUI,
        //    AddrKeys.ChatPanelItemUI,
        //    AddrKeys.ChatLeftItemUI,
        //    AddrKeys.ChatRightItemUI,
        //    AddrKeys.ChatContentEtcItemUI);
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

    public ChatPanelItem GetChatPanel(uint roomId)
    {
        if (chatPanels.TryGetValue(roomId, out var chatpanel))
        {
            return chatpanel;
        }
        else return null;
    }

    public void AddRoomList(RoomInfo room)
    {
        AddRoomList(room.RoomName, room.RoomNumber);
    }

    public void AddRoomList(string roomName, uint roomNumber)
    {
        RoomListItemUI item = ManagerCore.UI.AddItemUI<RoomListItemUI>(AddrKeys.RoomListItemUI, RoomListTransform);
        item.SetData(roomNumber, roomName);
        item.BindEventUnityAction(() => ShowChat(roomNumber));
        item.BindEventUnityAction(() => RoomListLongClicked(roomNumber), UIEvent.LongClick);

        m_roomList.Add(roomNumber, item);
    }

    public void RemoveRoom(uint roomNumber)
    {
        if (m_roomList.TryGetValue(roomNumber, out var room))
        {
            m_roomList.Remove(roomNumber);
            Destroy(room.gameObject);
        }
    }

    public void RefreshRoomList(DateTime time)
    {
        List<Room> list = ManagerCore.Room.GetRooms();
        foreach (var room in list)
        {
            AddRoomList(room.RoomName, room.RoomNumber);
        }
    }

    public void CloseCreateRoomPopup()
    {
        if (createRoomPopup != null) createRoomPopup.Hide();
    }


    public void CloseEnterRoomPopup()
    {
        if (enterRoomPopup != null) enterRoomPopup.Hide();
    }

    public void SetRefreshTime(DateTime time)
    {
        // TODO : change the format const.
        RoomListRefreshTime.Data = time.ToLocalTimeFormat();
    }

    public void SetPing(long ping)
    {
        Ping.Data = ping.ToString();
    }

    public void EditUserNameRes(bool success, string username = null)
    {
        if (infoPopup == null) return;

        if (success == true) infoPopup.SuccessChangeUserName(username);
        else infoPopup.FailChaneUserName();
    }

    public void Clear()
    {
        Context.RoomList.BindObject.transform.DestroyAllItems();
        Context.ChatPanel.BindObject.transform.DestroyAllItems();
    }

    public void ShowChat(uint roomId)
    {
        if (openedChatId == roomId) return;


        if (chatPanels.TryGetValue(openedChatId, out var opened))
        {
            opened.gameObject.SetActive(false);
        }
        if (m_roomList.TryGetValue(openedChatId, out var roomListItem))
        {
            roomListItem.OnUnSelected();
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
        if (m_roomList.TryGetValue(roomId, out var selectedRoomListItem))
        {
            selectedRoomListItem.OnSelected();
        }
        openedChatId = roomId;
    }

    public void RoomListLongClicked(uint roomNumber)
    {
        var popup = ManagerCore.UI.ShowPopupUI<SimplePopupUI>(
            AddrKeys.SimplePopupUI);

        popup.Setup($"Leave room, id={roomNumber}?", 2,
            new string[] { "Leave", "Cancel" },
            new UnityAction[]
            {
                // OK
                () =>
                {
                    if (openedChatId == roomNumber) openedChatId = 0;

                    ManagerCore.Network.ReqLeaveRoom(roomNumber);
                    if (m_roomList.TryGetValue(roomNumber, out var room))
                    {
                        room.gameObject.SetActive(false);
                    }
                    ManagerCore.Room.RemoveRoom(roomNumber);
                    var panel = chatPanels.Remove(roomNumber);
                    if (panel != null)
                    {
                        Destroy(panel.gameObject);
                    }
                    popup.Close();
                },
                // Cancel
                popup.Close,
            });
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
        if (LabelChatLoaded == false || LabelEmoticonLoaded == false) return;

        LoadingUI.Hide();
        Scene.CheckLoadAllCompleted();
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
