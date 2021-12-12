using System;

namespace GameKit.UI.Animations
{
    public interface IViewAnimator
    {
        bool IsPlaying { get; }
        void PlayShow(Action onReady, Action onComplete);
        void PlayHide(Action onReady, Action onComplete);
    }
}