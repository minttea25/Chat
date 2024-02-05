using Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ErrorManager : IManager
{
    public enum ErrorLevel
    {
        Info = 0, // for debug
        Warning = 1, // for debug
        Error = 2, // re-start? => quit
        Critical = 3, // quit
    }

    public void HandleError(int code, ErrorLevel level, string additionalMessage = null)
    {
        Debug.LogError($"[{code}] [{level}] {additionalMessage}");

        ManagerCore.UI.ShowPopupUIAsync<AlertPopupUI>(AddrKeys.AlertPopupUI, true,
            (ui) =>
            {
#if UNITY_EDITOR
                if (errors.ContainsKey(code) == false)
                {
                    ui.Setup($"Unknown error code: {code}, {additionalMessage}", "Ok");
                    return;
                }
                if (level == ErrorLevel.Info || level == ErrorLevel.Warning)
                {
                    ui.Setup($"{errors[code]} {additionalMessage ?? ""}", "Ok");
                }
                else ui.Setup($"{errors[code]} {additionalMessage ?? ""}", "Ok", Application.Quit);
#else
                if (level == ErrorLevel.Info || level == ErrorLevel.Warning)
                {
                    ui.Setup($"ErrorCode: {code} {additionalMessage ?? ""}", "Ok");
                }
                else ui.Setup($"ErrorCode: {code} {additionalMessage ?? ""}", "Ok", Application.Quit);
#endif

            });
    }


#if UNITY_EDITOR
    [Serializable]
    class ErrorCodes
    {
        public Dictionary<int, string> Errors { get; set; }
    }

    Dictionary<int, string> errors = null;
#endif


    public void LoadErrorFile()
    {
#if UNITY_EDITOR
        if (File.Exists(AppConst.ErrorFilePath) == false)
        {
            //CreateNewSample();
            Debug.LogError($"Can not find file: {AppConst.ErrorFilePath}");
            return;
        }

        string text = File.ReadAllText(AppConst.ErrorFilePath);

        errors = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorCodes>(text)?.Errors;

        if (errors == null)
        {
            Debug.LogError("Deserialization failed at Errors");
        }
#endif
    }

#if UNITY_EDITOR
    public void CreateNewSample()
    {
        ErrorCodes errorCodes = new ErrorCodes();
        errorCodes.Errors = new Dictionary<int, string>()
        {
            { 101, "Failed to load Network Config Scriptable Object." },
            { 102, "Port was 0." },
            { 103, "Enpoint was empty." },

            { 201, "WebLoginReq is not valid." },
            { 202, "WebRegisterReq is not valid" },
            { 203, "Failed to connect to account server." },

            { 301, "Can not find config file." },
            { 302, "Config has wrong values." },

            { 401, "Failed to connect to chat server." },
            { 402, "Disconnected before starting chat." },
            { 403, "Timeout for login." },

            { 501, "The chat you sent was not found." },

            { 1001, "Failed to load resources at MainScene." },
            { 1002, "Failed to load MainSceneUI at MainScene." },
        };

        string text = Newtonsoft.Json.JsonConvert.SerializeObject(errorCodes);
        File.WriteAllText(AppConst.ErrorFilePath, text);
    }
#endif
    void IManager.ClearManager()
    {
    }

    void IManager.InitManager()
    {
    }
}
