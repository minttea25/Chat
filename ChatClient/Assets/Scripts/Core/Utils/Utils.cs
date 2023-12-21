using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UObject = UnityEngine.Object;

namespace Core
{
    public class Utils
    {
        public static void AssertCrash(bool condition, string message = null, UObject context = null)
        {
            if (condition == false)
            {
                Debug.Assert(condition, message, context);
                throw new Exception();
            }
        }

        public static bool Assert(bool condition, string message = null, UObject context = null)
        {
            if (condition == false)
            {
                Debug.Assert(condition, message, context);
            }

            return condition;
        }

        public static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            if (go.TryGetComponent(out T component) == false)
            {
                component = go.AddComponent<T>();
            }

            return component;
        }

        public static void WriteJson(string path, object jsonObject, bool prettyPrint = true)
        {
            var text = JsonUtility.ToJson(jsonObject, prettyPrint);
            File.WriteAllText(path, text);

            Debug.Log($"Json file is written at {path}");
        }

        public static GameObject FindChild(GameObject parent, string name = null, bool recursive = false)
        {
            Transform transform = FindChild<Transform>(parent, name, recursive);

            if (transform != null)
            {
                return transform.gameObject;
            }
            else
            {
                return null;
            }
        }


        public static T FindChild<T>(GameObject parent, string name = null, bool recursive = false) where T : UObject
        {
            if (parent == null) return null;

            if (recursive == true)
            {
                foreach (T component in parent.GetComponentsInChildren<T>())
                {
                    if (string.IsNullOrEmpty(name) || component.name == name)
                    {
                        return component;
                    }
                }
            }
            else
            {

                for (int i = 0; i < parent.transform.childCount; i++)
                {
                    Transform transform = parent.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || transform.name == name)
                    {
                        if (transform.TryGetComponent(out T component))
                        {
                            return component;
                        }
                    }
                }
            }

            return null;
        }

        public static T FindChildForce<T>(GameObject parent, string name, bool recursive = false) where T : Component
        {
            if (parent == null) return null;

            if (recursive == true)
            {
                foreach (T component in parent.GetComponentsInChildren<T>())
                {
                    if (component.name == name)
                    {
                        return component;
                    }
                }
            }
            else
            {

                for (int i = 0; i < parent.transform.childCount; i++)
                {
                    Transform transform = parent.transform.GetChild(i);
                    if (transform.name == name)
                    {
                        if (transform.TryGetComponent(out T component))
                        {
                            return component;
                        }
                    }
                }
            }

            GameObject o = GameObject.Find(name);
            if (o != null)
            {
                return o.GetOrAddComponent<T>();
            }

            Debug.LogWarning($"Utils.FindChildForce<T>() uses GameObject.Find(). Frequent calls can affect performance.\n" +
                $"Add the component on the object directly and use Utils.FindChild<T>().");

            return null;
        }

        public static void BindEventOnUI(GameObject go, Action action, UIEvent type = UIEvent.Click)
        {
            var evt = GetOrAddComponent<UIEventHandler>(go);

            switch(type)
            {
                case UIEvent.Click:
                    evt.OnClickHandler -= action;
                    evt.OnClickHandler += action;
                    break;
                case UIEvent.LongClick:
                    evt.OnLongClickHandler -= action;
                    evt.OnLongClickHandler += action;
                    break;
            }
        }

        public static void BindUnityActionOnUI(GameObject go, UnityAction action, UIEvent type = UIEvent.Click)
        {
            var evt = GetOrAddComponent<UIUnityActionHandler>(go);

            switch (type)
            {
                case UIEvent.Click:
                    evt.OnClickHandler -= action;
                    evt.OnClickHandler += action;
                    break;
                case UIEvent.LongClick:
                    evt.OnLongClickHandler -= action;
                    evt.OnLongClickHandler += action;
                    break;
            }
        }

        public static void BindEventOnUIPointer(GameObject go, Action<PointerEventData> action, UIEvent type = UIEvent.Click)
        {
            var evt = GetOrAddComponent<UIEventHandlerPointer>(go);

            switch (type)
            {
                case UIEvent.Click:
                    evt.OnClickHandler -= action;
                    evt.OnClickHandler += action;
                    break;
                case UIEvent.LongClick:
                    evt.OnClickHandler -= action;
                    evt.OnClickHandler += action;
                    break;
            }
        }
    }
}
