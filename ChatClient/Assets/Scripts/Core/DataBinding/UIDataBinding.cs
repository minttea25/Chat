using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Core
{
    public class UIDataBinding
    {
        public static Dictionary<ObjectType, Action<GameObject, object>> DataBindings
            = new Dictionary<ObjectType, Action<GameObject, object>>()
        {
            { ObjectType.TextMeshProUGUI, ApplyText },
            { ObjectType.Image, ApplyImage },
            { ObjectType.RawImage, ApplyRawImage },
            { ObjectType.Toggle, ApplyToggle },
            { ObjectType.Slider, ApplySlider },
        };

        public static void ApplyText(GameObject bindObject, object text)
        {
            if (bindObject.TryGetComponent<TextMeshProUGUI>(out var textMesh))
            {
                textMesh.text = text as string;
            }
            else
            {
                Debug.LogWarning($"{bindObject.name} has no component=TextMeshProUGUI. The changes of value was not applied.");
            }
        }

        public static void ApplyImage(GameObject bindObject, object image)
        {
            if (bindObject.TryGetComponent<Image>(out var img))
            {
                img.sprite = image as Sprite;
            }
            else
            {
                Debug.LogWarning($"{bindObject.name} has no component=Image. The changes of value was not applied.");
            }
        }

        public static void ApplyToggle(GameObject bindObject, object listener)
        {
            if (bindObject.TryGetComponent<Toggle>(out var toggle))
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(listener as UnityAction<bool>);
            }
            else
            {
                Debug.LogWarning($"{bindObject.name} has no component=Toggle. The changes of value was not applied.");
            }
        }

        public static void ApplySlider(GameObject bindObject, object value)
        {
            if (bindObject.TryGetComponent<Slider>(out var slider))
            {
                slider.value = (float)value;
            }
            else
            {
                Debug.LogWarning($"{bindObject.name} has no component=Slider. The changes of value was not applied.");
            }
        }

        public static void ApplyRawImage(GameObject bindObject, object rawImage)
        {
            if (bindObject.TryGetComponent<RawImage>(out var img))
            {
                img.texture = rawImage as Texture2D;
            }
            else
            {
                Debug.LogWarning($"{bindObject.name} has no component=RawImage. The changes of value was not applied.");
            }
        }
    }
}
