using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    public class UIEventHandlerPointer : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public const float th_long_click = 0.5f;

        public Action<PointerEventData> OnClickHandler = null;
        public Action<PointerEventData> OnLongClickHandler = null;

        private bool isPointerDown = false;
        private float pointerDownTimer = 0f;

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (pointerDownTimer > th_long_click)
            {
                OnLongClickHandler?.Invoke(eventData);
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            pointerDownTimer = 0f;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            if (pointerDownTimer <= th_long_click)
            {
                OnClickHandler?.Invoke(eventData);
            }
        }

        private void Update()
        {
            if (isPointerDown)
            {
                pointerDownTimer += Time.deltaTime;
            }
        }
    }
}
