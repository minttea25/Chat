using Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

// TODO : not tested yet

[Serializable]
public class ChatCacheData
{
    // 4�� �� 1���� �� ������ �ֱ� (�������� null)
    public ChatDataType ChatDataType { get; set; }
    public ChatUserEnter ChatUserEnter { get; set; }
    public ChatUserLeave ChatUserLeave { get; set; }
    public ChatText ChatText { get; set; }
    public ChatIcon ChatIcon { get; set; }

    public void FromChatData(ChatData chatData)
    {
        ChatDataType = chatData.DataType;
        ChatUserEnter = null;
        ChatUserLeave = null;
        ChatText = null;
        ChatIcon = null;
        switch (chatData.DataType)
        {
            case ChatDataType.Text:
                ChatText = chatData as ChatText;
                break;
            case ChatDataType.Icon:
                ChatIcon = chatData as ChatIcon;
                break;
            case ChatDataType.Enter:
                ChatUserEnter = chatData as ChatUserEnter;
                break;
            case ChatDataType.Leave:
                ChatUserLeave = chatData as ChatUserLeave;
                break;
        }
    }
}

[Serializable]
public class ChatCaches
{
    public DateTime LastUpdate { get; set; }

    public List<ChatCacheData> Data { get; set; } = new();
}

public class ChatManager : IManager
{
    /// <summary>
    /// ���Ӱ� ������ ä�õ�
    /// </summary>
    Dictionary<uint, ChatCaches> addedChats = new Dictionary<uint, ChatCaches>();

    // �ϴ� ä�� ������ �ʹ� ���� ���� �������� �ʵ��� �Ѵ�.

    public void AddChat(uint room, ChatData chat)
    {
        if (addedChats.TryGetValue(room, out var cache) == false)
        {
            cache = new ChatCaches();
            addedChats.Add(room, cache);
        }
        var data = new ChatCacheData();
        data.FromChatData(chat);
        cache.Data.Add(data);


        // update the time of last update
        cache.LastUpdate = DateTime.Now;
    }

    /// <summary>
    /// Return the cached chat data. If no data, return null.
    /// </summary>
    /// <param name="roomNumber"></param>
    /// <returns></returns>
    public void LoadCachedChats(uint roomNumber, ChatPanelItem panel)
    {
        string filename = string.Format(AppConst.ChatCacheFileFormat, roomNumber);

        if (File.Exists(filename) == false)
        {
            //ErrorHandling.HandleError(ErrorHandling.ErrorType.Null,
            //    ErrorHandling.ErrorLevel.Error,
            //    $"Can not find the chat cache file: {filename}");
            return;
        }

        using (var fileStream = File.OpenRead(filename))
        using (var streamReader = new StreamReader(fileStream))
        using (var jsonReader = new JsonTextReader(streamReader))
        {
            var serializer = new JsonSerializer();

            while (jsonReader.Read())
            {
                ChatCaches chats = serializer.Deserialize<ChatCaches>(jsonReader);
                foreach (var c in chats.Data)
                {
                    panel.AddChat(c);
                }
            }
        }
    }

    void SaveCache()
    {
        foreach (var kv in addedChats)
        {
            uint room = kv.Key;
            var data = kv.Value;

            string filename = string.Format(AppConst.ChatCacheFileFormat, room);

            ChatCaches caches = new ChatCaches();
            if (File.Exists(filename) == true)
            {
                // ĳ���� ������ �̹� ���� ��� �ش� ������ �ε�
                using var fileStream = File.OpenRead(filename);
                using var streamReader = new StreamReader(fileStream);
                using var jsonReader = new JsonTextReader(streamReader);
                var serializer = new JsonSerializer();

                while (jsonReader.Read())
                {
                    // once
                    caches = serializer.Deserialize<ChatCaches>(jsonReader);
                }
            }

            // ���� ������ �߰�
            foreach (ChatCacheData chat in data.Data)
            {
                caches.Data.Add(chat);
            }
            caches.LastUpdate = data.LastUpdate;

            string text = JsonConvert.SerializeObject(caches);
            File.WriteAllText(filename, text);
        }
    }

    void IManager.ClearManager()
    {
        SaveCache();
    }

    void IManager.InitManager()
    {
    }
}
