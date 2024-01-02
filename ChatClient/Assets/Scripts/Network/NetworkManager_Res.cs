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

    public void ResEditUserName(CEditUserNameRes res)
    {
        ConnectingUI.Hide();

        // TODO
        Debug.Log("TODO : ResEditUserName");
    }

    public void ResRoomList(CRoomListRes res)
    {
        ConnectingUI.Hide();

        Debug.Log(res);

        ManagerCore.Scene.GetScene<MainScene>().RefreshRoomList(res.LoadTime, res.Rooms);
        
    }

    public void ResCreateRoom(CCreateRoomRes res)
    {
        LoadingUI.Hide();

        Debug.Log(res);
        NotificationUI.Show($"The new room is created! [{res.RoomInfo.RoomNumber}]");
    }

    public void ReqEnterRoom(CEnterRoomRes res)
    {
        LoadingUI.Hide();

        Debug.Log(res);
        NotificationUI.Show($"You entered the room [{res.RoomInfo.RoomNumber}]");

        // add ui at room list
        ManagerCore.Scene.GetScene<MainScene>().AddRoom(res.RoomInfo);
    }
}
