using System;
using System.Collections;
using System.Collections.Generic;
using GameKit.UI.Implementation;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.UI.Resolvers
{
    public class PoolViewResolver : PrefabViewResolver, IViewResolver, IDisposable
    {
        internal enum State
        {
            Empty,
            Pooled,
            Issued,
        }

        internal class PoolItem
        {
            public State State;
            public ViewComponent Instance;

            public void OnDestroy(ViewComponent view)
            {
                Instance = null;
                State = State.Empty;
            }
        }
        
        internal class IssuedEnumerator : IEnumerator<ViewComponent>
        {
            private IEnumerator<PoolItem> itemsEnumerator;
        
            internal IssuedEnumerator(IEnumerator<PoolItem> itemsEnumerator)
            {
                this.itemsEnumerator = itemsEnumerator;
            }
        
            public bool MoveNext()
            {
                Current = default;
                if (itemsEnumerator.MoveNext() == false) return false;
                var item = itemsEnumerator.Current;
                if (item == null) return false;
                if (item.State != State.Issued) return MoveNext();
                if (item.Instance == null) return MoveNext();
                Current = item.Instance;
                return true;
            }

            public void Reset()
            {
                itemsEnumerator.Reset();
            }
            
            public ViewComponent Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                itemsEnumerator.Dispose();
                itemsEnumerator = null;
            }
        }
        
        
        private List<PoolItem> items = new List<PoolItem>(1);

        public PoolViewResolver(string prefabPath) : base(prefabPath) { }

        public ViewComponent Resolve()
        {
            foreach (var item in items)
            {
                if (item.State == State.Pooled)
                {
                    item.State = State.Issued;
                    return item.Instance;
                }
            }
            
            return AddIssuedInstance(CreateInstance()).Instance;
        }

        public void Release(ViewComponent view)
        {
            view.HideObject();
            foreach (var item in items)
            {
                if (item.Instance == view)
                {
                    item.State = State.Pooled;
                    return;
                }
            }
            
            AddIssuedInstance(view).State = State.Pooled;
        }

        private PoolItem AddIssuedInstance(ViewComponent instance)
        {
            PoolItem e = null;
            foreach (var item in items)
            {
                if (item.State == State.Empty)
                {
                    e = item;
                    break;
                }
            }

            if (e == null)
            {
                e = new PoolItem();
                items.Add(e);
            }

            e.Instance = instance;
            e.Instance.EventDestroy += e.OnDestroy;
            e.State = State.Issued;
            return e;
        }
        
        public void Dispose()
        {
            foreach (var item in items)
            {
                if (item.Instance == null) continue;
                switch (item.State)
                {
                    case State.Empty:
                        break;
                    case State.Pooled:
                        Object.Destroy(item.Instance);
                        break;
                    case State.Issued:
                        Debug.LogError($"Destroying active view: " + item.Instance);
                        Object.Destroy(item.Instance);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            }
            items.Clear();
        }

        public IEnumerator<ViewComponent> GetEnumerator()
        {
            return new IssuedEnumerator(items.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}