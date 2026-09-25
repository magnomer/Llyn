using System;
using System.Windows.Threading;
using Llyn.Core;

namespace Llyn.UIDeportment;

public static class LObserver
{
    public static Action<LBulletin> LObserverCreate(DispatcherObject surface, Action<LBulletin> target)
    {
        return LObserverCreate<LBulletin>(surface, target);
    }

    public static Action<LBulletin> LObserverCreate(DispatcherObject surface, Action target)
    {
        ArgumentNullException.ThrowIfNull(target);

        return LObserverCreate<LBulletin>(surface, _ => target());
    }

    public static Action<LStep> LObserverCreate<LStep>(DispatcherObject surface, Action<LStep> target)
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
