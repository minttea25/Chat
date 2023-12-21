using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Core;

namespace Core
{
    public class UIEventHandlerDynamic : UIEventHandlerPointer
    {
        public Vector3 NormalScale { get; private set; } = new(1f, 1f, 1f);
        public Vector3 ClickedScale { get; private set; } = new(0.9f, 0.9f, 0.9f);

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            gameObject.transform.localScale = ClickedScale;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            gameObject.transform.localScale = NormalScale;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            gameObject.transform.localScale = NormalScale;
        }

        public void SetDynamicScale(float normal, float clicked)
        {
            NormalScale = new(normal, normal, normal);
            ClickedScale = new(clicked, clicked, clicked);
        }
    }
}
