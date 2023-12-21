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

        StartCoroutine(SendPing(3));
    }

    // TEMP
    // 다른데서 호출해야 깔끔함
    public long PingTick = 0;
    IEnumerator SendPing(float intervalSeconds)
    {
        yield return null;

        while (true)
        {
            yield return new WaitForSeconds(intervalSeconds);

            PingTick = Global.G_Stopwatch.ElapsedMilliseconds;
            ManagerCore.Network.session.Send(new SPingPacket());
        }
    }
}
