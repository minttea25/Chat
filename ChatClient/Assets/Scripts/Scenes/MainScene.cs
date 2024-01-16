using Chat;
using Core;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using ServerCoreTCP.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class MainScene : BaseScene
{
    public MainSceneUI UI => ui != null ? ui : throw new System.NullReferenceException();

    MainSceneUI ui = null;
    DateTime roomRefreshTime;


    private void OnEnable()
    {
        ManagerCore.Resource.LoadAllAsync(
            ResourceLoadCompleted,
            AddrKeys.MainSceneUI);
    }

    private void OnDisable()
    {
        ManagerCore.Resource.ReleaseAll(AddrKeys.MainSceneUI);
    }


    protected override void Init()
    {
        base.Init();

        SceneType = SceneTypes.Main;

        Screen.SetResolution(800, 600, false);

        //ManagerCore.UI.ShowSceneUIAsync<MainSceneUI>(AddrKeys.MainSceneUI,
        //    (ui) =>
        //    {
        //        this.ui = ui;
        //        ui.Scene = this;
        //        ConnectingUI.Show();
        //        ManagerCore.Network.StartService(ConnectionFailed);
        //    });
    }

    void ResourceLoadCompleted(List<string> failedKeys)
    {
        if (failedKeys.Count != 0)
        {
            ErrorHandling.HandleError(ErrorHandling.ErrorType.Runtime,
                ErrorHandling.ErrorLevel.Critical,
                "Failed to load resources");
            return;
        }

        ui = ManagerCore.UI.ShowSceneUI<MainSceneUI>(AddrKeys.MainSceneUI);
        if (ui == null)
        {
            ErrorHandling.HandleError(ErrorHandling.ErrorType.Runtime,
                ErrorHandling.ErrorLevel.Critical,
                "Failed to load MainSceneUI");
            return;
        }

        ui.Scene = this;
    }

    public void CheckLoadAllCompleted()
    {
        StartCoroutine(AllLoadCompleted());
    }

    IEnumerator AllLoadCompleted()
    {
        while (ManagerCore.Network.Connection == NetworkManager.ConnectState.Connecting)
        {
            yield return null;
        }

        if (ManagerCore.Network.Connection == NetworkManager.ConnectState.FailedToConnect)
        {
            // TODO : 연결 실패

        }

        if (ManagerCore.Network.Connection == NetworkManager.ConnectState.Disconnected)
        {
            // TODO : 오류
        }

        // req room list
        ManagerCore.Network.ReqRoomList();

    }
    

    public void ResCreateRoom()
    {
        UI.CloseCreateRoomPopup();
    }

    public void ResEnterRoom()
    {
        UI.CloseEnterRoomPopup();


    }

    public void ReqEditUserName(string userName)
    {
        ManagerCore.Network.ReqEditUserName(userName);
    }

    public void ResEditUserName(bool success, string username = null)
    {
        UI.EditUserNameRes(success, username);
    }

    public void Logout()
    {
        ManagerCore.Network.LogoutAndQuit();
    }

    public void AddRoomUI(RoomInfo roomInfo)
    {
        UI.AddRoomList(roomInfo);
    }

    public void RemoveRoomUI(uint roomNumber)
    {
        UI.RemoveRoom(roomNumber);
    }

    public void RefreshRoomList(Timestamp time)
    {
        DateTime now = time.ToDateTime().ToLocalTime();

        roomRefreshTime = now;
        ui.SetRefreshTime(now);

        ui.RefreshRoomList(now);
    }

    
}
