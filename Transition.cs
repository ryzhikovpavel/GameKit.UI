using System;
using System.Collections;
using GameKit.UI.Implementation;
using UnityEngine;

namespace GameKit.UI
{
    public enum Transition
    {
        Sequence,
        Simultaneously
    }

    public class TransitionHandler<TView> : IEnumerator, IDisposable where TView : ViewComponent
    {
        public TView View { get; private set; }

        public bool IsTransition => View is null || View.IsTransition;

        protected virtual bool KeepWaiting => IsTransition;

        internal void SetView(TView view)
        {
            View = view;
        }

        public void Dispose()
        {
            View = null;
        }

        bool IEnumerator.MoveNext() => KeepWaiting;
        void IEnumerator.Reset() { }
        object IEnumerator.Current => null;
    }
}