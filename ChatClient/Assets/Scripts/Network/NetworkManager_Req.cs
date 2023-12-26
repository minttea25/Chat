using Chat;
using Core;
using ServerCoreTCP.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NetworkManager : IManager, IUpdate
{
    public void ReqLogin()
    {
        AuthToken = "TestToken";
        UserInfo = new UserInfo() { UserId = 111, UserName = "TestName111" };
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
