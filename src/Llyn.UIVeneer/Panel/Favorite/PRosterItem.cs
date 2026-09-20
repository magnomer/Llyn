using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIVeneer;

internal sealed class PRosterItem : INotifyPropertyChanged
{
    private bool _pRosterItemChosen;

    internal PRosterItem(long id, string headword, string language, string epithet = "", bool chosen = false)
    {
        _pRosterItemChosen = chosen;
        PRosterItemId = id;
        PRosterItemHeadword = headword;
        PRosterItemName = headword;
        PRosterItemEpithet = epithet ?? string.Empty;
        PRosterItemLanguage = language;
        PRosterItemFlag = PEnsign.PEnsignFind(language);
    }

    public long PRosterItemId { get; }

    public string PRosterItemHeadword { get; }

    public string PRosterItemEpithet { get; }

    public string PRosterItemName { get; internal set; }

    public string PRosterItemLanguage { get; }

    public ImageSource? PRosterItemFlag { get; }

    internal static bool PRosterItemMatch(PRosterItem held, PRosterItem fresh)
    {
        return held.PRosterItemId == fresh.PRosterItemId
            && string.Equals(held.PRosterItemHeadword, fresh.PRosterItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.PRosterItemEpithet, fresh.PRosterItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.PRosterItemName, fresh.PRosterItemName, StringComparison.Ordinal)
            && string.Equals(held.PRosterItemLanguage, fresh.PRosterItemLanguage, StringComparison.Ordinal);
    }

    internal static void PRosterItemSync(PRosterItem held, PRosterItem fresh)
    {
        held.PRosterItemChosen = fresh.PRosterItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PRosterItemChosen
    {
        get => _pRosterItemChosen;

        set
        {
            if (_pRosterItemChosen == value)
            {
                return;
            }

            _pRosterItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PRosterItemChosen)));
        }
    }
}
