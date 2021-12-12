using System;
using System.Collections;
using System.Collections.Generic;
using GameKit.UI.Implementation;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.UI.Resolvers
{
    public class ShadowViewResolver: IViewResolver, IDisposable
    {
        private ViewShading instance;
        private string path;

        public ShadowViewResolver(string prefabPath)
        {
            path = prefabPath;
        }
        
        public ViewComponent Resolve()
        {
            if (instance == null)
            {
                instance = CreateInstance();
                instance.EventDestroy += OnInstanceDestroy;
            }

            return instance;
        }

        public void Release(ViewComponent view)
        {
            view.HideObject();
            if (instance == null)
                instance = (ViewShading) view;
        }
        
        public bool FindDisplayed(out ViewComponent view)
        {
            view = instance;
            return instance != null && instance.IsDisplayed;
        }

        public void Dispose()
        {
            if (instance != null) 
                Object.Destroy(instance);
        }
        
        private void OnInstanceDestroy(ViewComponent view)
        {
            instance = null;
        }
        
        private ViewShading CreateInstance()
        {
            var prefab = Resources.Load<GameObject>(path);
            if (prefab != null)
            {
                prefab.SetActive(false);
                var obj = Object.Instantiate(prefab);
#if UNITY_EDITOR
                prefab.SetActive(true);
#endif
                var view = obj.GetComponent<ViewShading>();
                view.name = view.GetType().Name;
                return view;
            }
            else
            {
                var view = ViewShading.CreateBlackout();
                return view;
            }
        }

        public IEnumerator<ViewComponent> GetEnumerator()
        {
            return new InstanceEnumerator(instance);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}