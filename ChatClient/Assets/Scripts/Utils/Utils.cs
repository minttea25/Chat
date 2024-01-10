using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatUtils
{
    public static string ToLocalTimeFormat(DateTime time, string format = "HH:mm")
    {
        return time.ToLocalTime().ToString(format);
    }
}
