using System.Collections.Generic;
using UnityEngine;

namespace GameKit.UI.Core
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ProcessorSafeArea
    {
        private readonly List<SafeAreaRect> items = new List<SafeAreaRect>();
        private bool ready;
        private Rect safeArea;
        
        public ProcessorSafeArea()
        {
            Loop.EventUpdate += OnUpdate;
        }

        public void Subscribe(SafeAreaRect rect)
        {
            items.Add(rect);
            if (ready)
                rect.Sync(safeArea);
        }

        public void Unsubscribe(SafeAreaRect rect)
        {
            var i = items.IndexOf(rect);
            if (i >= 0) items.RemoveAt(i);
        }
        
        private void OnUpdate()
        {
            if (safeArea == Screen.safeArea && ready) return;
            
            safeArea = Screen.safeArea;
            if (float.IsNaN(safeArea.x) || float.IsInfinity(safeArea.x) || float.IsNaN(safeArea.y) ||
                float.IsInfinity(safeArea.y))
            {
                ready = false;
                return;
            }
            
            ready = true;
            foreach (var item in items)
            {
                item.Sync(safeArea);
            }
        }
    }
}