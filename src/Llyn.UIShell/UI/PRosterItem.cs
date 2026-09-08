using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PRosterItem : INotifyPropertyChanged
{
    private bool _pRosterItemChosen;

    internal PRosterItem(string id, string headword, string language)
    {
        PRosterItemId = id;
        PRosterItemHeadword = headword;
        PRosterItemLanguage = language;
        PRosterItemFlag = PEnsign.PEnsignFind(language);
    }

    public string PRosterItemId { get; }

    public string PRosterItemHeadword { get; }

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
