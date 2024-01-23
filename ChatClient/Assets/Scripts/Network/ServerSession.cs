using Chat;
using Core;
using Google.Protobuf;
using ServerCoreTCP;
using ServerCoreTCP.MessageWrapper;
using System;
using System.Net;
using UnityEngine;

public class ServerSession : PacketSession
{

    void QueuePacket(ushort msgType, Session _, IMessage message)
    {
        MessageQueue.Instance.Push(new MessagePacket() { Message = message, Type = msgType });
    }

    public override void InitSession()
    {
        ;
    }

    public override void OnConnected(EndPoint endPoint)
    {
        ManagerCore.Network.SetConnected();
        Debug.Log($"Connected to server: {endPoint}");

        UnityJobQueue.Instance.Push(ManagerCore.Scene.GetScene<StartScene>().OnChatServerConnected);
        UnityJobQueue.Instance.Push(ManagerCore.Network.PingPong);
    }

    public override void OnDisconnected(EndPoint endPoint, object error = null)
    {
        Debug.Log($"Disconnected.");
    }

    public override void OnRecv(ReadOnlySpan<byte> buffer)
    {
        //Debug.Log($"OnRecv: {buffer.Length} bytes");
        MessageManager.Instance.OnRecvPacket(this, buffer, QueuePacket);
    }

    public override void OnSend(int numOfBytes)
    {
        //Debug.Log($"OnSend: {numOfBytes} bytes");
        if (numOfBytes <= 0) Debug.LogError("Sent byte was 0");
    }

    public override void PreSessionCleanup()
    {
        ;
    }
}
