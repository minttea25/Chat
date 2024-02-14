using Core;
using System.IO;
using UnityEngine;

public class WebConfig
{
    public string AccountWebServerBaseUrl { get; set; }
    public string LoginUrl { get; set; }
    public string RegisterUrl { get; set; }

    public bool Validate()
    {
        return !string.IsNullOrEmpty(AccountWebServerBaseUrl) && !string.IsNullOrEmpty(LoginUrl) && !string.IsNullOrEmpty(RegisterUrl);
    }
}

public class ReleaseConfig
{
    public static WebConfig GetReleaseConfigs()
    {
        if (File.Exists(AppConst.ConfigPath) == false)
        {
            ManagerCore.Error.HandleError(301, ErrorManager.ErrorLevel.Critical, $"Can not find base config file: {AppConst.ConfigPath}");
            return null;
        }

        string text = File.ReadAllText(AppConst.ConfigPath);
        WebConfig config = Newtonsoft.Json.JsonConvert.DeserializeObject<WebConfig>(text);

        if (config.Validate() == false)
        {
            ManagerCore.Error.HandleError(302, ErrorManager.ErrorLevel.Critical, $"The configs contain empty values: {AppConst.ConfigPath}");
            return null;
        }

        return config;
    }
}
