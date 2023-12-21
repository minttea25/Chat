using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    public class UIManagerCore : IManager
    {
        /// <summary>
        /// NOTE: The order of normal popup start at 10; You can open the normal popups up to 19.
        /// </summary>
        const int TopPopupSortingLayer = 30;

        public BaseUIPopup TopPopupUI
        {
            private set
            {
                _onTopPopup = value;
            }
            get
            {
                if (_onTopPopup != null) return _onTopPopup;
                else if (_popupStack.Count != 0) return _popupStack.Peek();
                else return null;
            }
        }

        // for main UI
        BaseUIScene _sceneUI { get; set; } = null;
        public T SceneUI<T>() where T : BaseUIScene => _sceneUI as T;

        // for popup UI
        int _order = 10;
        readonly Stack<BaseUIPopup> _popupStack = new();
        BaseUIPopup _onTopPopup = null;

        public GameObject RootObject
        {
            get
            {
                GameObject rootObject = GameObject.Find($"{Const.RootUIName}");
                if (rootObject == null)
                {
                    rootObject = new GameObject { name = $"{Const.RootUIName}" };
                }
                return rootObject;
            }
        }

        /// <summary>
        /// It should be called BaseUI Object containing Canvas
        /// </summary>
        /// <param name="canvasUI">Root object with Canvas component</param>
        /// <param name="sort">Does it need to be sorting-layer-displayed? (default = false)</param>
        public void SetCanvas(GameObject canvasUI, bool sort = false)
        {
            Canvas canvas = canvasUI.GetOrAddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;

            if (sort == true)
            {
                canvas.sortingOrder = _order;
                _order++;
            }
            else
            {
                canvas.sortingOrder = 0;
            }
        }

        public void SetSceneUI<T>(T sceneUI) where T : BaseUIScene
        {
            _sceneUI = sceneUI;
        }

        public void ShowPopupUIAsync<T>(string key, bool alwaysShowOnTop = false, Action<T> callback = null) where T : BaseUIPopup
        {
            ManagerCore.Resource.Instantiate(
                key,
                null,
                (result) =>
                {
                    T popupUI = result.GetOrAddComponent<T>();
                    if (alwaysShowOnTop == true)
                    {
                        _onTopPopup = popupUI;
                        popupUI.gameObject.GetOrAddComponent<Canvas>().sortingOrder = TopPopupSortingLayer;
                    }
                    else
                    {
                        _popupStack.Push(popupUI);
                    }

                    popupUI.transform.SetParent(RootObject.transform);
                    popupUI.Show();
                    callback?.Invoke(popupUI);
                }
                );
        }

        public void ShowSceneUIAsync<T>(string key, Action<T> callback = null) where T : BaseUIScene
        {
            ManagerCore.Resource.Instantiate(
                key,
                RootObject.transform,
                (result) =>
                {
                    T sceneUI = result.GetOrAddComponent<T>();
                    _sceneUI = sceneUI;
                    callback?.Invoke(sceneUI);
                }
                );
        }

        public void ShowUIAsync<T>(string key, int sortingOrder, Action<T> callback = null) where T : BaseUI
        {
            ManagerCore.Resource.Instantiate(
                key,
                RootObject.transform,
                (result) =>
                {
                    T ui = result.GetOrAddComponent<T>();
                    ui.GetComponent<Canvas>().sortingOrder = sortingOrder;
                    callback?.Invoke(ui);
                }
                );
        }

        public void AddItemUIAsync<T>(string key, Transform parent, Action<T> callback = null) where T : BaseUIItem
        {
            ManagerCore.Resource.Instantiate(
                key,
                parent,
                (result) =>
                {
                    T itemUI = result.GetOrAddComponent<T>();
                    callback?.Invoke(itemUI);
                }
                );
        }

        public void CloseTopPopupUI()
        {
            if (_onTopPopup != null)
            {
                ManagerCore.Resource.Destroy(TopPopupUI.gameObject);
                TopPopupUI = null;
                return;
            }
            else
            {
                ClosePopupUI();
            }
        }

        public void ClosePopupUI(BaseUIPopup popup)
        {

            if (_popupStack.Count == 0) return;

            // assertion crash
            if (_popupStack.Peek() != popup)
            {
                Debug.LogError($"Close popup UI failed", _popupStack.Peek());
                return;
            }

            ClosePopupUI();
        }

        public void ClosePopupUI()
        {
            if (_popupStack.Count == 0) return;

            BaseUIPopup popup = _popupStack.Pop();
            ManagerCore.Resource.Destroy(popup.gameObject);
            popup = null;

            _order--;
        }

        public void CloseAllPopupUI()
        {
            while(_popupStack.Count > 0)
            {
                ClosePopupUI();
            }
        }

        void IManager.InitManager()
        {
        }

        void IManager.ClearManager()
        {
            ClosePopupUI();

            if (_sceneUI != null)
            {
                ManagerCore.Resource.Destroy(_sceneUI.gameObject);
            }
            
            _sceneUI = null;
        }
    }
}
