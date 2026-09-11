using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PCohortItem : INotifyPropertyChanged
{
    private bool _pCohortItemChosen;

    internal PCohortItem(long id, string headword, string language)
    {
        PCohortItemId = id;
        PCohortItemHeadword = headword;
        PCohortItemName = headword;
        PCohortItemLanguage = language;
        PCohortItemFlag = PEnsign.PEnsignFind(language);
    }

    public long PCohortItemId { get; }

    public string PCohortItemHeadword { get; }

    public string PCohortItemName { get; internal set; }

    public string PCohortItemLanguage { get; }

    public ImageSource? PCohortItemFlag { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PCohortItemChosen
    {
        get => _pCohortItemChosen;

        set
        {
            if (_pCohortItemChosen == value)
            {
                return;
            }

            _pCohortItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PCohortItemChosen)));
        }
    }
}
