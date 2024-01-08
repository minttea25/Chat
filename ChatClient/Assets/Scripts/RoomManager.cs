using Chat;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;

public class RoomManager : IManager
{
    readonly object _lock = new object();
    readonly Dictionary<uint, Room> rooms = new();
    MainScene _scene = null;
    MainScene Scene 
    {
        get
        {
            if (_scene == null) _scene = ManagerCore.Scene.GetScene<MainScene>();
            return _scene;
        }
    } // can be null

    /// <summary>
    /// Add new room and show the item ui of it.
    /// </summary>
    /// <param name="info"></param>
    public void AddRoom(RoomInfo info)
    {
        lock (_lock)
        {
            if (rooms.ContainsKey(info.RoomNumber) == true) return;

            var room = new Room(info);
            rooms.Add(info.RoomNumber, room);
        }

        if (Scene != null) UnityJobQueue.Instance.Push(() => Scene.AddRoomUI(info));
    }

    public void RemoveRoom(uint roomNumber)
    {
        lock (_lock)
        {
            _ = rooms.Remove(roomNumber);
        }

        if (Scene != null) UnityJobQueue.Instance.Push(() => Scene.RemoveRoomUI(roomNumber));
    }

    public void Refresh(CRoomListRes res)
    {
        lock (_lock)
        {
            // TODO : more effective?
            foreach (var info in res.Rooms)
            {
                if (rooms.ContainsKey(info.RoomNumber) == false)
                {
                    rooms.Add(info.RoomNumber, new Room(info));
                }
            }

            var keysToRemove = rooms.Keys.Except(res.Rooms.Select(info => info.RoomNumber)).ToList();
            foreach (var key in keysToRemove)
            {
                rooms.Remove(key);
            }
        }

        if (Scene != null) UnityJobQueue.Instance.Push(() => Scene.RefreshRoomList(res.LoadTime));
    }

    public List<Room> GetRooms()
    {
        lock (_lock)
        {
            return rooms.Values.ToList();
        }
    }

    public bool TryGetRoom(uint roomId, out Room room)
    {
        return rooms.TryGetValue(roomId, out room);
    }

    public void CheckSend(int chatId, uint roomNumber, bool success)
    {
        if (rooms.TryGetValue(roomNumber, out Room room))
        {
            room.DoAction(() => room.UI.CheckPendingChat(chatId, success));
        }
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
        rooms.Clear();
    }

    void IManager.InitManager()
    {
    }
}
