using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Validations
{
    #region Inputs
    public static bool RoomNumber(string text, out uint result)
    {
        result = 0;
        if (string.IsNullOrEmpty(text)) return false;
        if (text.Length > 8) return false;
        if (uint.TryParse(text, out result) == false) return false;

        return true;
    }

    public static bool UserName(string text)
    {
        // TODO : validate user name

        return true;
    }
    #endregion
}
