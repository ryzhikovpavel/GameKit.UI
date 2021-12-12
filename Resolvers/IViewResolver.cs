using System.Collections.Generic;
using GameKit.UI.Implementation;

namespace GameKit.UI.Resolvers
{
    public interface IViewResolver: IEnumerable<ViewComponent>
    {
        ViewComponent Resolve();
        void Release(ViewComponent view);
    }
}