using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public abstract class BaseReusableUIPopup : BaseUIPopup
    {
        /// <summary>
        /// Set gameObject active(true) and show the ContentObject
        /// </summary>
        public sealed override void Show()
        {
            if (VisibleState == VisibleStates.Appearing || VisibleState == VisibleStates.Appeared) return;

            gameObject.SetActive(true);

            VisibleState = VisibleStates.Appearing;
            DOTweenAnimations.ShowUI(ContentObject, callback: () =>
            {
                VisibleState = VisibleStates.Appeared;
            });
        }

        public sealed override void Hide()
        {
            if (VisibleState == VisibleStates.Disappearing || VisibleState == VisibleStates.Disappeared) return;

            VisibleState = VisibleStates.Disappearing;
            DOTweenAnimations.HideUI(ContentObject, callback: () =>
            {
                VisibleState = VisibleStates.Disappeared;
                gameObject.SetActive(false);
            });
        }
    }
}
