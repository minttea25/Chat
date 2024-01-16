using Chat;
using Chat.Network;
using Core;
using Google.Protobuf;
using ServerCoreTCP;
using ServerCoreTCP.CLogger;
using ServerCoreTCP.Core;
using ServerCoreTCP.Utils;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public partial class NetworkManager : IManager, IUpdate
{
    #region Singleton
    static NetworkManager instance = new NetworkManager();
    public static NetworkManager Instance => instance;
    #endregion

    public enum ConnectState
    {
        NotConnected = 0,
        Connecting = 1,
        Connected = 2,
        Loginned = 3,
        FailedToConnect = 4,
        Disconnected = 5,
    }

    IPEndPoint endPoint = null;

    public long AccountDbId { get; private set; } = 0;
    public uint ConnectedId { get; private set; } = 0;
    public string AuthToken { get; private set; } = null;
    public bool Connected => Connection == ConnectState.Connected || Connection == ConnectState.Loginned;
    public ConnectState Connection { get; private set; } = ConnectState.NotConnected;
    public UserInfo UserInfo { get; private set; }

    ClientService client = null;
    public ServerSession session = null;

    Coroutine pingTask = null;


    // TEST
    public static string TestUserName;

    public void AccountServerConnected(long accountDbId, string authToken)
    {
        AuthToken = authToken;
        AccountDbId = accountDbId;
    }

    public void SetConnected()
    {
        Connection = ConnectState.Connected;
    }

    /// <summary>
    /// UserInfo must have UserDbId.
    /// </summary>
    /// <param name="receivedUserInfo"></param>
    public void SetLoginned(UserInfo receivedUserInfo)
    {
        if (receivedUserInfo.UserDbId == 0)
        {
            Debug.LogError("Failed to get UserDbId from server.");
            return;
        }
        Connection = ConnectState.Loginned;
        UserInfo = new();
        UserInfo.MergeFrom(receivedUserInfo);
    }

    #region Service
    /// <summary>
    /// Connect to server
    /// </summary>
    public void StartService(Action<SocketError> failedCallback = null)
    {
        ConnectingUI.Show();
        Core.Utils.AssertCrash(endPoint != null);

        ClientServiceConfig config = ClientServiceConfig.GetDefault();

        client = new ClientService(
            endPoint, () => { session = new ServerSession(); return session; },
            config, failedCallback ?? ConnectFailed);

        Connection = ConnectState.Connecting;
        client.Start();
    }

    /// <summary>
    /// Disconnect
    /// </summary>
    public void StopService()
    {
        if (Connection == ConnectState.Disconnected) return;
        Logout();
    }

    public void Send<T>(T message) where T : IMessage
    {
        if (Connection != ConnectState.Loginned) return;
        session?.Send(message);
    }


    void ConnectFailed(SocketError error)
    {
        Connection = ConnectState.FailedToConnect;
        Debug.LogError($"[{error}]Can not connect to server: {endPoint}");

        // TODO : Àç¿¬°á question popup
    }
    #endregion

    public void Logout()
    {
        // disconnect
        session?.Disconnect();
        client?.Stop();
        Connection = ConnectState.Disconnected;
        //CoroutineManager.StopCoroutineEx(pingTask);
    }

    public void LogoutAndQuit()
    {
        Logout();
        Application.Quit();
    }

    void IManager.ClearManager()
    {
        endPoint = null;
        session = null;
        client = null;
    }

    void IManager.InitManager()
    {
        // Essential
        MessageManager.Instance.Init();

        var config = Resources.Load<NetworkConfig>(ResourcePath.NetworkConfig);
        if (config == null)
        {
            ErrorHandling.HandleError(ErrorHandling.ErrorType.Network, ErrorHandling.ErrorLevel.Critical, "Can not find NetworkConfig in Resource directory.");
            return;
        }

        if (config.UseLocal == true)
        {
            if (config.Port == 0)
            {
                ErrorHandling.HandleError(ErrorHandling.ErrorType.Network, ErrorHandling.ErrorLevel.Critical, "Port is 0.");
                return;
            }

            IPAddress ipAddr = NetworkUtils.GetLocalIPAddress();
            endPoint = new IPEndPoint(address: ipAddr, port: config.Port);
        }
        else
        {
            if (string.IsNullOrEmpty(config.EndpointIPAddress)
                || config.Port == 0)
            {
                ErrorHandling.HandleError(ErrorHandling.ErrorType.Network, ErrorHandling.ErrorLevel.Critical, "Empty endpoint.");
                return;
            }

            IPAddress ipHost = IPAddress.Parse(config.EndpointIPAddress);
            endPoint = new IPEndPoint(ipHost, config.Port);
        }

        CoreLogger.CreateLoggerWithFlag((uint)CoreLogger.LoggerSinks.FILE, LoggerConfig.GetDefault());

        Debug.Log($"Loaded: EndPoint: {endPoint}");
    }

    public void Update()
    {
        if (Connected == false) return;

        // send
        session?.FlushSend();

        // receive
        var packets = MessageQueue.Instance.PopAll();
        foreach (var packet in packets)
        {
            MessageManager.Instance.HandlePacket(packet.Type, packet.Message, session);
        }
    }

    public void PingPong()
    {
        pingTask = CoroutineManager.StartCoroutineEx(SendPing(), nameof(SendPing));
    }

    public long PingTick { get; private set;} = 0;

    IEnumerator SendPing()
    {
        yield return null;

        while (true)
        {
            yield return new WaitForSeconds(AppConst.SendPingIntervaleSeconds);

            PingTick = Global.G_Stopwatch.ElapsedMilliseconds;
            ManagerCore.Network.session.Send(new SPingPacket());
        }
    }
}
