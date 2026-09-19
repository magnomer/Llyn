using System;
using System.Collections.Generic;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PYunjingItem : INotifyPropertyChanged
{
    private bool _pYunjingItemChosen;

    internal PYunjingItem(LDiwei row, bool chosen)
    {
        _pYunjingItemChosen = chosen;
        PYunjingItemId = row.LDiweiId;
        PYunjingItemKey = row.LDiweiKey;
        PYunjingItemCount = row.LDiweiCount;
        PYunjingItemFinal = row.LDiweiFinal;
    }

    public long PYunjingItemId { get; }

    public string PYunjingItemKey { get; }

    public int PYunjingItemCount { get; }

    public bool PYunjingItemFinal { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PYunjingItemChosen
    {
        get => _pYunjingItemChosen;

        set
        {
            if (_pYunjingItemChosen == value)
            {
                return;
            }

            _pYunjingItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PYunjingItemChosen)));
        }
    }

    internal static IReadOnlyList<PYunjingItem> PYunjingItemBuild(IReadOnlyList<LDiwei> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<PYunjingItem> built = new(rows.Count);
        foreach (LDiwei row in rows)
        {
            built.Add(new PYunjingItem(row, row.LDiweiChosen));
        }

        return built;
    }

    internal static bool PYunjingItemMatch(PYunjingItem held, PYunjingItem fresh)
    {
        return held.PYunjingItemId == fresh.PYunjingItemId
            && string.Equals(held.PYunjingItemKey, fresh.PYunjingItemKey, StringComparison.Ordinal)
            && held.PYunjingItemCount == fresh.PYunjingItemCount
            && held.PYunjingItemFinal == fresh.PYunjingItemFinal;
    }

    internal static void PYunjingItemSync(PYunjingItem held, PYunjingItem fresh)
    {
        held.PYunjingItemChosen = fresh.PYunjingItemChosen;
    }
}
