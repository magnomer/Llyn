using System;
using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed class CSCustoms
{
    private readonly List<CSCustomsMode> _csCustomsMode = [];

    private readonly List<long> _csCustomsTarget = [];

    public CSCustoms(IReadOnlyList<IReadOnlyList<long>> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates);

        foreach (IReadOnlyList<long> row in candidates)
        {
            bool single = row.Count == 1;
            _csCustomsMode.Add(single ? CSCustomsMode.CSCustomsModeMerge : CSCustomsMode.CSCustomsModeFresh);
            _csCustomsTarget.Add(single ? row[0] : 0);
        }
    }

    public CSCustomsRow CSCustomsRowRead(int row)
    {
        CSCustomsMode mode = _csCustomsMode[row];
        long target = _csCustomsTarget[row];
        bool targeted = mode != CSCustomsMode.CSCustomsModeFresh;
        long loss = mode == CSCustomsMode.CSCustomsModeReplace ? target : 0;
        return new CSCustomsRow(mode, target, targeted, loss);
    }

    public bool CSCustomsModeSet(int row, CSCustomsMode mode)
    {
        _csCustomsMode[row] = mode;
        return CSCustomsReadyCheck();
    }

    public bool CSCustomsTargetSet(int row, long target)
    {
        _csCustomsTarget[row] = target;
        return CSCustomsReadyCheck();
    }

    public bool CSCustomsReadyCheck()
    {
        for (int row = 0; row < _csCustomsMode.Count; row++)
        {
            if (_csCustomsMode[row] != CSCustomsMode.CSCustomsModeFresh && _csCustomsTarget[row] <= 0)
            {
                return false;
            }
        }

        return true;
    }

    public static int CSCustomsCardScan<CSCustomsCard>(
        IReadOnlyList<CSCustomsCard> cards, Func<CSCustomsCard, IReadOnlyList<CSCustomsCard>> children)
    {
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(children);

        int count = 0;
        foreach (CSCustomsCard card in cards)
        {
            count += 1 + CSCustomsCardScan(children(card), children);
        }

        return count;
    }
}
