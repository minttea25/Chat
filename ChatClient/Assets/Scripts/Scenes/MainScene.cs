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
    // 다른데서 호출해야 깔끔함
    
    

    public Coroutine StartCoroutineEx(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }
}
