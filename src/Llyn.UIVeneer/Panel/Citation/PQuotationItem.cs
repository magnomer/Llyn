using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIVeneer;

internal sealed class PQuotationItem : INotifyPropertyChanged
{
    private bool _pQuotationItemChosen;

    internal PQuotationItem(long id, string headword, string language, string epithet = "", bool chosen = false)
    {
        _pQuotationItemChosen = chosen;
        PQuotationItemId = id;
        PQuotationItemHeadword = headword;
        PQuotationItemEpithet = epithet ?? string.Empty;
        PQuotationItemLanguage = language;
        PQuotationItemFlag = PEnsign.PEnsignFind(language);
    }

    public long PQuotationItemId { get; }

    public string PQuotationItemHeadword { get; }

    public string PQuotationItemEpithet { get; }

    public required string PQuotationItemName { get; init; }

    public string PQuotationItemLanguage { get; }

    public ImageSource? PQuotationItemFlag { get; }

    internal static bool PQuotationItemMatch(PQuotationItem held, PQuotationItem fresh)
    {
        return held.PQuotationItemId == fresh.PQuotationItemId
            && string.Equals(held.PQuotationItemHeadword, fresh.PQuotationItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.PQuotationItemEpithet, fresh.PQuotationItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.PQuotationItemName, fresh.PQuotationItemName, StringComparison.Ordinal)
            && string.Equals(held.PQuotationItemLanguage, fresh.PQuotationItemLanguage, StringComparison.Ordinal);
    }

    internal static void PQuotationItemSync(PQuotationItem held, PQuotationItem fresh)
    {
        held.PQuotationItemChosen = fresh.PQuotationItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PQuotationItemChosen
    {
        get => _pQuotationItemChosen;

        set
        {
            if (_pQuotationItemChosen == value)
            {
                return;
            }

            _pQuotationItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PQuotationItemChosen)));
        }
    }
}
