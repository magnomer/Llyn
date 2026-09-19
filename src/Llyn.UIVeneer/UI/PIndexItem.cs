using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIVeneer;

internal sealed class PIndexItem : INotifyPropertyChanged
{
    private bool _pIndexItemChosen;

    internal PIndexItem(long id, string headword, string language, string epithet = "", bool chosen = false)
    {
        _pIndexItemChosen = chosen;
        PIndexItemId = id;
        PIndexItemHeadword = headword;
        PIndexItemName = headword;
        PIndexItemEpithet = epithet ?? string.Empty;
        PIndexItemLanguage = language;
        PIndexItemFlag = PEnsign.PEnsignFind(language);
    }

    public long PIndexItemId { get; }

    public string PIndexItemHeadword { get; }

    public string PIndexItemEpithet { get; }

    public string PIndexItemName { get; internal set; }

    public string PIndexItemLanguage { get; }

    public ImageSource? PIndexItemFlag { get; }

    internal static bool PIndexItemMatch(PIndexItem held, PIndexItem fresh)
    {
        return held.PIndexItemId == fresh.PIndexItemId
            && string.Equals(held.PIndexItemHeadword, fresh.PIndexItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.PIndexItemEpithet, fresh.PIndexItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.PIndexItemName, fresh.PIndexItemName, StringComparison.Ordinal)
            && string.Equals(held.PIndexItemLanguage, fresh.PIndexItemLanguage, StringComparison.Ordinal);
    }

    internal static void PIndexItemSync(PIndexItem held, PIndexItem fresh)
    {
        held.PIndexItemChosen = fresh.PIndexItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PIndexItemChosen
    {
        get => _pIndexItemChosen;

        set
        {
            if (_pIndexItemChosen == value)
            {
                return;
            }

            _pIndexItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PIndexItemChosen)));
        }
    }
}
