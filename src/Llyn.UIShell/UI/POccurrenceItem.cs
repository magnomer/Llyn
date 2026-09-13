using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class POccurrenceItem : INotifyPropertyChanged
{
    private bool _pOccurrenceItemChosen;

    internal POccurrenceItem(long id, string headword, string language)
    {
        POccurrenceItemId = id;
        POccurrenceItemHeadword = headword;
        POccurrenceItemName = headword;
        POccurrenceItemLanguage = language;
        POccurrenceItemFlag = PEnsign.PEnsignFind(language);
    }

    public long POccurrenceItemId { get; }

    public string POccurrenceItemHeadword { get; }

    public string POccurrenceItemName { get; internal set; }

    public string POccurrenceItemLanguage { get; }

    public ImageSource? POccurrenceItemFlag { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool POccurrenceItemChosen
    {
        get => _pOccurrenceItemChosen;

        set
        {
            if (_pOccurrenceItemChosen == value)
            {
                return;
            }

            _pOccurrenceItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(POccurrenceItemChosen)));
        }
    }
}
