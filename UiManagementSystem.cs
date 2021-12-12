using System;
using System.Collections.Generic;
using GameKit.UI.Implementation;
using GameKit.UI.Resolvers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace GameKit.UI
{
    public class UiManagementSystem
    {
        private readonly Dictionary<Type, IViewResolver> resolvers = new Dictionary<Type, IViewResolver>();
        private readonly List<ViewScreen> screensHistory = new List<ViewScreen>();
        private ViewScreen activeScreen;
        private ViewShading shading;

        private Layer screenLayer;
        private Layer dialogLayer;

        public Settings Settings { get; } = new Settings();
        
        public UiManagementSystem()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                    OnSceneLoaded(scene, LoadSceneMode.Additive);
            }
            
            screenLayer = new Layer(0, 90, 10);
            dialogLayer = new Layer(100, 1000, 10);
        }

        internal void NotifyDialogOpen(ViewComponent dialog)
        {
            screenLayer.NotifySuspend();
            dialogLayer.NotifySuspend();
            
            if (dialogLayer.Count == 0)
            {
                if (shading == null)
                {
                    shading = (ViewShading)Resolve(typeof(ViewShading)) ?? ViewShading.CreateBlackout();
                    shading.Initialize();
                }
                shading.ShowInternal();
            }
            dialogLayer.PushToTop(shading);
            dialogLayer.PushToTop(dialog);
        }

        internal void NotifyDialogClose(ViewComponent dialog)
        {
            dialogLayer.Remove(dialog);
            if (dialogLayer.Count == 1)
            {
                shading.HideInternal();
                dialogLayer.Remove(shading);
                screenLayer.NotifyTopResume();
            }
            else
            {
                dialogLayer.NotifyTopResume();
            }

            resolvers[dialog.GetType()].Release(dialog);
        }
        
        public void TransitionTo<TScreen>() where TScreen : ViewScreen
        {
            TransitionTo<TScreen>(Settings.Transition);
        }

        public void TransitionTo<TScreen>(Transition transition) where TScreen : ViewScreen
        {
            TransitionTo((TScreen) Resolve(typeof(TScreen)), transition);
        }
        
        public void TransitionTo(ViewScreen screen, Transition transition)
        {
            CloseDialogs();
            screenLayer.Clear();

            if (activeScreen == null)
            {
                screen.ShowInternal();
            }
            else
            {
                screensHistory.Add(activeScreen);
                screenLayer.PushToTop(activeScreen);
                switch (transition)
                {
                    case Transition.Sequence:
                        activeScreen.HideInternal(screen.ShowInternal);
                        break;
                    case Transition.Simultaneously:
                        activeScreen.HideInternal();
                        screen.ShowInternal();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(transition), transition, null);
                }
            }
            
            screenLayer.PushToTop(screen);
            activeScreen = screen;
        }
        
        public TDialog Create<TDialog>() where TDialog: ViewDialog
        {
            var view = (TDialog) Resolve(typeof(TDialog));
            return view;
        }
        
        public void CloseDialogs()
        {
            foreach (var resolver in resolvers.Values)
            {
                foreach (var view in resolver)
                {
                    if (view is ViewDialog d && view.IsDisplayed && view.State != ViewState.Hiding)
                    {
                        d.CloseCancel();
                    }
                }
            }
        }

        public void CloseDialogs<TDialog>() where TDialog: ViewDialog
        {
            if (resolvers.TryGetValue(typeof(TDialog), out var resolver))
            {
                // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                foreach (ViewDialog view in resolver)
                {
                    if (view.IsDisplayed && view.State != ViewState.Hiding) view.CloseCancel();
                }
            }
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CheckAndInitializeEventSystem();
            
            var canvasType = typeof(Canvas);
            var objects = scene.GetRootGameObjects();
            foreach (var obj in objects)
            {
                if (obj.TryGetComponent(canvasType, out Component canvas))
                {
                    var views = obj.GetComponentsInChildren<ViewComponent>(true);
                    foreach (var view in views)
                    {
                        var type = view.GetType();

                        if (resolvers.TryGetValue(type, out var resolver) == false)
                            resolvers[type] = resolver = CreateResolverFor(type);

                        resolver.Release(view);
                        view.Initialize();
                    }
                }
            }
        }
        
        private void OnActiveSceneChanged(Scene prevActive, Scene newActive)
        {
            CheckAndInitializeEventSystem();
        }
        
        public bool IsActive<T>() where T: ViewComponent
        {
            if (resolvers.TryGetValue(typeof(T), out var resolver))
            {
                foreach (var view in resolver)
                {
                    if (view.IsDisplayed) return true;
                }
            }
            return false;
        }

        public bool FindActive<T>(out T view) where T: ViewComponent
        {
            if (resolvers.TryGetValue(typeof(T), out var resolve))
            {
                foreach (var item in resolve)
                {
                    if (item.IsDisplayed)
                    {
                        view = item as T;
                        return view != null;
                    }
                }
            }

            view = null;
            return false;
        }

        public ViewComponent Resolve(Type type)
        {
            if (TryResolveView(type, out ViewComponent view) && view != null)
                return view;

            var resolver = CreateResolverFor(type);
            resolvers[type] = resolver;
            return resolver.Resolve();
        }

        private bool TryResolveView(Type type, out ViewComponent view)
        {
            if (resolvers.TryGetValue(type, out var pool))
            {
                view = pool.Resolve();
                return view != null;
            }

            view = null;
            return false;
        }

        private IViewResolver CreateResolverFor(Type type)
        {
            if (typeof(ViewScreen).IsAssignableFrom(type)) return new SingletonViewResolver(GetPath(type));
            if (typeof(ViewDialog).IsAssignableFrom(type)) return new PoolViewResolver(GetPath(type));
            if (type == typeof(ViewShading)) return new ShadowViewResolver(GetPath(type));
            throw new Exception($"Cannot create resolver for {type.Name}");
        }
        
        private string GetPath(Type type)
        {
            string path = Settings.PrefabsFolder;
            if (string.IsNullOrEmpty(path)) path = type.Name;
            return $"{path}/{type.Name}";
        }

        private void CheckAndInitializeEventSystem()
        {
            if (EventSystem.current == null)
            {
                var g = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }
    }
}