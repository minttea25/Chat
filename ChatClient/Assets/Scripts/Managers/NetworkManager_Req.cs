using Chat;
using Core;
using Google.Protobuf.WellKnownTypes;
using ServerCoreTCP.Utils;
using System;
using UnityEngine;

public partial class NetworkManager : IManager, IUpdate
{
    public void ReqLogin()
    {
        ConnectingUI.Show();

        SLoginReq loginReq = new SLoginReq()
        {
            AccountDbId = AccountDbId,
            AuthToken = AuthToken,
            Ipv4Address = NetworkUtils.GetIPv4Address(),
        };

        // TEST
        Debug.Log($"Trying to login with [{loginReq}]");

        session.Send(loginReq);
    }

    public void ReqLogin_()
    {
        UnityJobQueue.Instance.Push(ConnectingUI.Show);

        SLoginReq loginReq = new SLoginReq()
        {
            AccountDbId = AccountDbId,
            AuthToken = AuthToken,
            Ipv4Address = NetworkUtils.GetIPv4Address(),
        };

        // TEST
        Debug.Log($"Trying to login with [{loginReq}]");

        session.Send(loginReq);
    }

    public void ReqEditUserName(string userName)
    {
        ConnectingUI.Show();

        SEditUserNameReq req = new SEditUserNameReq()
        {
            NewUserName = userName,
            UserInfo = UserInfo,
        };
        session.Send(req);
    }

    public void ReqRoomList()
    {
        ConnectingUI.Show();

        SRoomListReq roomListReq = new SRoomListReq()
        {
            UserInfo = UserInfo,
        };
        Send(roomListReq);
    }

    public void ReqCreateRoom(uint roomNumber)
    {
        LoadingUI.Show();

        SCreateRoomReq req = new() 
        { 
            RoomNumber = roomNumber, RoomName = $"Room{roomNumber}"
        };
        Send(req);
    }

    public void ReqEnterRoom(uint roomNumber)
    {
        LoadingUI.Show();

        Debug.Log($"Enter room {roomNumber}");
        SEnterRoomReq req = new()
        {
            RoomNumber = roomNumber,
        };
        Send(req);
    }

    public void ReqLeaveRoom(uint roomNunber)
    {
        SLeaveRoomReq req = new() { RoomNumber = roomNunber, UserInfo = UserInfo, };
        Send(req);
    }

    public void ReqSendChatText(string message, uint roomNumber, int chatId)
    {
        SSendChatText req = new()
        {
            RoomNumber = roomNumber,
            Chat = new Chat.ChatText()
            {
                Msg = message,
                ChatBase = new Chat.ChatBase()
                {
                    ChatType = ChatType.Text,
                    Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                    // it has no db id
                }
            },
            SenderInfo = UserInfo,
            ChatId = chatId,
        };
        Send(req);
    }
}
