using UnityEngine;
using UnityEditor;

namespace Core
{
    public abstract partial class BaseUIPopup : BaseUI
    {
        [Tooltip("A panel to apply show/hide animation; It must not be a canvas.\nIf not assigned in Editor, it would find with a tag or name.")]
        public GameObject ContentObject; // Do not change the name.

        VisibleStates m_visibleState = VisibleStates.Disappeared;
        public VisibleStates VisibleState
        {
            get => m_visibleState;
            set => m_visibleState = value;
        }

        public enum VisibleStates
        {
            Appearing,
            Appeared,
            Disappearing,
            Disappeared,
        }

        public override void Init()
        {
            base.Init();

            ManagerCore.UI.SetCanvas(gameObject, true);

            BindContent();
            ContentObject.SetActive(false);
        }

        public void Close()
        {
            VisibleState = VisibleStates.Disappearing;
            DOTweenAnimations.HideUI(ContentObject, callback: () =>
            {
                VisibleState = VisibleStates.Disappeared;
                ManagerCore.UI.ClosePopupUI(this);
            });
        }

        public virtual void Show()
        {
            if (VisibleState == VisibleStates.Appearing || VisibleState == VisibleStates.Appeared) return;

            VisibleState = VisibleStates.Appearing;
            DOTweenAnimations.ShowUI(ContentObject, callback: () =>
            {
                VisibleState = VisibleStates.Appeared;
            });
        }

        public virtual void Hide()
        {
            if (VisibleState == VisibleStates.Disappearing || VisibleState == VisibleStates.Disappeared) return;

            VisibleState = VisibleStates.Disappearing;
            DOTweenAnimations.HideUI(ContentObject, callback: () => 
            {
                VisibleState = VisibleStates.Disappeared;
            });
        }

        internal void BindContent()
        {
            if (ContentObject == null)
            {
                ContentObject = GameObject.FindGameObjectWithTag(Const.ContentObjectTag);

                if (ContentObject == null)
                {
                    ContentObject = GameObject.Find(Const.ContentObjectName);
                }
            }
            Utils.Assert(ContentObject != null, "Can not find ContentObject");
        }
    }
}
