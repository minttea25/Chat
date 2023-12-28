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
        ConnectingUI.Hide();

        Debug.Log($"LoginRes: {res.LoginRes}");
        UserInfo.UserDbId = res.UserInfo.UserDbId;
        switch (res.LoginRes)
        {
            case LoginRes.LoginInvalid:
                break;
            case LoginRes.LoginSuccess:
                ReqRoomList();
                break;
            case LoginRes.LoginFailed:
                break;
            case LoginRes.LoginError:
                break;
        }
    }

    public void ResRoomList(CRoomListRes res)
    {
        ConnectingUI.Hide();

        Debug.Log(res.LoadTime);
        Debug.Log($"ResRoomList: {res.Rooms.Count}");

        ManagerCore.Scene.GetScene<MainScene>().RefreshRoomList(res.LoadTime, res.Rooms);
        
    }

    public void ResCreateRoom(CCreateRoomRes res)
    {
        LoadingUI.Hide();

        Debug.Log(res);
    }

    public void ReqEnterRoom(CEnterRoomRes res)
    {
        LoadingUI.Hide();

        Debug.Log(res);
    }
}
