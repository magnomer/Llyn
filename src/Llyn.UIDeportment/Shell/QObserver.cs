using System;
using System.Windows.Threading;

namespace Llyn.UIDeportment;

public static class QObserver
{
    public static Action<LStep> QObserverCreate<LStep>(DispatcherObject surface, Action target)
    {
        ArgumentNullException.ThrowIfNull(target);

        return QObserverCreate<LStep>(surface, _ => target());
    }

    public static Action<LStep> QObserverCreate<LStep>(DispatcherObject surface, Action<LStep> target)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(target);

        return QObserverCreate(surface.Dispatcher, target);
    }

    public static Action<LStep> QObserverCreate<LStep>(Action<LStep> target)
    {
        ArgumentNullException.ThrowIfNull(target);

        return QObserverCreate(Dispatcher.CurrentDispatcher, target);
    }

    private static Action<LStep> QObserverCreate<LStep>(Dispatcher dispatcher, Action<LStep> target)
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
