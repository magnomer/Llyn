using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PRosterItem : INotifyPropertyChanged
{
    private bool _pRosterItemChosen;

    internal PRosterItem(long id, string headword, string language, string epithet = "")
    {
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
