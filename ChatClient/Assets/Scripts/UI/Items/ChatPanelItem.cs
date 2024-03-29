using Core;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class ChatPanelItemContext : UIContext
{
    public UIObject ChatList = new();
    public UIObject SendChatPanel = new(); // no use
    public UIObject<TMP_InputField> ChatTextInput = new();
    public UIObject ToolPanel = new();
    public UIObject<Button> IconButton = new();
    public UIObject<Button> SendButton = new();

    public UIObject<EmoticonPanel> EmoticonPanel = new();
}

public class ChatPanelItem : BaseUIItem
{
    [SerializeField]
    ChatPanelItemContext Context = new();

    Transform ChatListTransform => Context.ChatList.BindObject.transform;

    public uint RoomNumber { get; private set; } = default;

    int cid = 1; // for checking successful sending
    readonly Dictionary<int, IMyChat> sendingChats = new Dictionary<int, IMyChat>();


    public override void Init()
    {
        base.Init();

        Context.SendButton.Component.onClick.AddListener(SendText);
        Context.IconButton.Component.onClick.AddListener(IconSelectButton);

        ChatListTransform.DestroyAllItems();
        Context.EmoticonPanel.BindObject.SetActive(false);
    }

    private void OnEnable()
    {
        LoadPendingChats();
    }

    void LoadPendingChats()
    {
        if (RoomNumber == 0) return;

        if (ManagerCore.Room.TryGetRoom(RoomNumber, out Room room) == false)
        {
            Utils.AssertCrash(false, $"The room[{RoomNumber}] is not in RoomManager");
            return;
        }

        // show loading
        List<ChatData> chats = room.GetPendingChats();
        foreach (ChatData chat in chats)
        {
            AddChat(chat);
        }
        // hide loading
    }

    void InvokePendingActions()
    {
        if (RoomNumber == 0) return;

        if (ManagerCore.Room.TryGetRoom(RoomNumber, out Room room) == false)
        {
            Utils.AssertCrash(false, $"The room[{RoomNumber}] is not in RoomManager");
            return;
        }

        // invoke actions
        var actions = room.GetPendingJobs();
        foreach (var action in actions) action?.Invoke();
    }

    public void SetRoom(uint roomId)
    {
        RoomNumber = roomId;
        if (ManagerCore.Room.TryGetRoom(RoomNumber, out Room room) == false)
        {
            Utils.AssertCrash(false, $"The room[{RoomNumber}] is not in RoomManager");
            return;
        }
        room.AttachUI(this);

        LoadPendingChats();
    }


    public void AddChat(ChatCacheData chat)
    {
        switch (chat.ChatDataType)
        {
            case ChatDataType.Text:
                AddChatText(chat.ChatText);
                break;
            case ChatDataType.Icon:
                AddChatIcon(chat.ChatIcon);
                break;
            case ChatDataType.Enter:
                AddChatEnter(chat.ChatUserEnter);
                break;
            case ChatDataType.Leave:
                AddChatLeave(chat.ChatUserLeave);
                break;
        }
    }

    public void AddChat(ChatData chat)
    {
        switch (chat.DataType)
        {
            case ChatDataType.Text:
                AddChatText(chat as ChatText);
                break;
            case ChatDataType.Icon:
                AddChatIcon(chat as ChatIcon);
                break;
            case ChatDataType.Enter:
                AddChatEnter(chat as ChatUserEnter);
                break;
            case ChatDataType.Leave:
                AddChatLeave(chat as ChatUserLeave);
                break;
        }
    }

    public void CheckPendingChat(int chatId, bool success = true)
    {
        if (sendingChats.TryGetValue(chatId, out var chat))
        {
            chat.SetSuccess(success);
            if (success)
            {
                sendingChats.Remove(chatId);
            }
        }
        else
        {
            ManagerCore.Error.HandleError(501, ErrorManager.ErrorLevel.Error, "Can not find chat id.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns>Chat Id</returns>
    int AddMyChat(string message)
    {
        int chatId = cid;

        ChatRightItemUI right = ManagerCore.UI.AddItemUI<ChatRightItemUI>(AddrKeys.ChatRightItemUI, ChatListTransform);
        Utils.Assert(right != null);

        sendingChats.Add(chatId, right);
        cid++;

        right.SetMessage(message, DateTime.Now.ToLocalTimeFormat());

        return chatId;
    }

    int AddMyChat(uint emoticonId)
    {
        int chatId = cid;

        ChatRightIconItemUI right = ManagerCore.UI.AddItemUI<ChatRightIconItemUI>(AddrKeys.ChatRightEmoticonItemUI, ChatListTransform);
        Utils.Assert(right != null);

        sendingChats.Add(chatId, right);
        cid++;

        right.SetMessage(emoticonId, DateTime.Now.ToLocalTimeFormat());

        return chatId;
    }

    void AddChatText(ChatText chat)
    {
        // my chat
        if (chat.IsMine == true) return;

        ChatLeftItemUI left = ManagerCore.UI.AddItemUI<ChatLeftItemUI>(AddrKeys.ChatLeftItemUI, ChatListTransform);
        Utils.Assert(left != null);

        left.SetMessage(chat.Message, chat.Time.ToLocalTimeFormat(), chat.UserName);
    }

    void AddChatIcon(ChatIcon chat)
    {
        // my chat
        if (chat.IsMine == true) return;
        ChatLeftIconItemUI left = ManagerCore.UI.AddItemUI<ChatLeftIconItemUI>(AddrKeys.ChatLeftEmoticonItemUI, ChatListTransform);
        Utils.Assert(left != null);

        left.SetMessage(chat.IconId, chat.Time.ToLocalTimeFormat(), chat.UserName);
    }

    void AddChatEnter(ChatUserEnter chat)
    {
        var enter = ManagerCore.UI.AddItemUI<ChatContentEtcItemUI>(AddrKeys.ChatContentEtcItemUI, ChatListTransform);
        Utils.Assert(enter != null);

        enter.SetText(chat.UserName, chat.Time.ToLocalTimeFormat(), enter: true);
    }

    void AddChatLeave(ChatUserLeave chat)
    {
        var leave = ManagerCore.UI.AddItemUI<ChatContentEtcItemUI>(AddrKeys.ChatContentEtcItemUI, ChatListTransform);
        Utils.Assert(leave != null);

        leave.SetText(chat.UserName, chat.Time.ToLocalTimeFormat(), enter: false);
    }


    void IconSelectButton()
    {
        Context.EmoticonPanel.BindObject.SetActive(true);
    }

    public void SendText()
    {
        string message = Context.ChatTextInput.Component.text;

        if (ValidateMessage(message) == false) return;

        // 미리 보여주기
        int chatId = AddMyChat(message);

        ManagerCore.Network.ReqSendChatText(message, RoomNumber, chatId);

        Context.ChatTextInput.Component.text = string.Empty;
    }

    public void SendEmoticon(uint emoticonId)
    {
        Debug.Log($"{emoticonId}: {ManagerCore.Data.Emoticons.GetEmoticon(emoticonId).name}");

        
        if (emoticonId == 0) return;

        // 미리 보여주기
        int chatId = AddMyChat(emoticonId);

        ManagerCore.Network.ReqSendChatEmoticon(emoticonId, RoomNumber, chatId);

        Context.EmoticonPanel.BindObject.SetActive(false);
    }

    bool ValidateMessage(string message)
    {
        if (string.IsNullOrEmpty(message) == true) return false;

        // TODO
        return true;
    }



#if UNITY_EDITOR
    public override System.Type GetContextType()
    {
        return typeof(ChatPanelItemContext);
    }

    protected override object GetContext()
    {
        return Context;
    }
#endif
}
