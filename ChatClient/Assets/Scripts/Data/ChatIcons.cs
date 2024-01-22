using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatEmoticonData
{
    // TEST
    //public Dictionary<uint, string> Emoticons = new Dictionary<uint, string>()
    //{
    //    { 1, AddrKeys.Icon_Party },
    //    { 2, AddrKeys.Icon_Laughing },
    //    { 3, AddrKeys.Icon_Smile },
    //    { 4, AddrKeys.Icon_Kirakira },
    //    { 5, AddrKeys.Icon_Sad },
    //};

    public Dictionary<uint, string> Emoticons { get; set; }

    public Sprite GetEmoticon(uint id)
    {
        if (Emoticons.TryGetValue(id, out var key))
        {
            return ManagerCore.Resource.Get<Sprite>(key);
        }
        else return null;
        
    }
}
