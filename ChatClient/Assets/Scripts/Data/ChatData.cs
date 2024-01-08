using Chat;
using System;
using UnityEditor.VersionControl;

public enum ChatDataType
{
    Text, Icon, Enter, Leave
}

public abstract class ChatData
{
    public ChatData(ChatDataType dataType, UserInfo user, DateTime time)
    {
        User = user;
        DataType = dataType;
        Time = time;
    }

    public UserInfo User { get; private set; }
    public ChatDataType DataType { get; private set; }
    public DateTime Time { get; private set; }
    
}

public class ChatUserEnter : ChatData
{
    public ChatUserEnter(CUserEnterRoom msg) : base(ChatDataType.Enter, msg.EnterUser, msg.EnteredTime.ToDateTime())
    {
    }
}

public class ChatUserLeave : ChatData
{
    public ChatUserLeave(CUserLeftRoom msg) : base(ChatDataType.Leave, msg.LeftUser, msg.LeftTime.ToDateTime())
    {
    }
}

public class ChatText : ChatData
{
    public ChatText(CChatText chat) : base(ChatDataType.Text, chat.SenderInfo, chat.Chat.ChatBase.Timestamp.ToDateTime())
    {
        ChatType = chat.Chat.ChatBase.ChatType;
        Message = chat.Chat.Msg;
    }

    public ChatType ChatType { get; private set; }
    public string Message { get; private set; }
}

public class ChatIcon : ChatData
{
    public ChatIcon(CChatIcon chat) : base(ChatDataType.Icon, chat.SenderInfo, chat.Chat.ChatBase.Timestamp.ToDateTime())
    {
        ChatType = chat.Chat.ChatBase.ChatType;
        IconId = chat.Chat.IconId;
    }

    public ChatType ChatType { get; private set; }
    public uint IconId { get; private set; }
}
