using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System;
using TMPro;
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
}

public class ChatPanelItem : BaseUIItem
{
    [SerializeField]
    ChatPanelItemContext Context = new();

    Transform ChatListTransform => Context.ChatList.BindObject.transform;

    public uint RoomNumber { get; private set; } = default;

    int cid = 1; // for checking successful sending
    Dictionary<int, ChatRightItemUI> pendingChats = new Dictionary<int, ChatRightItemUI>();

    public override void Init()
    {
        base.Init();

        Context.SendButton.Component.onClick.AddListener(SendText);
        Context.IconButton.Component.onClick.AddListener(IconSelectButton);

        ChatListTransform.DestroyAllItems();
    }

    private void OnEnable()
    {
        if (RoomNumber == default) return;

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

    public void SetRoom(uint roomId)
    {
        RoomNumber = roomId;
        if (ManagerCore.Room.TryGetRoom(RoomNumber, out Room room) == false)
        {
            Utils.AssertCrash(false, $"The room[{RoomNumber}] is not in RoomManager");
            return;
        }
        room.AttachUI(this);
    }

    //private void Update()
    //{
    //    if (RoomId == default) return;
    //    if (ManagerCore.Chat.Rooms.ContainsKey(RoomId) == false) return;
    //}

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
        if (pendingChats.TryGetValue(chatId, out var chat))
        {
            chat.SetSuccess(success);
            if (success)
            {
                pendingChats.Remove(chatId);
            }
        }
        else
        {
            // TODO : error handling in DEBUG
            Debug.LogWarning($"Can not find the chat [id={chatId}].");
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

        pendingChats.Add(chatId, right);
        cid++;

        right.SetMessage(message, DateTime.Now.ToLocalTimeFormat());

        return chatId;
    }

    void AddChatText(ChatText chat)
    {
        // my chat
        if (chat.User.UserDbId == ManagerCore.Network.UserInfo.UserDbId)
        {
            return;
        }
        else
        {
            ChatLeftItemUI left = ManagerCore.UI.AddItemUI<ChatLeftItemUI>(AddrKeys.ChatLeftItemUI, ChatListTransform);
            Utils.Assert(left != null);

            left.SetMessage(chat.Message, chat.Time.ToLocalTimeFormat(), chat.User.UserName);
        }
        
    }

    void AddChatIcon(ChatIcon chat)
    {
        // TODO : set Icon
        Debug.Log("AddChatIcon");

        
    }

    void AddChatEnter(ChatUserEnter chat)
    {
        var enter = ManagerCore.UI.AddItemUI<ChatContentEtcItemUI>(AddrKeys.ChatContentEtcItemUI, ChatListTransform);
        Utils.Assert(enter != null);

        enter.SetText(chat.User.UserName, chat.Time.ToLocalTimeFormat(), enter: true);
    }

    void AddChatLeave(ChatUserLeave chat)
    {
        var leave = ManagerCore.UI.AddItemUI<ChatContentEtcItemUI>(AddrKeys.ChatContentEtcItemUI, ChatListTransform);
        Utils.Assert(leave != null);

        leave.SetText(chat.User.UserName, chat.Time.ToLocalTimeFormat(), enter: false);
    }


    void IconSelectButton()
    {
        Debug.Log("IconSelectButton");
    }

    void SendText()
    {
        string message = Context.ChatTextInput.Component.text;

        if (ValidateMessage(message) == false) return;

        // 미리 보여주기
        int chatId = AddMyChat(message);

        ManagerCore.Network.ReqSendChatText(message, RoomNumber, chatId);

        Context.ChatTextInput.Component.text = string.Empty;
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
