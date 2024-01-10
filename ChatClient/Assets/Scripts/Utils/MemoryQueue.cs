using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chat.Utils
{
    struct MemoryObject<T> where T : class
    {
        public int priority;
        public T target;
    }

    public class MemoryQueue<Key, T> where T : class
    {
        public int Capacity => m_capacity;

        private readonly int m_capacity;
        private readonly Dictionary<Key, MemoryObject<T>> map;
        private int p = 1;

        public MemoryQueue(int capacity)
        {
            m_capacity = capacity;
            map = new Dictionary<Key, MemoryObject<T>>();
        }

        public void Add(Key key, T value, out T removed)
        {
            if (map.ContainsKey(key))
            {
                removed = null;
                return; 
            }

            if (map.Count >= m_capacity)
            {
                Remove(out T r);
                removed = r;
            }
            else
            {
                removed = null;
            }

            MemoryObject<T> memoryObject = new()
            {
                priority = p,
                target = value,
            };
            p++;
            map.Add(key, memoryObject);
        }


        bool Remove(out T removed)
        {
            if (map.Count == 0)
            {
                removed = null;
                return false;
            }

            int lowest = int.MaxValue;
            Key key = map.Keys.GetEnumerator().Current;
            removed = null;
            foreach (var kv  in map)
            {
                if (lowest > kv.Value.priority)
                {
                    lowest = kv.Value.priority;
                    removed = kv.Value.target;
                    key = kv.Key;
                }
            }

            map.Remove(key);
            return true;
        }

        public bool Contains(Key key) => map.ContainsKey(key);

        public bool TryGetValue(Key key, out T value)
        {
            if (map.Count == 0 || map.ContainsKey(key) == false)
            {
                value = null;
                return false;
            }

            value = map[key].target;
            return true;
        }
    }
}
