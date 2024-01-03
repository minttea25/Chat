using Chat;
using Core;
using Google.Protobuf.WellKnownTypes;
using ServerCoreTCP.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NetworkManager : IManager, IUpdate
{
    public void ReqLogin()
    {
        ConnectingUI.Show();
        
        SLoginReq loginReq = new SLoginReq()
        {
            AuthToken = AuthToken,
            UserInfo = UserInfo
        };
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

    public void ReqCreateRoom(ulong roomNumber)
    {
        LoadingUI.Show();

        Debug.Log("TODO : ReqCreateRoom");
        SCreateRoomReq req = new() 
        { 
            RoomNumber = roomNumber, RoomName = $"Room{roomNumber}"
        };
        Send(req);
    }

    public void ReqEnterRoom(ulong roomNumber)
    {
        LoadingUI.Show();

        Debug.Log($"Enter room {roomNumber}");
        SEnterRoomReq req = new()
        {
            RoomNumber = roomNumber,
        };
        Send(req);
    }

    public void ReqSendChatText(string message, ulong roomNumber, int chatId)
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
                    Timestamp = Timestamp.FromDateTime(DateTime.Now),
                    // it has no db id
                }
            },
            SenderInfo = UserInfo,
            ChatId = chatId,
        };
        Send(req);
    }
}
