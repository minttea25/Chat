using Chat;
using Core;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using ServerCoreTCP.Core;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class MainScene : BaseScene
{
    List<RoomInfo> rooms = new List<RoomInfo>();
    MainSceneUI ui = null;
    Timestamp roomRefreshTime = null;



    protected override void Init()
    {
        base.Init();

        SceneType = SceneTypes.Main;

        Screen.SetResolution(800, 600, false);

        ManagerCore.UI.ShowSceneUIAsync<MainSceneUI>(AddrKeys.MainSceneUI,
            (ui) =>
            {
                this.ui = ui;
                ConnectingUI.Show();
                ManagerCore.Network.StartService(ConnectionFailed);
            });
    }

    public void RefreshRoomList(Timestamp time, RepeatedField<RoomInfo> rooms)
    {
        this.rooms.Clear();

        roomRefreshTime = time;
        ui.SetRefreshTime(time);
        foreach(var room in rooms)
        {
            this.rooms.Add(room);
        }
        ui.RefreshRoomList(this.rooms);
    }

    void ConnectionFailed(SocketError error)
    {
        Debug.Log($"connection failed: {error}");

        // TODO : show error popup
        // TODO : retry to connect
    }
}
