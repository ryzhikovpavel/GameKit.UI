using System;
using UnityEngine;

namespace GameKit.UI.Core
{
    public interface IViewAnimator
    {
        bool IsPlaying { get; }
        void PlayShow(Action onReady, Action onComplete);
        void PlayHide(Action onReady, Action onComplete);
    }
}