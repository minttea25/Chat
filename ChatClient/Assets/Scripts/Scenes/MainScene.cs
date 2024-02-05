using Chat;
using Core;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MainScene : BaseScene
{
    public MainSceneUI UI => ui != null ? ui : throw new System.NullReferenceException();

    MainSceneUI ui = null;
    DateTime roomRefreshTime;


    private void OnEnable()
    {
        LoadingUI.Show();
        ManagerCore.Resource.LoadWithLabelAsync(
            AddrKeys.Label_Main,
            ResourceLoadCompleted);
    }

    private void OnDisable()
    {
        ManagerCore.Resource.ReleaseWithLabel(AddrKeys.Label_Main);
    }


    protected override void Init()
    {
        base.Init();

        SceneType = SceneTypes.Main;

        Screen.SetResolution(UIValues.MainSceneResolutionWidth, UIValues.MainSceneResolutionHeight, UIValues.UseFullScreen);
    }

    void ResourceLoadCompleted(List<string> failedKeys)
    {
        if (failedKeys.Count != 0)
        {
            StringBuilder t = new StringBuilder("");
            foreach (string key in failedKeys) { t.Append($"{key}, "); }
            ManagerCore.Error.HandleError(1001, ErrorManager.ErrorLevel.Critical, $"Failed to load resources: {t}");
            return;
        }

        ui = ManagerCore.UI.ShowSceneUI<MainSceneUI>(AddrKeys.MainSceneUI);
        if (ui == null)
        {
            ManagerCore.Error.HandleError(1002, ErrorManager.ErrorLevel.Critical, "Failed to load MainSceneUI");
            return;
        }

        ui.Scene = this;

        LoadingUI.Hide();
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
            ManagerCore.Error.HandleError(401, ErrorManager.ErrorLevel.Info, "Failed to connect to chat server.");
        }

        if (ManagerCore.Network.Connection == NetworkManager.ConnectState.Disconnected)
        {
            ManagerCore.Error.HandleError(402, ErrorManager.ErrorLevel.Info, "Disconnected before starting application.");
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
