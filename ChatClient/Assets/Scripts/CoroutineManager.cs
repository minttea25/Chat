using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    // string으로만 관리되는 코루틴
    static Dictionary<string, Coroutine> coroutines = new Dictionary<string, Coroutine>();
    static CoroutineManager instance;

    void Awake()
    {
        // make unity - singleton
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public static Coroutine StartCoroutineEx(IEnumerator routine)
    {
        return instance.StartCoroutine(routine);
    }

    public static Coroutine StartCoroutineEx(IEnumerator routine, string name)
    {
        var co = instance.StartCoroutine(routine);
        coroutines.Add(name, co);
        return co;
    }

    public static void StopCoroutineEx(Coroutine routine)
    {
        instance.StopCoroutine(routine);
    }
    public static void StopCoroutineEx(string name)
    {
        if (coroutines.TryGetValue(name, out var co))
        {
            instance.StopCoroutine(co);
        }
    }

    public static void StopAllCoroutinesEx()
    {
        instance.StopAllCoroutines();
        coroutines.Clear();
    }
}
