using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StartScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        Screen.SetResolution(300, 100, false);

        SceneType = SceneTypes.Start;

        ManagerCore.UI.ShowSceneUIAsync<StartSceneUI>(AddrKeys.StartSceneUI);
    }

    public void TestLogin()
    {
        // TEST
        ManagerCore.Scene.LoadScene(SceneTypes.Main);
    }

    public void ReqLogin()
    {
        ConnectingUI.Show();

        // TODO : Request Login to login server
    }

    public void ReqRegister()
    {
        ConnectingUI.Show();

        // TODO : Request Register to login server
    }

    public void ResLogin()
    {
        ConnectingUI.Hide();

        // TODO : res 贸府
    }

    public void ResRegister()
    {
        ConnectingUI.Hide();

        // TODO : res 贸府
    }
}
