using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerCoreTCP.MessageWrapper;
using System.Net;
using System;
using Chat;
using ServerCoreTCP.Core;
using Core;
using ServerCoreTCP;
using Google.Protobuf;
using System.Threading;

public class ServerSession : PacketSession
{
    public long PingTick { get; private set; } = 0;

    Coroutine pingTask;

    IEnumerator SendPing(float intervalSeconds)
    {
        yield return null;

        while (true)
        {
            PingTick = Global.G_Stopwatch.ElapsedTicks;
            Send(new SPingPacket());

            yield return new WaitForSeconds(intervalSeconds);
        }
    }

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
        Debug.Log($"Connected to server: {endPoint}");

        // TODO
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
