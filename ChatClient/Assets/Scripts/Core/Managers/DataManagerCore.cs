using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;


namespace Core
{
    public interface IJsonLoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDictionaryData();
    }

    public class DataManagerCore : IManager
    {
        // add members to load
        
        public bool Loaded()
        {


            return true;
        }

        public void Init()
        {
            
        }

        void LoadJsonAsync<T>(string key, Action<T> callback)
        {
            ManagerCore.Resource.LoadAsyncOnce<TextAsset>(key, (textAsset) =>
            {
                T json = JsonUtility.FromJson<T>(textAsset.text);
                callback?.Invoke(json);
            });
        }

        void LoadJsonLoaderAsync<Loader, Key, Value>(string key, Action<Loader> callback) where Loader : IJsonLoader<Key, Value>
        {
            ManagerCore.Resource.LoadAsyncOnce<TextAsset>(key, (textAsset) =>
            {
                Loader loader = JsonUtility.FromJson<Loader>(textAsset.text);
                callback?.Invoke(loader);
            });
        }

        void LoadXmlAsync<T>(string key, Action<T> callback)
        {
            ManagerCore.Resource.LoadAsyncOnce<TextAsset>(key, (textAsset) =>
            {
                XmlSerializer xs = new(typeof(T));
                MemoryStream stream = new(System.Text.Encoding.UTF8.GetBytes(textAsset.text));
                callback?.Invoke((T)xs.Deserialize(stream));
            });
        }

        void LoadXmlAsync<Loader, Key, Value>(string key, Action<Loader> callback) where Loader : IJsonLoader<Key, Value>, new()
        {
            ManagerCore.Resource.LoadAsyncOnce<TextAsset>(key, (textAsset) =>
            {
                XmlSerializer xs = new(typeof(Loader));
                MemoryStream stream = new(System.Text.Encoding.UTF8.GetBytes(textAsset.text));
                callback?.Invoke((Loader)xs.Deserialize(stream));
            });
        }

        void IManager.ClearManager()
        {
        }

        void IManager.InitManager()
        {
            Init();
        }
    }
}
