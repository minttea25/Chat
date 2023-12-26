using Chat;
using Core;
using ServerCoreTCP.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NetworkManager : IManager, IUpdate
{
    public void ResLogin(CLoginRes res)
    {
        Debug.Log($"LoginRes: {res.LoginRes}");
        switch (res.LoginRes)
        {
            case LoginRes.LoginInvalid:
                break;
            case LoginRes.LoginSuccess:
                ManagerCore.Network.ReqRoomList();
                break;
            case LoginRes.LoginFailed:
                break;
            case LoginRes.LoginError:
                break;
        }
    }

    public void ResRoomList(CRoomListRes res)
    {
        Debug.Log(res.LoadTime);
        Debug.Log($"ResRoomList: {res.Rooms.Count}");

        ManagerCore.Scene.GetScene<MainScene>().RefreshRoomList(res.LoadTime, res.Rooms);

        
    }
}
