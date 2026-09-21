using System;
using System.Collections.Generic;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PGroveItem : INotifyPropertyChanged
{
    private bool _pGroveItemChosen;

    internal PGroveItem(LStem row, bool chosen)
    {
        _pGroveItemChosen = chosen;
        PGroveItemId = row.LStemId;
        PGroveItemKey = row.LStemKey;
        PGroveItemCount = row.LStemCount;
    }

    public long PGroveItemId { get; }

    public string PGroveItemKey { get; }

    public int PGroveItemCount { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PGroveItemChosen
    {
        get => _pGroveItemChosen;

        set
        {
            if (_pGroveItemChosen == value)
            {
                return;
            }

            _pGroveItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PGroveItemChosen)));
        }
    }

    internal static IReadOnlyList<PGroveItem> PGroveItemBuild(IReadOnlyList<LStem> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<PGroveItem> built = new(rows.Count);
        foreach (LStem row in rows)
        {
            built.Add(new PGroveItem(row, row.LStemChosen));
        }

        return built;
    }

    internal static bool PGroveItemMatch(PGroveItem held, PGroveItem fresh)
    {
        return held.PGroveItemId == fresh.PGroveItemId
            && string.Equals(held.PGroveItemKey, fresh.PGroveItemKey, StringComparison.Ordinal)
            && held.PGroveItemCount == fresh.PGroveItemCount;
    }

    internal static void PGroveItemSync(PGroveItem held, PGroveItem fresh)
    {
        held.PGroveItemChosen = fresh.PGroveItemChosen;
    }
}
