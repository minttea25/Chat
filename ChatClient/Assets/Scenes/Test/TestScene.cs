using Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        ManagerCore.Resource.LoadWithLabelAsync(AddrKeys.EmoticonLabel, (failed) =>
        {
            Debug.Log(failed.Count);
        });

        //var t = Newtonsoft.Json.JsonConvert.SerializeObject(new ChatEmoticonData());
        //File.WriteAllText("JsonTest.json", t);

        //StartCoroutine(nameof(Test));
    }
}
