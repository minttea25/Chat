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
    public uint RoomNumber => Info.RoomNumber;
    public string RoomName => Info.RoomName;

    public bool Activated => ui != null && ui.gameObject.activeSelf;
    public ChatPanelItem UI => ui != null ? ui : throw new NullReferenceException();

    readonly object chatLock = new object();
    readonly object actionLock = new object();
    readonly List<ChatData> pendingChats = new List<ChatData>();
    readonly List<Action> pendingJobs = new List<Action>();
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
            lock (chatLock)
            {
                pendingChats.Add(chat);
            }
        }
    }

    public void DoAction(Action action)
    {
        if (Activated == true)
        {
            action?.Invoke();
        }
        else
        {
            lock (actionLock)
            {
                pendingJobs.Add(action);
            }
        }
    }

    public List<ChatData> GetPendingChats()
    {
        lock (chatLock)
        {
            var result = pendingChats;
            pendingChats.Clear();
            return result;
        }
    }

    public List<Action> GetPendingJobs()
    {
        lock (actionLock)
        {
            var result = pendingJobs;
            pendingJobs.Clear();
            return result;
        }
    }
}
