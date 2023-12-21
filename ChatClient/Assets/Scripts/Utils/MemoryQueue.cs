using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chat.Utils
{
    public class MemoryQueue<Key, T>
    {
        public int Capacity => _capacity;

        private readonly int _capacity;
        private readonly Queue<Key> queue;
        private readonly Dictionary<Key, T> map;

        public MemoryQueue(int capacity)
        {
            _capacity = capacity;
            queue = new(capacity);
            map = new Dictionary<Key, T>(capacity);
        }

        public bool AddAndGetRemoved(Key key, T value, out T removed)
        {
            if (map.ContainsKey(key))
            {
                removed = default;

                return false;
            }

            if (queue.Count >= _capacity)
            {
                // �뷮�� �ʰ��ϴ� ��� ���� ������ ������Ʈ�� ����
                Key oldestKey = queue.Dequeue();
                removed = map[oldestKey];
                map.Remove(oldestKey);
                map.Add(key, value);
                queue.Enqueue(key);

                return true;
            }
            else
            {
                map.Add(key, value);
                queue.Enqueue(key);
                removed = default;

                return false;
            }
        }

        public void Add(Key key, T value)
        {
            if (map.ContainsKey(key))
            {
                return;
            }

            if (queue.Count >= _capacity)
            {
                // �뷮�� �ʰ��ϴ� ��� ���� ������ ������Ʈ�� ����
                Key oldestKey = queue.Dequeue();
                map.Remove(oldestKey);
            }

            // �� ������Ʈ �߰�
            map.Add(key, value);
            queue.Enqueue(key);
        }

        public bool Remove(Key key, out T removed)
        {
            if (map.TryGetValue(key, out T value))
            {
                map.Remove(key);

                // queue���� �ش� �������� ����
                int count = queue.Count;
                for (int i = 0; i < count; i++)
                {
                    Key item = queue.Dequeue();
                    if (!EqualityComparer<Key>.Default.Equals(item, key))
                    {
                        queue.Enqueue(item);
                    }
                }

                removed = value;
                return true;
            }
            else
            {
                removed = default;
                return false;
            }
        }

        public bool Contains(Key key) => map.ContainsKey(key);

        public bool TryGetValue(Key key, out T value) => map.TryGetValue(key, out value);

        public T Get(Key key)
        {
            if (map.TryGetValue(key, out T obj))
            {
                return obj;
            }
            return default; // �Ǵ� null (���� Ÿ���� ��)
        }
    }
}
