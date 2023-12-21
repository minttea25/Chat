using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// A list or array that contains observers is needed.
    /// </summary>
    public interface ISubject<T>
    {
        public void AddObserver(IObserver<T> observer);

        public void RemoveObserver(IObserver<T> observer);

        public void NotifyObservers();

    }
}
