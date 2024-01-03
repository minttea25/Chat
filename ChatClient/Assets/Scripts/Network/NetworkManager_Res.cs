using Chat;
using Core;
using ServerCoreTCP.Utils;
using UnityEngine;

public partial class NetworkManager : IManager, IUpdate
{
    public void ResLogin(CLoginRes res)
    {
        ConnectingUI.Hide();

        Debug.Log($"LoginRes: {res.LoginRes}"); // TEMP
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

        Debug.Log(res); // TEMP

        ManagerCore.Room.Refresh(res);
    }

    public void ResCreateRoom(CCreateRoomRes res)
    {
        LoadingUI.Hide();
        Debug.Log(res); // TEMP

        // Note: if res is not ok, the roominfo is null.
        if (res.Res != CreateRoomRes.CreateRoomOk)
        {
            UnityJobQueue.Instance.Push(() => NotificationUI.Show($"Entering the room [{res.RoomNumber}] is failed: {res}."));
            return;
        }

        Utils.AssertCrash(res.RoomInfo != null);

        UnityJobQueue.Instance.Push(() => NotificationUI.Show($"The new room is created! [{res.RoomInfo.RoomNumber}]"));
    }

    public void ResEnterRoom(CEnterRoomRes res)
    {
        LoadingUI.Hide();
        Debug.Log(res); // TEMP

        // Note: if res is not ok, the roominfo is null.
        if (res.Res != EnterRoomRes.EnterRoomOk)
        {
            UnityJobQueue.Instance.Push(() => NotificationUI.Show($"Entering the room [{res.RoomNumber}] is failed: {res}."));
            return;
        }

        Utils.AssertCrash(res.RoomInfo != null);

        UnityJobQueue.Instance.Push(() => NotificationUI.Show($"You entered the room [{res.RoomInfo.RoomNumber}]"));
        ManagerCore.Room.AddRoom(res.RoomInfo);
    }

    public void ResSendChat(CSendChat res)
    {
        // no hidng ui

        // TEMP
        Debug.Log(res);

        if (res.Error != SendChatError.Success)
        {
            UnityJobQueue.Instance.Push(() => NotificationUI.Show($"Failed to send message: {res.Error}"));

            ManagerCore.Room.CheckSend(res.ChatId, res.RoomNumber, success: false);
            return;
        }

        ManagerCore.Room.CheckSend(res.ChatId, res.RoomNumber, success: true);
    }
}
