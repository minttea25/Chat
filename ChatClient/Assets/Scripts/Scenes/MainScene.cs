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

    //List<RoomInfo> rooms = new List<RoomInfo>();
    MainSceneUI ui = null;
    DateTime roomRefreshTime;



    protected override void Init()
    {
        base.Init();

        SceneType = SceneTypes.Main;

        Screen.SetResolution(800, 600, false);

        ManagerCore.UI.ShowSceneUIAsync<MainSceneUI>(AddrKeys.MainSceneUI,
            (ui) =>
            {
                this.ui = ui;
                ui.Scene = this;
                ConnectingUI.Show();
                ManagerCore.Network.StartService(ConnectionFailed);
            });
    }

    // TEMP
    public void TryLogin()
    {
        int rand = int.Parse(DateTime.Now.ToString("HHmmss"));
        ManagerCore.Network.SetUserInfo("TestToken", $"TestLoginId{rand}", $"TestName{rand}");
        ManagerCore.Network.ReqLogin();
    }

    public void ReqEditUserName(string userName)
    {
        Debug.Log("TODO : send edit req");
    }

    public void Logout()
    {
        ManagerCore.Network.LogoutAndQuit();
    }

    public void CloseRoomPopup()
    {
        UI.CloseRoomPopups();
    }

    public void AddRoomUI(RoomInfo roomInfo)
    {
        UI.AddRoomList(roomInfo);
    }

    public void RemoveRoomUI(ulong roomNumber)
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

    void ConnectionFailed(SocketError error)
    {
        Debug.Log($"connection failed: {error}");

        // TODO : show error popup
        // TODO : retry to connect
    }
}
