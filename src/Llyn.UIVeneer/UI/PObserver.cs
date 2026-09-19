using System;
using System.Windows.Threading;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PObserver : LObserver
{
    private readonly Dispatcher? _pObserverDispatcher;

    private readonly Action<LBulletin> _pObserverTarget;

    internal PObserver(DispatcherObject surface, Action<LBulletin> target)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(target);

        _pObserverDispatcher = surface.Dispatcher;
        _pObserverTarget = target;
    }

    internal PObserver(DispatcherObject surface, Action target)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(target);

        _pObserverDispatcher = surface.Dispatcher;
        _pObserverTarget = _ => target();
    }

    internal PObserver(Action<LBulletin> target)
    {
        ArgumentNullException.ThrowIfNull(target);

        _pObserverDispatcher = null;
        _pObserverTarget = target;
    }

    public void LObserverBulletinHandle(LBulletin bulletin)
    {
        if (_pObserverDispatcher is null || _pObserverDispatcher.CheckAccess())
        {
            _pObserverTarget(bulletin);
            return;
        }

        _pObserverDispatcher.BeginInvoke(_pObserverTarget, bulletin);
    }
}
