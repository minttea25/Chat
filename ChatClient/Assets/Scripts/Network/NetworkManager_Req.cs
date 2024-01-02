using Chat;
using Core;
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
    }
}
