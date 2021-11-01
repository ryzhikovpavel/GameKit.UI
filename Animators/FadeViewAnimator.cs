#pragma warning disable 649
using UnityEngine;
using System;
using System.Collections;
using GameKit.UI.Core;

namespace GameKit.UI.Animators
{
    [RequireComponent(typeof(CanvasGroup))]
    internal class FadeViewAnimator : MonoBehaviour, IViewAnimator
    {
        [Flags]
        private enum Direction
        {
            In = 1,
            Out = 2
        }
        
        [Range(0.1f, 5f), SerializeField] private float duration = 0.33f;
        [SerializeField] private Direction direction = Direction.In | Direction.Out;
        private CanvasGroup transparent;
        private Coroutine routine;

        public bool IsPlaying => routine != null;

        public void PlayShow(Action onReady, Action onComplete)
        {
            if (routine != null)
            {
                Debug.LogWarning($"{gameObject.name} replace animation on Show. Completion events from past animation will not be dispatched");
                StopCoroutine(routine);
            }
            if (direction.HasFlag(Direction.In))
                routine = StartCoroutine(Fade(1, onReady, onComplete));
            else
            {
                transparent.alpha = 1;
                onReady?.Invoke();
                onComplete?.Invoke();
            }
        }

        public void PlayHide(Action onReady, Action onComplete)
        {
            if (routine != null)
            {
                Debug.LogWarning($"{gameObject.name} replace animation on Hide. Completion events from past animation will not be dispatched");
                StopCoroutine(routine);
            }

            if (direction.HasFlag(Direction.Out))
                routine = StartCoroutine(Fade(0, onReady, onComplete));
            else
            {
                transparent.alpha = 0;
                onReady?.Invoke();
                onComplete?.Invoke();
            }
        }

        private void Awake()
        {
            transparent = GetComponent<CanvasGroup>();
            transparent.alpha = 0;
        }

        private void OnDisable()
        {
            if (routine != null) StopCoroutine(routine);
        }

        private IEnumerator Fade(int target, Action onReady, Action onComplete)
        {
            yield return null;
            
            int dir = target == 0 ? -1 : 1;

            while (transparent.alpha.Equals(target) == false)
            {
                transparent.alpha = Mathf.Clamp01(transparent.alpha + dir * Time.unscaledDeltaTime / duration);
                yield return null;
            }
            onReady?.Invoke();
            yield return null;
            onComplete?.Invoke();
            routine = null;
        }
    }
}