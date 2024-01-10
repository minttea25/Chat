using Chat;
using System;

public enum ChatDataType
{
    Text, Icon, Enter, Leave
}

public abstract class ChatData
{
    public ChatData(ChatDataType dataType, bool isMine, ulong userDbId, DateTime time, string userName)
    {
        IsMine = isMine;
        UserName = userName;
        UserDbId = userDbId;
        DataType = dataType;
        Time = time;
    }

    public string UserName { get; private set; } // TEMP
    public bool IsMine { get; private set; }
    public ulong UserDbId { get; private set; }
    public ChatDataType DataType { get; private set; }
    public DateTime Time { get; private set; }
    
}

public class ChatUserEnter : ChatData
{
    public ChatUserEnter(CUserEnterRoom msg) : base(ChatDataType.Enter, false, msg.EnterUser.UserDbId, msg.EnteredTime.ToDateTime(), msg.EnterUser.UserName)
    {
    }
}

public class ChatUserLeave : ChatData
{
    public ChatUserLeave(CUserLeftRoom msg) : base(ChatDataType.Leave, false, msg.LeftUser.UserDbId, msg.LeftTime.ToDateTime(), msg.LeftUser.UserName)
    {
    }
}

public class ChatText : ChatData
{
    public ChatText(CChatText chat, bool isMine) : base(ChatDataType.Text, isMine, chat.SenderInfo.UserDbId, chat.Chat.ChatBase.Timestamp.ToDateTime(), chat.SenderInfo.UserName)
    {
        ChatType = chat.Chat.ChatBase.ChatType;
        Message = chat.Chat.Msg;
    }

    public ChatType ChatType { get; private set; }
    public string Message { get; private set; }
}

public class ChatIcon : ChatData
{
    public ChatIcon(CChatIcon chat, bool isMine) : base(ChatDataType.Icon, isMine, chat.SenderInfo.UserDbId, chat.Chat.ChatBase.Timestamp.ToDateTime(), chat.SenderInfo.UserName)
    {
        ChatType = chat.Chat.ChatBase.ChatType;
        IconId = chat.Chat.IconId;
    }

    public ChatType ChatType { get; private set; }
    public uint IconId { get; private set; }
}
