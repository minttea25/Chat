using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Core.Animation
{
    public class AnimationHandler : MonoBehaviour
    {
        public bool IsRunning { private set; get; } = false;

        private readonly Queue<Animatable> animationQueue = new();
        private readonly object locker = new();

        public void AddAnimation(Animatable animatable)
        {
            lock (locker)
            {
                animationQueue.Enqueue(animatable);
            }
        }

        public void ClearAnimations()
        {
            lock (locker)
            {
                animationQueue.Clear();
            }
        }

        public void StartAnimations(float interval = 0.0f)
        {
            StartCoroutine(StartAnimationCo(interval));
        }

        public void StartAnimations(Action callback, float interval = 0.0f)
        {
            StartCoroutine(StartAnimationCo(interval, callback));
        }

        public void StartAnimationsInSequence(Action callback = null)
        {
            StartCoroutine(StartAnimationInSequenceCo(callback));
        }

        private IEnumerator StartAnimationCo(float interval)
        {
            lock (locker)
            {
                IsRunning = true;
                while (animationQueue.Count != 0)
                {
                    var animation = animationQueue.Dequeue();
                    animation.PlayAnimation();

                    yield return new WaitForSeconds(interval);
                }
                IsRunning = false;
            }
        }

        private IEnumerator StartAnimationCo(float interval, Action callback)
        {
            List<Animatable> aList = new();

            lock (locker)
            {
                IsRunning = true;

                while (animationQueue.Count != 0)
                {
                    var animation = animationQueue.Dequeue();
                    animation.PlayAnimation();
                    aList.Add(animation);

                    yield return new WaitForSeconds(interval);
                }

                IsRunning = false;
            }

            foreach (var animation in aList)
            {
                while (animation.IsFinished == false) yield return null;
            }

            callback.Invoke();
        }

        private IEnumerator StartAnimationInSequenceCo(Action callback = null)
        {
            lock (locker)
            {
                IsRunning = true;

                while (animationQueue.Count != 0)
                {
                    var animation = animationQueue.Dequeue();
                    yield return StartCoroutine(animation.Play());
                }

                IsRunning = false;
            }

            callback?.Invoke();
        }
    }
}


