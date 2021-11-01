using System;
using GameKit.UI.Core;

namespace GameKit.UI.Animators
{
    internal class DummyViewAnimator: IViewAnimator
    {
        public bool IsPlaying => false;

        public void PlayShow(Action onReady, Action onComplete)
        {
            onReady?.Invoke();
            onComplete?.Invoke();
        }

        public void PlayHide(Action onReady, Action onComplete)
        {
            onReady?.Invoke();
            onComplete?.Invoke();
        }
    }
}