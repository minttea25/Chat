using System;

public static class Extension
{
    public static string ToLocalTimeFormat(this DateTime dateTime, string format = "HH:mm")
    {
        return ChatUtils.ToLocalTimeFormat(dateTime, format);
    }
}
