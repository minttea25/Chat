using Chat;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room
{
    readonly public RoomInfo Info;

    public ICollection<UserInfo> Users => Info.Users;
    public ulong RoomDbId => Info.RoomDbId;
    public ulong RoomNuber => Info.RoomNumber;
    public string RoomName => Info.RoomName;

    public bool Activated => ui != null && ui.gameObject.activeSelf;
    public ChatPanelItem UI => ui != null ? ui : throw new NullReferenceException();

    readonly object _lock = new object();
    readonly List<ChatData> _pendingChats = new List<ChatData>();
    ChatPanelItem ui = null;

    public Room(RoomInfo info)
    {
        Info = info;
    }

    public void AttachUI(ChatPanelItem ui)
    {
        this.ui = ui;
    }

    public void AddChat(ChatData chat)
    {
        if (Activated == true)
        {
            UI.AddChat(chat);
        }
        else
        {
            lock (_lock)
            {
                _pendingChats.Add(chat);
            }
        }
    }

    public List<ChatData> GetPendingChats()
    {
        lock ( _lock)
        {
            var result = _pendingChats;
            _pendingChats.Clear();
            return result;
        }
    }
}
