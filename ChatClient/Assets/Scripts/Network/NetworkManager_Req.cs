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
        //int rand = Random.Range(0, 10000);
        int rand = int.Parse(DateTime.Now.ToString("HHmmss"));
        AuthToken = "TestToken";
        UserInfo = new UserInfo() { UserLoginId = $"TestId{rand}", UserName = $"TestName{rand}" };
        SLoginReq loginReq = new SLoginReq()
        {
            AuthToken = AuthToken,
            UserInfo = UserInfo
        };
        session.Send(loginReq);
    }

    public void ReqRoomList()
    {
        SRoomListReq roomListReq = new SRoomListReq()
        {
            UserInfo = UserInfo,
        };
        Send(roomListReq);
    }


}
