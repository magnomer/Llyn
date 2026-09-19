using System;
using System.ComponentModel;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PXiaoyunItem : INotifyPropertyChanged
{
    private bool _pXiaoyunItemChosen;

    internal PXiaoyunItem(LVistaRow row, bool chosen)
    {
        _pXiaoyunItemChosen = chosen;
        PXiaoyunItemId = row.LVistaRowId;
        PXiaoyunItemHeadword = row.LVistaRowHeadword;
        PXiaoyunItemName = row.LVistaRowName;
        PXiaoyunItemEpithet = row.LVistaRowEpithet ?? string.Empty;
        PXiaoyunItemFlag = PEnsign.PEnsignFind(row.LVistaRowLanguage);
    }

    public long PXiaoyunItemId { get; }

    public string PXiaoyunItemHeadword { get; }

    public string PXiaoyunItemEpithet { get; }

    public string PXiaoyunItemName { get; }

    public ImageSource? PXiaoyunItemFlag { get; }

    internal static bool PXiaoyunItemMatch(PXiaoyunItem held, PXiaoyunItem fresh)
    {
        return held.PXiaoyunItemId == fresh.PXiaoyunItemId
            && string.Equals(held.PXiaoyunItemHeadword, fresh.PXiaoyunItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.PXiaoyunItemEpithet, fresh.PXiaoyunItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.PXiaoyunItemName, fresh.PXiaoyunItemName, StringComparison.Ordinal);
    }

    internal static void PXiaoyunItemSync(PXiaoyunItem held, PXiaoyunItem fresh)
    {
        held.PXiaoyunItemChosen = fresh.PXiaoyunItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PXiaoyunItemChosen
    {
        get => _pXiaoyunItemChosen;

        set
        {
            if (_pXiaoyunItemChosen == value)
            {
                return;
            }

            _pXiaoyunItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PXiaoyunItemChosen)));
        }
    }
}
