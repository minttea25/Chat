using Chat;
using Core;
using ServerCoreTCP.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = SceneTypes.Main;

        // TEST
        ManagerCore.Network.StartService();
    }

    // TEMP
    // �ٸ����� ȣ���ؾ� �����
    
    

    public Coroutine StartCoroutineEx(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }
}
