#pragma warning disable 649
using System;
using System.Collections;
using UnityEngine;

namespace GameKit.UI.Animations
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
        
        [Range(0.1f, 5f), SerializeField] private float duration = 0.15f;
        [SerializeField] private Direction direction = Direction.In | Direction.Out;
        private CanvasGroup _transparent;
        private Coroutine _routine;

        public bool IsPlaying => _routine != null;

        public void PlayShow(Action onReady, Action onComplete)
        {
            if (_routine != null)
            {
                Debug.LogWarning($"{gameObject.name} replace animation on Show. Completion events from past animation will not be dispatched");
                StopCoroutine(_routine);
            }
            if (direction.HasFlag(Direction.In))
                _routine = StartCoroutine(Fade(1, onReady, onComplete));
            else
            {
                _transparent.alpha = 1;
                onReady?.Invoke();
                onComplete?.Invoke();
            }
        }

        public void PlayHide(Action onReady, Action onComplete)
        {
            if (_routine != null)
            {
                Debug.LogWarning($"{gameObject.name} replace animation on Hide. Completion events from past animation will not be dispatched");
                StopCoroutine(_routine);
            }

            if (direction.HasFlag(Direction.Out))
                _routine = StartCoroutine(Fade(0, onReady, onComplete));
            else
            {
                _transparent.alpha = 0;
                onReady?.Invoke();
                onComplete?.Invoke();
            }
        }

        private void Awake()
        {
            _transparent = GetComponent<CanvasGroup>();
            _transparent.alpha = 0;
        }

        private void OnDisable()
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = null;
        }

        private IEnumerator Fade(int target, Action onReady, Action onComplete)
        {
            yield return null;
            
            int dir = target == 0 ? -1 : 1;

            while (_transparent.alpha.Equals(target) == false)
            {
                _transparent.alpha = Mathf.Clamp01(_transparent.alpha + dir * Time.unscaledDeltaTime / duration);
                yield return null;
            }
            onReady?.Invoke();
            yield return null;
            _routine = null;
            onComplete?.Invoke();
        }
    }
}