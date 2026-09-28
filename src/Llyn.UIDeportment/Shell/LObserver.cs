using System;
using System.Windows.Threading;

namespace Llyn.UIDeportment;

public static class LObserver
{
    public static Action<LStep> LObserverCreate<LStep>(DispatcherObject surface, Action target)
    {
        ArgumentNullException.ThrowIfNull(target);

        return LObserverCreate<LStep>(surface, _ => target());
    }

    public static Action<LStep> LObserverCreate<LStep>(DispatcherObject surface, Action<LStep> target)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(target);

        return LObserverCreate(surface.Dispatcher, target);
    }

    public static Action<LStep> LObserverCreate<LStep>(Action<LStep> target)
    {
        ArgumentNullException.ThrowIfNull(target);

        return LObserverCreate(Dispatcher.CurrentDispatcher, target);
    }

    private static Action<LStep> LObserverCreate<LStep>(Dispatcher dispatcher, Action<LStep> target)
    {
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
