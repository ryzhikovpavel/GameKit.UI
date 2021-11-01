using System.Collections.Generic;
using GameKit.Pooling;

namespace GameKit.UI.Core
{
    public interface IViewResolver: IEnumerable<ViewComponent>
    {
        ViewComponent Resolve();
        void Release(ViewComponent view);
    }
}