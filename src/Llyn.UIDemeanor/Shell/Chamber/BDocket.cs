using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Llyn.UIDemeanor;

internal sealed class BDocket
{
    private readonly BLoop _bDocketLoop;
    private readonly Task _bDocketExit;
    private readonly Action _bDocketChanged;
    private readonly List<BSParley> _bDocketParleys = [];

    internal BDocket(BLoop loop, Task exit, Action changed)
    {
        ArgumentNullException.ThrowIfNull(loop);
        ArgumentNullException.ThrowIfNull(exit);
        ArgumentNullException.ThrowIfNull(changed);

        _bDocketLoop = loop;
        _bDocketExit = exit;
        _bDocketChanged = changed;
    }

    internal IReadOnlyList<BSParley> BDocketParleys => _bDocketParleys;

    internal int? BDocketConsult(BSParley parley, Action<Exception>? fault = null, Task? until = null)
    {
        ArgumentNullException.ThrowIfNull(parley);

        if (_bDocketExit.IsCompleted)
        {
            return null;
        }

        _bDocketParleys.Add(parley);
        _bDocketChanged();
        _bDocketLoop.BLoopRun(
            until is null
                ? Task.WhenAny(parley.BSParleyAnswer, _bDocketExit)
                : Task.WhenAny(parley.BSParleyAnswer, until, _bDocketExit),
            fault);
        if (until is not null)
        {
            _bDocketParleys.Remove(parley);
            _bDocketChanged();
        }

        parley.BSParleyKeyObserve(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        return parley.BSParleyAnswer.Result;
    }

    internal void BDocketKeyObserve(ConsoleKeyInfo key)
    {
        BSParley parley = _bDocketParleys[^1];
        parley.BSParleyKeyObserve(key);
        if (parley.BSParleyAnswer.IsCompleted)
        {
            _bDocketParleys.Remove(parley);
        }

        _bDocketChanged();
    }
}
