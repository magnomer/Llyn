using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Conduct;

public sealed class CSCustoms
{
    private readonly List<CSCustomsMode> _csCustomsMode = [];

    private readonly List<long> _csCustomsTarget = [];

    private readonly IReadOnlyList<CMarkupTarget> _csCustomsLoss;

    internal CSCustoms(IReadOnlyList<CMarkupEntry> entries, IReadOnlyList<CMarkupTarget> targets)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(targets);

        CSCustomsEntry = entries;
        _csCustomsLoss = targets;
        foreach (CMarkupEntry entry in entries)
        {
            bool single = entry.CMarkupEntryTarget.Count == 1;
            _csCustomsMode.Add(single ? CSCustomsMode.CSCustomsModeMerge : CSCustomsMode.CSCustomsModeFresh);
            _csCustomsTarget.Add(single ? entry.CMarkupEntryTarget[0] : 0);
        }
    }

    public IReadOnlyList<CMarkupEntry> CSCustomsEntry { get; }

    public CSCustomsRow CSCustomsRowRead(int row)
    {
        CSCustomsMode mode = _csCustomsMode[row];
        long target = _csCustomsTarget[row];
        bool targeted = mode != CSCustomsMode.CSCustomsModeFresh;
        CMarkupTarget? lost = mode == CSCustomsMode.CSCustomsModeReplace
            ? _csCustomsLoss.FirstOrDefault(found => found.CMarkupTargetId == target)
            : null;
        return lost is null
            ? new CSCustomsRow(mode, target, targeted, null, 0, 0)
            : new CSCustomsRow(
                mode, target, targeted, "Customs.Loss", lost.CMarkupTargetMeaning, lost.CMarkupTargetCollocation);
    }

    internal IReadOnlyList<CSCustomsRow> LSCustomsRowsRead()
    {
        return Enumerable.Range(0, _csCustomsMode.Count).Select(CSCustomsRowRead).ToList();
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
}
