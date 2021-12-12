using System;
using System.Collections;
using System.Collections.Generic;
using GameKit.UI.Implementation;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.UI.Resolvers
{
    public class SingletonViewResolver : PrefabViewResolver, IViewResolver, IDisposable
    {
        private ViewComponent _instance;
        
        public SingletonViewResolver(string prefabPath) : base(prefabPath) { }

        public ViewComponent Resolve()
        {
            if (_instance == null)
            {
                _instance = CreateInstance();
                _instance.EventDestroy += OnInstanceDestroy;
            }
            
            return _instance;
        }

        public void Release(ViewComponent view)
        {
            view.HideObject();
            if (_instance == null)
                _instance = view;
        }
        
        public bool FindDisplayed(out ViewComponent view)
        {
            view = _instance;
            return _instance.IsDisplayed;
        }

        public void Dispose()
        {
            if (_instance != null)
            {
                Debug.LogWarning($"View instance {_instance.name} request destroying...");
                Object.Destroy(_instance);
                _instance = null;
            }
        }

        private void OnInstanceDestroy(ViewComponent view)
        {
            _instance = null;
        }

        public IEnumerator<ViewComponent> GetEnumerator()
        {
            return new InstanceEnumerator(_instance);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}