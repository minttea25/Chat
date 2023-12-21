using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Core
{
    public class DOTweenAnimations
    {
        public static float AnimationDuration = 0.5f;

        /// <summary>
        /// Check the [active=false] before calling it.
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="easeType"></param>
        /// <param name="callback"></param>
        public static void ShowUI(GameObject ui, Ease easeType = Ease.OutBack, Action callback = null)
        {
            ui.transform.localScale = Vector3.zero;

            ui.SetActive(true);

            ui.transform.DOScale(Vector3.one, AnimationDuration)
                .SetEase(easeType)
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }

        /// <summary>
        /// Check the [active=true] before calling it.
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="easeType"></param>
        /// <param name="callback"></param>
        public static void HideUI(GameObject ui, Ease easeType = Ease.InBack, Action callback = null)
        {
            ui.transform.DOScale(Vector3.zero, AnimationDuration)
                .SetEase(easeType)
                .OnComplete(() =>
                {
                    ui.SetActive(false);
                    callback?.Invoke();
                });
        }
    }
}

