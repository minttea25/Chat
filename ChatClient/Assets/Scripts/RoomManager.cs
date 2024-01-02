using Chat;
using Core;
using System.Collections.Generic;

public class RoomManager : IManager
{
    readonly Dictionary<ulong, Room> rooms = new();

    public bool TryGetRoom(ulong roomId, out Room room)
    {
        return rooms.TryGetValue(roomId, out room);
    }

    public void AddChat(CChatText chat)
    {
        if (rooms.TryGetValue(chat.RoomNumber, out Room room))
        {
            ChatText data = new ChatText(chat);
            room.AddChat(data);
        }
    }

    public void AddChat(CChatIcon chat)
    {
        if (rooms.TryGetValue(chat.RoomNumber, out Room room))
        {
            ChatIcon data = new ChatIcon(chat);
            room.AddChat(data);
        }
    }

    public void UserEnter(CUserEnterRoom msg)
    {
        if (rooms.TryGetValue(msg.RoomNumber, out Room room))
        {
            ChatUserEnter data = new ChatUserEnter(msg);
            room.AddChat(data);
        }
    }

    public void UserLeft(CUserLeftRoom msg)
    {
        if (rooms.TryGetValue(msg.RoomNumber, out Room room))
        {
            ChatUserLeave data = new ChatUserLeave(msg);
            room.AddChat(data);
        }
    }

    void IManager.ClearManager()
    {
    }

    void IManager.InitManager()
    {
    }
}
