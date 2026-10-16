using System;
using System.Threading;

namespace Llyn.UIDemeanor;

public static class BObserver
{
    public static Action<LStep> BObserverCreate<LStep>(Action<LStep> target)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (SynchronizationContext.Current is not BLoop loop)
        {
            throw new InvalidOperationException("An observer needs the console loop as the current context.");
        }

        return step =>
        {
            if (SynchronizationContext.Current == loop)
            {
                target(step);
                return;
            }

            loop.BLoopDefer(() => target(step));
        };
    }
}
