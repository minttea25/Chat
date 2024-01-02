using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static string ToLocalTimeFormat(this DateTime dateTime, string format = "HH:mm")
    {
        return ChatUtils.ToLocalTimeFormat(dateTime, format);
    }
}
