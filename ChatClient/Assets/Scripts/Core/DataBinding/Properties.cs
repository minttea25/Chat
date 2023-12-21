using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

namespace Core
{
    public enum ObjectType
    {
        // 우선 순위 주의
        // custom은 직접 설정해야됨
        None = 0,
        CUSTOM = 1, // 사용자 데이터
        TextMeshProUGUI, // 텍스트
        Slider, // 값
        Image, // sprite
        RawImage, // raw image
        Toggle, // on/off
    }

    public interface IProperty<DataType>
    {
        public DataType Data { get; set; }
        public void Update();
    }

    public class CustomProperty<DataType, Com> : IProperty<DataType> where Com : Component, IDBCustomComponent<DataType>
    {
        List<UIObject<Com, DataType>> _ui;
        DataType _data;

        public CustomProperty(DataType data = default)
        {
            _data = data;
            _ui = new List<UIObject<Com, DataType>>();
        }

        public DataType Data
        {
            get { return _data; }
            set { _data = value; Update(); }
        }

        public void AddUI(params UIObject<Com, DataType>[] ui)
        {
            foreach (var uiObj in ui) _ui.Add(uiObj);
        }

        public void Update()
        {
            foreach (var uIObject in _ui)
            {
                uIObject.Component.UpdateUI(Data);
            }
        }
    }

    public class Property<DataType, Com> : IProperty<DataType> where Com : Component
    {
        List<UIObject<Com>> _ui;
        DataType _data;

        public Property(DataType data = default)
        {
            _data = data;
            _ui = new List<UIObject<Com>>();
        }

        public DataType Data
        {
            get { return _data; }
            set { _data = value; Update(); }
        }

        public void AddUI(params UIObject<Com>[] ui)
        {
            foreach (UIObject<Com> uiObj in ui) _ui.Add(uiObj);
        }

        public void Update()
        {
            foreach (UIObject<Com> uIObject in _ui)
            {
                uIObject.Update(Data);
            }
        }
    }


    public class DataWrapper<DataType>
    {
        public DataWrapper(DataType value) { this.value = value; }

        public DataType value;
    }
}

