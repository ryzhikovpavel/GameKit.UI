﻿using System.Collections;
using System.Collections.Generic;
using GameKit.UI.Implementation;

namespace GameKit.UI.Resolvers
{
    public class InstanceEnumerator: IEnumerator<ViewComponent>
    {
        private ViewComponent instance;
        public InstanceEnumerator(ViewComponent instance)
        {
            this.instance = instance;
        }

        public bool MoveNext()
        {
            Current = instance;
            instance = null;
            return Current != null;
        }

        public void Reset() { }

        public ViewComponent Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            Current = null;
        }
    }
}