using Core;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
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

    public void ReqAccountLogin(string id, string password)
    {
        // TODO : encrypt password
        AccountLoginWebReq data = new AccountLoginWebReq(id, password);
        // send login web request and get auth token
        var suc = ManagerCore.Web.SendLoginRequest(data, ResAccountLogin);
        if (suc == false)
        {
            // TODO : data is invalid.
            // ReqLogin ȣ�� ������ string üũ �ϱ� ������ ���� ȣ�� �� �� ����!
            return;
        }

        ConnectingUI.Show();
    }

    public void ReqAccountRegister(string id, string password)
    {
        // TODO : encrypt password
        CreateAccountWebReq data = new CreateAccountWebReq()
        {
            AccountId = id,
            AccountPassword = password
        };
        // send login web request and get auth token
        var suc = ManagerCore.Web.SendRegisterRequest(data, ResAccountRegister);
        if (suc == false)
        {
            // TODO : data is invalid.
            // ReqLogin ȣ�� ������ string üũ �ϱ� ������ ���� ȣ�� �� �� ����!
            return;
        }

        ConnectingUI.Show();
    }

    public void ResAccountLogin(AccountLoginWebRes res)
    {
        ConnectingUI.Hide();

        // TEMP
        Debug.Log(res);

        switch (res.Res)
        {
            // successful
            case 0:
                ManagerCore.Network.AccountServerConnected(res.AccountDbId, res.AuthToken);
                
                // ä�� ���� ����
                
                ManagerCore.Network.StartService(ConnectionFailed);
                break;
            // --------- failed -----------
            case 1:
                // TODO : failed to login
                Debug.LogError("Failed to login to account server.");
                break;
            default:
                break;
        }
    }

    public void ResAccountRegister(CreateAccountWebRes res)
    {
        ConnectingUI.Hide();

        // TEMP
        Debug.Log(res);

        switch (res.Res)
        {
            // successful
            case 0:
                break;
            // failed
            case 1:
                break;
        }
    }

    public void OnChatServerConnected()
    {
        // ä�� ������ ���� �� �α��� �õ�
        ConnectingUI.Hide();
        TryToLoginChatServer();
    }

    void ConnectionFailed(SocketError error)
    {
        Debug.Log($"connection failed: {error}");

        // TODO : show error popup
        // TODO : retry to connect
        NotificationUI.Show($"Connection Failed: {error}");
    }

    

    public void TryToLoginChatServer()
    {
        ManagerCore.Network.ReqLogin();

        StartCoroutine(CheckLoginTimeout());
    }

    public void ChatServerLoginSucceess()
    {
        // ä�� ���� �α��� ���� => main scene �ε�
        ManagerCore.Scene.LoadScene(SceneTypes.Main);
    }

    IEnumerator CheckLoginTimeout()
    {
        yield return new WaitForSeconds(AppConst.LoginChatServerTimeoutSeconds);

        if (ManagerCore.Network.Connection != NetworkManager.ConnectState.Loginned)
        {
            string msg = $"Failed to login: {ManagerCore.Network.Connection}";
            ErrorHandling.HandleError(ErrorHandling.ErrorType.Network, ErrorHandling.ErrorLevel.Error, msg);

            // failed to connect
            ManagerCore.Network.Logout();
        }

    }
}
