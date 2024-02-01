using Core;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebManager : IManager
{
    public string BaseUrl => WebUrls.AccountWebServerBaseUrl;
    public WebUrls WebUrls { get; private set; } = null;

    public bool SendLoginRequest(AccountLoginWebReq account, Action<AccountLoginWebRes> resCallback)
    {
        if (account.Validate() == false)
        {
            ManagerCore.Error.HandleError(201, ErrorManager.ErrorLevel.Warning, "AccountLoginWebReq is invalid.");
            return false;
        }

        SendRequestPost(
            GetUrl(WebUrls.LoginUrl),
            account,
            resCallback);

        return true;
    }

    public bool SendRegisterRequest(CreateAccountWebReq account, Action<CreateAccountWebRes> resCallback)
    {
        if (account.Validate() == false)
        {
            ManagerCore.Error.HandleError(202, ErrorManager.ErrorLevel.Warning, "AccountLoginWebReq is invalid.");
            return false;
        }

        SendRequestPost(
            GetUrl(WebUrls.RegisterUrl),
            account,
            resCallback);

        return true;
    }

    string GetUrl(string url) => WebUrls.AccountWebServerBaseUrl + url;

    public void SendRequestPost<T>(string url, object data, Action<T> resCallback)
    {
        ManagerCore.Instance.StartCoroutine(SendWebRequestCo(url, UnityWebRequest.kHttpVerbPOST, data, resCallback));
    }

    public void SendRequestRaw<T>(string url, string method, object data = null, Action<T> resCallback = null)
    {
        // TODO : 
    }

    IEnumerator SendWebRequestCo<T>(string reqUrl, string method, object data, Action<T> resCallback)
    {
#if UNITY_EDITOR
        Debug.Log(data);
#endif

        byte[] json = null;
        if (data != null)
        {
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            json = Encoding.UTF8.GetBytes(jsonString);
        }

        using (UnityWebRequest req = new(reqUrl, method))
        {
            req.uploadHandler = new UploadHandlerRaw(json);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError
                || req.result == UnityWebRequest.Result.ProtocolError)
            {
                ManagerCore.Error.HandleError(203, ErrorManager.ErrorLevel.Info, "Can not connect to AccountServer.");

                NotificationUI.Show("Can not connect to AccountServer.");
                ConnectingUI.Hide();
            }
            else
            {
                T resData = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(req.downloadHandler.text);
#if UNITY_EDITOR
                Debug.Log(resData);
#endif
                resCallback.Invoke(resData);
            }
        }
    }


    void IManager.ClearManager()
    {
    }

    void IManager.InitManager()
    {
#if UNITY_EDITOR
        WebUrls = Resources.Load<WebUrls>(ResourcePath.WebUrls);
        if (WebUrls == null)
        {
            ManagerCore.Error.HandleError(203, ErrorManager.ErrorLevel.Critical, "Failed to load Configs.");
        }
        Debug.Log($"[Scriptable Object]Loaded WebUrls: {WebUrls}");
#else
        WebConfig config = ReleaseConfig.GetReleaseConfigs();
        if (config == null)
        {
            ManagerCore.Error.HandleError(203, ErrorManager.ErrorLevel.Critical, "Failed to load Configs.");
        }
        else
        {
            WebUrls = WebUrls.FromConfig(config);
        }
        Debug.Log($"[File] Loaded WebUrls: {WebUrls}  in {AppConst.ConfigPath}");
#endif
    }
}
