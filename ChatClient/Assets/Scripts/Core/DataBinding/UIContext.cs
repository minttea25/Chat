using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

namespace Core
{
    public abstract class UIContext
    {
        
    }

    public class UIComponentMapper
    {
        readonly Dictionary<Type, ObjectType> componentTypeMap;
        public UIComponentMapper()
        {
            // Initialize the component type map
            componentTypeMap = new() 
            {
                { typeof(TextMeshProUGUI), ObjectType.TextMeshProUGUI },
                { typeof(Slider), ObjectType.Slider },
                { typeof(Image), ObjectType.Image },
                { typeof(RawImage), ObjectType.RawImage },
                { typeof(Toggle), ObjectType.Toggle},
            };
        }

        public ObjectType GetObjectType(GameObject go)
        {
            ObjectType type = ObjectType.None;

            foreach (var component in go.GetComponents<Component>())
            {
                if (componentTypeMap.TryGetValue(component.GetType(), out var uiType))
                {
                    type = (int)uiType > (int)type ? uiType : type;
                }
            }

            return type;
        }
    }
}

