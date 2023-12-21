using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public abstract partial class BaseUIPopup : BaseUI
    {
#if UNITY_EDITOR
        public bool FindContentObject()
        {
            if (ContentObject == null)
            {
                FindContentObject(transform, 0);
            }

            return ContentObject != null;
        }


        void FindContentObject(Transform parent, int depth)
        {
            if (depth == Const.ContentObjectSearchDepthLevel) return;

            foreach (Transform tf in parent)
            {
                if (tf.name == Const.ContentObjectName)
                {
                    ContentObject = tf.gameObject;
                    return;
                }

                if (tf.CompareTag(Const.ContentObjectTag) == true)
                {
                    ContentObject = tf.gameObject;
                    return;
                }

                // recursive call
                FindContentObject(tf, depth + 1);
            }
        }
#endif
    }
}
