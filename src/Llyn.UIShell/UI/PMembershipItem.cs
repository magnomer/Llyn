using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PMembershipItem : INotifyPropertyChanged
{
    private bool _pMembershipItemChosen;

    internal PMembershipItem(string id, string headword, string language)
    {
        PMembershipItemId = id;
        PMembershipItemHeadword = headword;
        PMembershipItemName = headword;
        PMembershipItemLanguage = language;
        PMembershipItemFlag = PEnsign.PEnsignFind(language);
    }

    public string PMembershipItemId { get; }

    public string PMembershipItemHeadword { get; }

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
