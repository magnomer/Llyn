using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCaretTrail
{
    private readonly CDesk _cCaretTrailDesk;

    private readonly Func<LCardDraft, IEnumerable<long>> _cCaretTrailRow;

    private readonly Action<long, long> _cCaretTrailRemoval;

    private readonly Dictionary<long, IReadOnlyList<long>> _cCaretTrailKept = [];

    private LTenure? _cCaretTrailTenure;

    internal CCaretTrail(CDesk desk, Func<LCardDraft, IEnumerable<long>> row, Action<long, long> removal)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(removal);

        _cCaretTrailDesk = desk;
        _cCaretTrailRow = row;
        _cCaretTrailRemoval = removal;
    }

    public (bool CCaretMoved, long? CCaretAnchor) CCaretTrailMove(long cardId, int step)
    {
        (IReadOnlyList<long> row, int place) = LCaretTrailResolve(cardId);
        int target = place + step;
        if (target < 0 || target > row.Count)
        {
            return (false, place < row.Count ? row[place] : null);
        }

        _cCaretTrailKept[cardId] = row.Skip(target).ToList();
        return (true, target < row.Count ? row[target] : null);
    }

    public void CCaretTrailDelete(long cardId, int step)
    {
        (IReadOnlyList<long> row, int place) = LCaretTrailResolve(cardId);
        int target = place + (step < 0 ? step : step - 1);
        if (target >= 0 && target < row.Count)
        {
            _cCaretTrailRemoval(cardId, row[target]);
        }
    }

    public long? CCaretTrailRead(long cardId)
    {
        (IReadOnlyList<long> row, int place) = LCaretTrailResolve(cardId);
        return place < row.Count ? row[place] : null;
    }

    internal int CCaretTrailResolve(long cardId)
    {
        return LCaretTrailResolve(cardId).CCaretPosition;
    }

    private (IReadOnlyList<long> CCaretRow, int CCaretPosition) LCaretTrailResolve(long cardId)
    {
        LTenure? tenure = _cCaretTrailDesk.CDeskDraft.CDeskDraftTenure;
        if (!ReferenceEquals(tenure, _cCaretTrailTenure))
        {
            _cCaretTrailTenure = tenure;
            _cCaretTrailKept.Clear();
        }

        IReadOnlyList<long> row = tenure?.LTenureRead() is LDraft held
            && held.LDraftContent.LEntryDraftMeanings
                .Concat(held.LDraftContent.LEntryDraftCollocations)
                .FirstOrDefault(card => card.LCardDraftId == cardId) is LCardDraft card
            ? _cCaretTrailRow(card).ToList()
            : [];
        int place = row.Count;
        if (_cCaretTrailKept.TryGetValue(cardId, out IReadOnlyList<long>? trail))
        {
            List<long> ids = [.. row];
            foreach (long kept in trail)
            {
                int found = ids.IndexOf(kept);
                if (found >= 0)
                {
                    place = found;
                    break;
                }
            }
        }

        _cCaretTrailKept[cardId] = row.Skip(place).ToList();
        return (row, place);
    }
}
