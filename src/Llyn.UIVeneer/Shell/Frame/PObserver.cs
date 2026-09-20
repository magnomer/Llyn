using System;
using System.Windows.Threading;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal static class PObserver
{
    internal static Action<LBulletin> PObserverCreate(DispatcherObject surface, Action<LBulletin> target)
    {
        return PObserverCreate<LBulletin>(surface, target);
    }

    internal static Action<LBulletin> PObserverCreate(DispatcherObject surface, Action target)
    {
        ArgumentNullException.ThrowIfNull(target);

        return PObserverCreate<LBulletin>(surface, _ => target());
    }

    internal static Action<PStep> PObserverCreate<PStep>(DispatcherObject surface, Action<PStep> target)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(target);

        Dispatcher dispatcher = surface.Dispatcher;
        return step =>
        {
            if (dispatcher.CheckAccess())
            {
                target(step);
                return;
            }

            dispatcher.BeginInvoke(target, step);
        };
    }
}
