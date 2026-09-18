using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PMembershipItem : INotifyPropertyChanged
{
    private bool _pMembershipItemChosen;

    internal PMembershipItem(long id, string headword, string language, string epithet = "", bool chosen = false)
    {
        _pMembershipItemChosen = chosen;
        PMembershipItemId = id;
        PMembershipItemHeadword = headword;
        PMembershipItemEpithet = epithet ?? string.Empty;
        PMembershipItemLanguage = language;
        PMembershipItemFlag = PEnsign.PEnsignFind(language);
    }

    public long PMembershipItemId { get; }

    public string PMembershipItemHeadword { get; }

    public string PMembershipItemEpithet { get; }

    public required string PMembershipItemName { get; init; }

    public string PMembershipItemLanguage { get; }

    public ImageSource? PMembershipItemFlag { get; }

    internal static bool PMembershipItemMatch(PMembershipItem held, PMembershipItem fresh)
    {
        return held.PMembershipItemId == fresh.PMembershipItemId
            && string.Equals(held.PMembershipItemHeadword, fresh.PMembershipItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.PMembershipItemEpithet, fresh.PMembershipItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.PMembershipItemName, fresh.PMembershipItemName, StringComparison.Ordinal)
            && string.Equals(held.PMembershipItemLanguage, fresh.PMembershipItemLanguage, StringComparison.Ordinal);
    }

    internal static void PMembershipItemSync(PMembershipItem held, PMembershipItem fresh)
    {
        held.PMembershipItemChosen = fresh.PMembershipItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PMembershipItemChosen
    {
        get => _pMembershipItemChosen;

        set
        {
            if (_pMembershipItemChosen == value)
            {
                return;
            }

            _pMembershipItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PMembershipItemChosen)));
        }
    }
}
