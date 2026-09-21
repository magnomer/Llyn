using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PKindredItem : INotifyPropertyChanged
{
    private bool _pKindredItemChosen;

    internal PKindredItem(LVistaRow row, bool chosen)
    {
        _pKindredItemChosen = chosen;
        PKindredItemId = row.LVistaRowId;
        PKindredItemHeadword = row.LVistaRowHeadword;
        PKindredItemName = row.LVistaRowName;
        PKindredItemEpithet = row.LVistaRowEpithet ?? string.Empty;
        PKindredItemFlag = PEnsign.PEnsignFind(row.LVistaRowLanguage);
    }

    public long PKindredItemId { get; }

    public string PKindredItemHeadword { get; }

    public string PKindredItemEpithet { get; }

    public string PKindredItemName { get; }

    public ImageSource? PKindredItemFlag { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PKindredItemChosen
    {
        get => _pKindredItemChosen;

        set
        {
            if (_pKindredItemChosen == value)
            {
                return;
            }

            _pKindredItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PKindredItemChosen)));
        }
    }

    internal static IReadOnlyList<PKindredItem> PKindredItemBuild(IReadOnlyList<LVistaRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<PKindredItem> built = new(rows.Count);
        foreach (LVistaRow row in rows)
        {
            built.Add(new PKindredItem(row, row.LVistaRowChosen));
        }

        return built;
    }

    internal static bool PKindredItemMatch(PKindredItem held, PKindredItem fresh)
    {
        return held.PKindredItemId == fresh.PKindredItemId
            && string.Equals(held.PKindredItemHeadword, fresh.PKindredItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.PKindredItemEpithet, fresh.PKindredItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.PKindredItemName, fresh.PKindredItemName, StringComparison.Ordinal);
    }

    internal static void PKindredItemSync(PKindredItem held, PKindredItem fresh)
    {
        held.PKindredItemChosen = fresh.PKindredItemChosen;
    }
}
