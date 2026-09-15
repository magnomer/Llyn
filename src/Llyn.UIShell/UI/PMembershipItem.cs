using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PMembershipItem : INotifyPropertyChanged
{
    private bool _pMembershipItemChosen;

    internal PMembershipItem(long id, string headword, string language, string epithet = "")
    {
        PMembershipItemId = id;
        PMembershipItemHeadword = headword;
        PMembershipItemName = headword;
        PMembershipItemEpithet = epithet ?? string.Empty;
        PMembershipItemLanguage = language;
        PMembershipItemFlag = PEnsign.PEnsignFind(language);
    }

    public long PMembershipItemId { get; }

    public string PMembershipItemHeadword { get; }

    public string PMembershipItemEpithet { get; }

    public string PMembershipItemName { get; internal set; }

    public string PMembershipItemLanguage { get; }

    public ImageSource? PMembershipItemFlag { get; }

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
