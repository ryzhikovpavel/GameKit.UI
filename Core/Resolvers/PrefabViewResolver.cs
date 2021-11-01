using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.UI.Core
{
    public class PrefabViewResolver
    {
        private readonly string path;
        
        public PrefabViewResolver(string prefabPath)
        {
            path = prefabPath;
            
#if UNITY_EDITOR
            var prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab not found on {path}");
                return;
            }
            
            var comp = prefab.GetComponent<ViewComponent>(); 
            if (comp == null)
            {
                throw new ArgumentException($"Prefab {prefab.name} not contains ViewComponent");
            }
#endif
        }
        
        protected ViewComponent CreateInstance()
        {
            var prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
                throw new Exception($"Prefab not found on {path}");

            prefab.SetActive(false);
            var obj = Object.Instantiate(prefab);
            
#if UNITY_EDITOR
            prefab.SetActive(true);
#endif
            var view = obj.GetComponent<ViewComponent>();
            view.name = view.GetType().Name;
            view.Initialize();
            return view;
        }
    }
}