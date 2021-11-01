using System.Collections.Generic;
using UnityEngine;

namespace GameKit.UI.Core
{
    internal class Layer
    {
        private readonly List<ViewComponent> views = new List<ViewComponent>();
        private readonly int orderBase;
        private readonly int orderMax;
        private readonly int orderStep;
        private int orderTop;

        public int Count => views.Count;
        
        public Layer(int orderBase, int orderMax, int orderStep)
        {
            this.orderBase = orderBase;
            this.orderMax = orderMax;
            this.orderStep = orderStep;
        }

        public void Clear()
        {
            views.Clear();
        }

        public void Remove(ViewComponent view)
        {
            view.EventDestroy -= OnViewDestroying;
            views.Remove(view);
        }
        
        public void PushToTop(ViewComponent view)
        {
            Stack(view);
            view.Order = orderMax;
            views.Sort(OrderSort);
            Ordering();
        }

        public void PushToBack(ViewComponent view)
        {
            Stack(view);
            view.Order = orderBase - 1;
            views.Sort(OrderSort);
            Ordering();
        }

        private void Stack(ViewComponent view)
        {
            var index = views.IndexOf(view);
            if (index >= 0) return;
            view.EventDestroy += OnViewDestroying;
            views.Add(view);
        }
        
        private int OrderSort(ViewComponent a, ViewComponent b)
        {
            if (a == b) return 0;
            if (a == null) return 1;
            if (b == null) return -1;
            return a.Order.CompareTo(b.Order);
        }

        private void Ordering()
        {
            int order = orderBase;
            foreach (var view in views)
            {
                if (view == null)
                {
                    Debug.LogError($"View is null on Layer");
                    continue;
                }

                view.Order = order;
                order += orderStep;
            }

            orderTop = order;
            if (order >= orderMax)
            {
                Debug.LogError($"Order index out of range. Current {order} but max {orderMax}");
            }
        }

        private void OnViewDestroying(ViewComponent view)
        {
            Remove(view);
            if (view is ViewShading)
                Debug.LogWarning("Shading destroyed before hiding all dialogs");
        }
    }
}