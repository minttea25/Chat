using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Core
{
    public interface IObserver<T>
    {
        public void Notify(T subject);
    }
}

