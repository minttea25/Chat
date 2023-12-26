using Chat;
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

    IPEndPoint endPoint = null;

    public uint ConnectedId { get; private set; } = 0;
    public string AuthToken { get; private set; } = null;
    public bool Connected { get; set; } = false;
    public UserInfo UserInfo { get; private set; }

    ClientService client = null;
    public ServerSession session = null;

    Coroutine pingTask = null;

    

    #region Service
    /// <summary>
    /// Connect to server
    /// </summary>
    public void StartService(Action<SocketError> failedCallback = null)
    {
        Utils.AssertCrash(endPoint != null);

        ClientServiceConfig config = ClientServiceConfig.GetDefault();

        client = new ClientService(
            endPoint, () => { session = new ServerSession(); return session; },
            config, failedCallback ?? ConnectFailed);

        client.Start();
    }

    /// <summary>
    /// Disconnect
    /// </summary>
    public void StopService()
    {
        // disconnect
        session?.Disconnect();
        client?.Stop();
        Connected = false;
    }

    public void Send<T>(T message) where T : IMessage
    {
        session?.Send(message);
    }


    void ConnectFailed(SocketError error)
    {
        Connected = false;
        Debug.LogError($"[{error}]Can not connect to server: {endPoint}");

        // TODO : 재연결 question popup
    }
    #endregion

    void IManager.ClearManager()
    {
        CoroutineManager.StopCoroutineEx(pingTask);

        endPoint = null;
        session = null;
        client = null;
    }

    void IManager.InitManager()
    {
        MessageManager.Instance.Init();

        // TODO: config를 통해 endpoint 지정
        // TEMP
        string host = Dns.GetHostName(); // local host name of my pc
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        endPoint = new IPEndPoint(address: ipAddr, port: 8888);

        CoreLogger.CreateLoggerWithFlag((uint)CoreLogger.LoggerSinks.FILE, LoggerConfig.GetDefault());
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
            // TODO : use constant
            yield return new WaitForSeconds(3f);

            PingTick = Global.G_Stopwatch.ElapsedMilliseconds;
            ManagerCore.Network.session.Send(new SPingPacket());
        }
    }
}
