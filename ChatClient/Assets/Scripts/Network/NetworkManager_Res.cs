using Chat;
using Core;
using ServerCoreTCP.Utils;
using UnityEngine;

public partial class NetworkManager : IManager, IUpdate
{
    public void ResLogin(CLoginRes res)
    {
        Debug.Log($"LoginRes: {res}"); // TEMP
        SetLoginned(res.UserInfo);
        UnityJobQueue.Instance.Push(() => ConnectingUI.Hide());
        
        switch (res.LoginRes)
        {
            case LoginRes.LoginInvalid:
                break;
            case LoginRes.LoginSuccess:
                SRoomListReq roomListReq = new SRoomListReq()
                {
                    UserInfo = UserInfo,
                };
                Send(roomListReq);
                break;
            case LoginRes.LoginFailed:
                break;
            case LoginRes.LoginError:
                break;
        }
    }

    public void ResEditUserName(CEditUserNameRes res)
    {
        UnityJobQueue.Instance.Push(() => ConnectingUI.Hide());

        if (res.Res == EditUserNameRes.EditOk)
        {
            UnityJobQueue.Instance.Push(() =>
            {
                MainScene scene = ManagerCore.Scene.GetScene<MainScene>();
                if (scene != null)
                {
                    scene.ResEditUserName(true, res.NewUserName);
                }
            });
        }
        else
        {
            UnityJobQueue.Instance.Push(() =>
            {
                MainScene scene = ManagerCore.Scene.GetScene<MainScene>();
                if (scene != null)
                {
                    scene.ResEditUserName(false);
                }
            });
        }
    }

    public void ResRoomList(CRoomListRes res)
    {
        UnityJobQueue.Instance.Push(() => ConnectingUI.Hide());

        Debug.Log(res); // TEMP

        ManagerCore.Room.Refresh(res);
    }

    public void ResCreateRoom(CCreateRoomRes res)
    {
        UnityJobQueue.Instance.Push(() => LoadingUI.Hide());
        Debug.Log(res); // TEMP

        // Note: if res is not ok, the roominfo is null.
        if (res.Res != CreateRoomRes.CreateRoomOk)
        {
            UnityJobQueue.Instance.Push(() => NotificationUI.Show($"Entering the room [{res.RoomNumber}] is failed: {res}."));
            return;
        }

        Utils.AssertCrash(res.RoomInfo != null);

        UnityJobQueue.Instance.Push(() => NotificationUI.Show($"The new room is created! [{res.RoomInfo.RoomNumber}]"));
        UnityJobQueue.Instance.Push(() =>
        {
            var scene = ManagerCore.Scene.GetScene<MainScene>();
            if (scene != null) scene.ResCreateRoom();
        });
    }

    public void ResEnterRoom(CEnterRoomRes res)
    {
        UnityJobQueue.Instance.Push(() => LoadingUI.Hide());
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
        UnityJobQueue.Instance.Push(() =>
        {
            var scene = ManagerCore.Scene.GetScene<MainScene>();
            if (scene != null) scene.ResEnterRoom();
        });
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
