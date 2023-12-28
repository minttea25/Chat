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
    }

    public void ReqEnterRoom(ulong roomNumber)
    {
        LoadingUI.Show();

        Debug.Log($"Enter room {roomNumber}");
    }
}
