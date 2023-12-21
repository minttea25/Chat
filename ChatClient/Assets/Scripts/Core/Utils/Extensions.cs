using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UObject = UnityEngine.Object;

namespace Core
{
    public static class Extensions
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            return Utils.GetOrAddComponent<T>(go);
        }

        public static T GetOrAddComponent<T>(this BaseUI ui) where T : Component
        {
            return Utils.GetOrAddComponent<T>(ui.gameObject);
        }

        public static GameObject FindChild(this GameObject parent, string name = null, bool recursive = false)
        {
            return Utils.FindChild(parent, name, recursive);
        }

        public static T FindChild<T>(this GameObject parent, string name = null, bool recursive = false) where T : UObject
        {
            return Utils.FindChild<T>(parent, name, recursive);
        }

        public static T FindChildForce<T>(this GameObject parent, string name, bool recursive = false) where T : Component
        {
            return Utils.FindChildForce<T>(parent, name, recursive);
        }

        public static void BindEventOnUI(this GameObject go, Action action, UIEvent type = UIEvent.Click)
        {
            Utils.BindEventOnUI(go, action, type); 
        }

        public static void BindUnityActionOnUI(this GameObject go, UnityAction action, UIEvent type = UIEvent.Click)
        {
            Utils.BindUnityActionOnUI(go, action, type);
        }

        public static void BindEventOnUIPointer(this GameObject go, Action<PointerEventData> action, UIEvent type = UIEvent.Click)
        {
            Utils.BindEventOnUIPointer(go, action, type);
        }

        public static void DestroyAllItems(this Transform parent)
        {
            ManagerCore.Resource.DestroyAllItems(parent);
        }
    }
}
