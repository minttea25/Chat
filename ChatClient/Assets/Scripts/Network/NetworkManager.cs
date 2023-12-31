using Chat;
using Core;
using Google.Protobuf;
using JetBrains.Annotations;
using ServerCoreTCP;
using ServerCoreTCP.CLogger;
using ServerCoreTCP.Core;
using ServerCoreTCP.Utils;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetworkManager : IManager, IUpdate
{
    #region Singleton
    static NetworkManager instance = new NetworkManager();
    public static NetworkManager Instance => instance;
    #endregion

    IPEndPoint endPoint = null;

    public uint ConnectedId { get; private set; } = 0;
    public string AuthToken { get; private set; } = null;
    public bool Connected { get; private set; } = false;

    ClientService client = null;
    public ServerSession session = null;

    /// <summary>
    /// Connect to server
    /// </summary>
    public void StartService()
    {
        Utils.AssertCrash(endPoint != null);

        ClientServiceConfig config = ClientServiceConfig.GetDefault();

        client = new ClientService(
            endPoint, () => { session = new ServerSession(); return session; },
            config, ConnectFailed);

        client.Start();

        Connected = true;
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

    public void Send(IMessage message)
    {
        session?.Send(message);
    }


    void ConnectFailed(SocketError error)
    {
        Connected = false;
        Debug.LogError($"[{error}]Can not connect to server: {endPoint}");

        // TODO : 재연결 question popup
    }


    void IManager.ClearManager()
    {
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
}
