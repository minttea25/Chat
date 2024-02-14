using Chat;
using Core;
using ServerCoreTCP.Utils;
using UnityEngine;

public partial class NetworkManager : IManager, IUpdate
{

    // Note : 일단 재연결에 대해 고려 안함

    public void ResLogin(CLoginRes res)
    {
        if (Connection == ConnectState.Loginned) return;

        Debug.Log($"LoginRes: {res}"); // TEMP
        SetLoginned(res.UserInfo);
        UnityJobQueue.Instance.Push(() => ConnectingUI.Hide());
        
        // TODO : failed 처리
        switch (res.LoginRes)
        {
            case LoginRes.LoginInvalid:
                break;
            case LoginRes.LoginSuccess:
                UnityJobQueue.Instance.Push(() => ManagerCore.Scene.GetScene<StartScene>().ChatServerLoginSucceess());
                break;
            case LoginRes.LoginFailed:
            case LoginRes.LoginError:
                NotificationUI.Show_Unity("Login Failed.");
                break;
        }
    }

    public void ResEditUserName(CEditUserNameRes res)
    {
        UnityJobQueue.Instance.Push(() => ConnectingUI.Hide());

        if (res.Res == EditUserNameRes.EditOk)
        {
            UserInfo.UserName = res.NewUserName;
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
            NotificationUI.Show_Unity($"Entering the room [{res.RoomNumber}] is failed: {res}.");
            return;
        }

        Utils.AssertCrash(res.RoomInfo != null);

        NotificationUI.Show_Unity($"The new room is created! [{res.RoomInfo.RoomNumber}]");
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
            NotificationUI.Show_Unity($"Entering the room [{res.RoomNumber}] is failed: {res}.");
            return;
        }

        Utils.AssertCrash(res.RoomInfo != null);

        UnityJobQueue.Instance.Push(() => NotificationUI.Show($"You entered the room [{res.RoomInfo.RoomNumber}]"));
        NotificationUI.Show_Unity($"You entered the room [{res.RoomInfo.RoomNumber}]");

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
            NotificationUI.Show_Unity($"Failed to send message: {res.Error}");


            ManagerCore.Room.CheckSend(res.ChatId, res.RoomNumber, success: false);
            return;
        }

        ManagerCore.Room.CheckSend(res.ChatId, res.RoomNumber, success: true);
    }
}
