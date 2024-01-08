using Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChatManager : IManager
{
    Dictionary<uint, List<ChatData>> cachedChats = new();

    /// <summary>
    /// Return the cached chat data. If no data, return null.
    /// </summary>
    /// <param name="roomNumber"></param>
    /// <returns></returns>
    public List<ChatData> GetCachedChats(uint roomNumber)
    {
        string filename = string.Format(AppConst.ChatCacheFileFormat, roomNumber);

        if (File.Exists(filename) == false) return null;

        return null;
    }

    void IManager.ClearManager()
    {
        throw new System.NotImplementedException();
    }

    void IManager.InitManager()
    {
        throw new System.NotImplementedException();
    }
}
