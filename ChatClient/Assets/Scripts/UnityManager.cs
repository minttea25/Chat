using Core;
using ServerCoreTCP.Utils;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class UnityJobQueue
{
    #region Singleton
    static readonly UnityJobQueue instance = new UnityJobQueue();
    public static UnityJobQueue Instance => instance;
    #endregion

    readonly ConcurrentQueue<Action> queue = new();

    public void Push(Action job)
    {
        queue.Enqueue(job);
    }

    public Action[] PopAll()
    {
        var arr = queue.ToArray();
        queue.Clear();
        return arr;
    }
}

public class UnityManager : IUpdate, IManager
{
    public void ClearManager()
    {
    }

    public void InitManager()
    {
        ;
    }

    public void Update()
    {
        var jobs = UnityJobQueue.Instance.PopAll();
        foreach (var job in jobs)
        {
            job.Invoke();
        }
    }
}
