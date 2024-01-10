using Core;
using ServerCoreTCP.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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



    // update���� ���� => �ִ� ������ �� 20�� ó����
    public List<Action> PopAll()
    {
        List<Action> result = new List<Action>();
        for(int i = 0; i<AppConst.ProcessUnityJobQueuePerFrame; ++i)
        {
            if (queue.TryDequeue(out var action))
            {
                result.Add(action);
            }
            if (queue.IsEmpty == true) break;
        }

        return result;
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
