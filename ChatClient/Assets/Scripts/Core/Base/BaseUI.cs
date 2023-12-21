using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UObject = UnityEngine.Object;

namespace Core
{
    public abstract partial class BaseUI : MonoBehaviour
    {
        readonly Dictionary<Type, UObject[]> _objects = new();
        protected bool _init = false;

        readonly UIComponentMapper UIComponentMapper = new();

        /// <summary>
        /// It is called on Awake of BaseUI(parent). (NOTE: You don't need to call it on Awake or Start of other MonoBehaviour)
        /// It should be have 'base.Init()' included at first line.
        /// </summary>
        public virtual void Init()
        {
            _init = true;
        }

        private void Awake()
        {
            Init();
        }

        #region Bind Functions

        public void BindText(Type type) { Bind<TextMeshProUGUI>(type); }
        public void BindButton(Type type) { Bind<Button>(type); }
        public void BindSlider(Type type) { Bind<Slider>(type); }
        public void BindImage(Type type) { Bind<Image>(type); }
        public void BindRawImage(Type type) { Bind<RawImage>(type); }
        public void BindInputField(Type type) { Bind<TMP_InputField>(type); }
        public void BindDropDown(Type type) { Bind<TMP_Dropdown>(type); }
        public void BindObject(Type type) { Bind<GameObject>(type); }

        public void Bind<T>(Type type) where T : UObject
        {
            string[] names = Enum.GetNames(type);
            UObject[] objects = new UObject[names.Length];

            _objects.Add(typeof(T), objects);

            for (int i = 0; i < objects.Length; i++)
            {
                if (typeof(T) == typeof(GameObject))
                {
                    objects[i] = gameObject.FindChild(names[i], true);
                }
                else
                {
                    objects[i] = gameObject.FindChild<T>(names[i], true);
                }
                if (objects[i] == null)
                {
                    Debug.LogWarning($"Failed to bind {names[i]} [Type: {type.Name}]");
                }
            }
        }

        public void BindForce<T>(Type type) where T : Component
        {
            string[] names = Enum.GetNames(type);
            T[] objects = new T[names.Length];

            _objects.Add(typeof(T), objects);

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i] = gameObject.FindChildForce<T>(names[i], recursive: true);

                if (objects[i] == null)
                {
                    Debug.LogWarning($"Failed to bind {names[i]} [Type: {type.Name}]");
                }
            }
        }

        /// <summary>
        /// NOTE: The default Text type is considered TextMeshProUGUI. (UnityEngine.UI.Text is now legacy.)
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public TextMeshProUGUI GetText(int idx) { return Get<TextMeshProUGUI>(idx); }
        public Button GetButton(int idx) { return Get<Button>(idx); }
        public Slider GetSlider(int idx) { return Get<Slider>(idx); }
        public Image GetImage(int idx) { return Get<Image>(idx); }
        public RawImage GetRawImage(int idx) { return Get<RawImage>(idx); }

        /// <summary>
        /// NOTE: The default InputField type is considered TMP_InputField. (UnityEngine.UI.InputField is now legacy.)
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public TMP_InputField GetInputField(int idx) { return Get<TMP_InputField>(idx); }

        /// <summary>
        /// NOTE: The default DropDOwn type is considered TMP_DropDown. (UnityEngine.UI.DropDown is now legacy.)
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public TMP_Dropdown GetDropDown(int idx) { return Get<TMP_Dropdown>(idx); }

        /// <summary>
        /// NOTE: The Panel type is considered GameObject.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public GameObject GetObject(int idx) { return Get<GameObject>(idx); }

        public T Get<T>(int idx) where T : UObject
        {
            if (_objects.TryGetValue(typeof(T), out UObject[] objects))
            {
                return objects[idx] as T;
            }
            else
            {
                Debug.LogError($"There is no object [index={idx}] with type={typeof(T).Name}");
                return null;
            }
        }

        #endregion

        #region BindEvnet Functions
        public void BindEvent(Action action, UIEvent type = UIEvent.Click)
        {
            var evt = gameObject.GetOrAddComponent<UIEventHandler>();

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

        public void BindEventUnityAction(UnityAction action, UIEvent type = UIEvent.Click)
        {
            var evt = gameObject.GetOrAddComponent<UIUnityActionHandler>();

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

        public void BindEventPointer(Action<PointerEventData> action, UIEvent type = UIEvent.Click)
        {
            var evt = gameObject.GetOrAddComponent<UIEventHandlerPointer>();

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

        #endregion
    }

}
