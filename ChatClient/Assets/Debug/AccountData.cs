using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "AccountData", menuName = "Test/AccountData")]
public class AccountData : ScriptableObject
{
    public string LoginId;

    public void LoadText()
    {
#if UNITY_EDITOR
        Debug.LogWarning("LoadText is for debugging in built version");
#endif
        string filename = "AccountData.txt";
        
        if (File.Exists(filename) == false)
        {
            int rand = int.Parse(DateTime.Now.ToString("HHmmss"));
            LoginId = $@"Test{rand}";
            File.WriteAllText(filename, LoginId);
        }
        else
        {
            var text = File.ReadAllText(filename);
            LoginId = text;
        }
    }
}
