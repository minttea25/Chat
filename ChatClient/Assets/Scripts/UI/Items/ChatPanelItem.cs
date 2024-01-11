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
}

public class ChatPanelItem : BaseUIItem
{
    [SerializeField]
    ChatPanelItemContext Context = new();

    Transform ChatListTransform => Context.ChatList.BindObject.transform;

    public uint RoomNumber { get; private set; } = default;

    int cid = 1; // for checking successful sending
    readonly Dictionary<int, ChatRightItemUI> sendingChats = new Dictionary<int, ChatRightItemUI>();

    public override void Init()
    {
        base.Init();

        Context.SendButton.Component.onClick.AddListener(SendText);
        Context.IconButton.Component.onClick.AddListener(IconSelectButton);

        ChatListTransform.DestroyAllItems();
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
            ErrorHandling.HandleError(ErrorHandling.ErrorType.Logic,
                ErrorHandling.ErrorLevel.Error,
                $"Can not find chat [id={chatId}]");
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

    void AddChatText(ChatText chat)
    {
        // my chat
        if (chat.IsMine)
        {
            return;
        }
        else
        {
            ChatLeftItemUI left = ManagerCore.UI.AddItemUI<ChatLeftItemUI>(AddrKeys.ChatLeftItemUI, ChatListTransform);
            Utils.Assert(left != null);

            left.SetMessage(chat.Message, chat.Time.ToLocalTimeFormat(), chat.UserName);
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
