using System;
using System.Collections;
using GameKit.UI.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace GameKit.UI.Implementation
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class ViewComponent : MonoBehaviour
    {
        [SerializeField] private bool autoHideOnSuspend;
        
        public event Action<ViewComponent> EventDestroy = delegate {};
        
        public bool IsDisplayed => gameObject.activeSelf && _canvas.enabled;
        public bool IsTransition => Animator.IsPlaying;
        public ViewState State { get; private set; }
        
        public IViewAnimator Animator { get; private set; }
        public virtual bool Interactable { get; set; }
        

        internal int Order
        {
            get => _canvas.sortingOrder;
            set => _canvas.sortingOrder = value;
        }
        
        private Canvas _canvas;
        private GraphicRaycaster _raycaster;

        internal void ShowInternal() => ShowInternal(null);
        internal void ShowInternal(Action onComplete)
        {
            gameObject.SetActive(true);
            Interactable = false;
            State = ViewState.Showing;
            OnActivated();
            Animator.PlayShow(onComplete, ShowComplete);
        }

        internal void HideInternal() => HideInternal(null);
        internal void HideInternal(Action onComplete)
        {
            if (IsDisplayed == false)
            {
                Debug.LogWarning($"{gameObject.name} is disabled");
                onComplete?.Invoke();
                return;
            }
         
            State = ViewState.Hiding;
            Interactable = false;
            OnHide();
            Animator.PlayHide(onComplete, HideComplete);
        }

        internal void Suspend()
        {
            if (autoHideOnSuspend) _canvas.enabled = false;
            OnSuspend();
            State = ViewState.Suspended;
        }

        internal void Resume()
        {
            if (autoHideOnSuspend) _canvas.enabled = true;
            OnResume();
            State = ViewState.Displayed;
        }

        protected virtual void OnActivated() { }
        protected virtual void OnDeactivated() { }
        
        protected virtual void OnDisplayed() { }
        protected virtual void OnHide() { }
        
        protected virtual void OnSuspend() { }
        protected virtual void OnResume() { }


        private void OnDestroy()
        {
            EventDestroy?.Invoke(this);
        }

        private void ShowComplete()
        {
            Interactable = true;
            State = ViewState.Displayed;
            OnDisplayed();
        }

        protected internal virtual void HideComplete()
        {
            Interactable = true;
            State = ViewState.Disabled;
            OnDeactivated();
            gameObject.SetActive(false);
        }
        
        public IEnumerator AwaitHidingBegin()
        {
            while (IsDisplayed && Interactable) yield return null;
        }

        public IEnumerator AwaitInteractableReady()
        {
            while (IsDisplayed && Interactable == false) yield return null;
        }

        public IEnumerator AwaitHidingEnd()
        {
            while (IsDisplayed) yield return null;
        }

        public IEnumerator AwaitTransitionEnd()
        {
            while (Animator.IsPlaying) yield return null;
        }
        
        internal virtual void Initialize()
        {
            _canvas = GetComponent<Canvas>();
            _raycaster = GetComponent<GraphicRaycaster>();
            Animator = GetComponent<IViewAnimator>() ?? new DummyViewAnimator();
        }
        
        protected virtual void Reset()
        {
            var rt = (RectTransform)transform;
            rt.anchorMax = new Vector2(1, 1);
            rt.anchorMin = new Vector2(0, 0);
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(0, 0);
            gameObject.name = GetType().Name;

            _canvas = GetComponent<Canvas>();

            if (_canvas.isRootCanvas)
            {
                var scaler = gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

                switch (Screen.orientation)
                {
                    case ScreenOrientation.Portrait:
                    case ScreenOrientation.PortraitUpsideDown:
                        scaler.referenceResolution = new Vector2(1024, 2048);
                        scaler.matchWidthOrHeight = 1;
                        break;
                    case ScreenOrientation.LandscapeLeft:
                    case ScreenOrientation.LandscapeRight:
                        scaler.referenceResolution = new Vector2(2048, 1024);
                        scaler.matchWidthOrHeight = 1;
                        break;
                    case ScreenOrientation.AutoRotation:
                        scaler.referenceResolution = new Vector2(2048, 1024);
                        scaler.matchWidthOrHeight = 0.5f;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            
            gameObject.AddComponent<FadeViewAnimator>();

            var layer = SortingLayer.NameToID("UI");
            _canvas.overrideSorting = SortingLayer.IsValid(layer);
            _canvas.sortingLayerID = layer;

#if UNITY_EDITOR
            while (UnityEditorInternal.ComponentUtility.MoveComponentDown(this)) {}
#endif
        }
    }
}