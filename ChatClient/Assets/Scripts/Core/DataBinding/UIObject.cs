using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Core
{
    public interface IDBCustomComponent<T>
    {
        public void UpdateUI(T data);
    }

    [Serializable]
    public class UIObject<Com> where Com : Component
    {
        /// <summary>
        /// DO NOT CHANGE THE NAME; It is relevant  from the System.Reflection. See: Const.BindObjectFieldName.
        /// </summary>
        public GameObject BindObject = null;
        /// <summary>
        /// DO NOT CHANGE THE NAME; It is relevant  from the System.Reflection. See: Const.BindObjectFieldName.
        /// </summary>
        public ObjectType BindObjectType;
        public Com Component => BindObject == null ? null : BindObject.GetComponent<Com>();

        public void Update(object data)
        {
            UIDataBinding.DataBindings[BindObjectType]?.Invoke(BindObject, data);
        }
    }

    [Serializable]
    public class UIObject<Com, Data> where Com : Component, IDBCustomComponent<Data>
    {
        /// <summary>
        /// DO NOT CHANGE THE NAME; It is relevant  from the System.Reflection. See: Const.BindObjectFieldName.
        /// </summary>
        public GameObject BindObject = null;
        /// <summary>
        /// DO NOT CHANGE THE NAME; It is relevant  from the System.Reflection. See: Const.BindObjectFieldName.
        /// </summary>
        public ObjectType BindObjectType = ObjectType.CUSTOM;
        public Com Component => BindObject == null ? null : BindObject.GetComponent<Com>();
    }
}
