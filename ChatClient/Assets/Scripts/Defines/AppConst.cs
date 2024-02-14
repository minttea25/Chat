using UnityEngine;

public class AppConst
{

    // TODO : ¿œ¥‹ json
    public const string ChatCacheFileFormat = "Chat_{0}.json";



    public readonly static string ConfigPath = $"{Application.streamingAssetsPath}/Config.json";
    public readonly static string ErrorFilePath = $"{Application.persistentDataPath}/Error.json";





    public const int WebResOk = 1;

    public const float SendPingIntervaleSeconds = 5f;

    public const float LoginChatServerTimeoutSeconds = 15f;

    public const int ProcessUnityJobQueuePerFrame = 20;

    public const int NewMessageVisibleMaxCount = 99;
}

public class ResourcePath
{
    public const string NetworkConfig = "NetworkConfig";
    public const string WebUrls = "WebUrls";
}
