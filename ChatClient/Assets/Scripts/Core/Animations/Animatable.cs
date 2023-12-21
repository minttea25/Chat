using System.Collections;
using UnityEngine;

namespace Core.Animation
{
    public abstract class Animatable : MonoBehaviour
    {
        public float Time { get; protected set; }
        public bool IsFinished { get; protected set; } = false;
        protected Coroutine animationCo;

        public void PlayAnimation()
        {
            animationCo = StartCoroutine(Play());
        }

        public void StopAnimation()
        {
            if (animationCo != null)
            {
                StopCoroutine(animationCo);
                IsFinished = true;
            }
        }

        internal abstract IEnumerator Play();
    }
}
