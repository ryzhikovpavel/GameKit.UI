﻿using GameKit.UI.Animators;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameKit.UI.Core
{
    [RequireComponent(typeof(Canvas))]
    public class ViewShading : ViewComponent
    { 
        internal static ViewShading CreateBlackout()
        {
            var obj = new GameObject("ShadingBlackout");
            obj.AddComponent<CanvasRenderer>();
            
            var view = obj.AddComponent<ViewShading>();
            view.Reset();
            
            var img = obj.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0.5f);
            obj.SetActive(false);
            return view;
        }
    }
}