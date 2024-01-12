using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorHandling
{
    public enum ErrorLevel
    {
        Info = 0,
        Warning = 1,
        Error = 2,
        Critical = 3,
    }

    public enum ErrorType
    {
        Null = 0,
        Logic = 1,
        Network = 2,
        Runtime = 3,
    }

#if UNITY_EDITOR
    public static void HandleError(ErrorType type, ErrorLevel level, string message = null, object target = null)
    {
        string error = null;
        if (target is not null && target is GameObject go)
        {
            error = string.Format("[{0}] {1} at {2}", type, message, go);
        }
        else
        {
            error = string.Format("[{0}] {1} [{2}]", type, message, target ?? string.Empty);
        }


        // use Debug log
        switch (level)
        {
            case ErrorLevel.Info:
                Debug.Log(error);
                break;
            case ErrorLevel.Warning:
                Debug.LogWarning(error);
                break;
            case ErrorLevel.Error:
                Debug.LogError(error);
                break;
            case ErrorLevel.Critical:
                Debug.LogError($"CRITICAL: {error}");
                Application.Quit();
                break;
        }
    }
#else
    public static void HandleError(ErrorType type, ErrorLevel level, string message = null)
    {

    }
#endif

}
