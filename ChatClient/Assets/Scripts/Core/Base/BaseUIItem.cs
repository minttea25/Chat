using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core
{
    public abstract class BaseUIItem : BaseUI
    {
        /// <summary>
        /// Need to check null
        /// </summary>
        protected UIEventHandlerDynamic dynamicHandler = null;

        public override void Init()
        {
            base.Init();
        }

        public void UseDynamicUIEventHandler()
        {
            dynamicHandler = gameObject.GetOrAddComponent<UIEventHandlerDynamic>();
        }

        public void RemoveDynamicEventHandler()
        {
            Destroy(gameObject.GetComponent<UIEventHandlerDynamic>());
        }
    }
}
