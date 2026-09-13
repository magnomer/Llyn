using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PFootnoteItem : INotifyPropertyChanged
{
    private bool _pFootnoteItemChosen;

    internal PFootnoteItem(long id, string headword, string language)
    {
        PFootnoteItemId = id;
        PFootnoteItemHeadword = headword;
        PFootnoteItemName = headword;
        PFootnoteItemLanguage = language;
        PFootnoteItemFlag = PEnsign.PEnsignFind(language);
    }

    public long PFootnoteItemId { get; }

    public string PFootnoteItemHeadword { get; }

    public string PFootnoteItemName { get; internal set; }

    public string PFootnoteItemLanguage { get; }

    public ImageSource? PFootnoteItemFlag { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PFootnoteItemChosen
    {
        get => _pFootnoteItemChosen;

        set
        {
            if (_pFootnoteItemChosen == value)
            {
                return;
            }

            _pFootnoteItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PFootnoteItemChosen)));
        }
    }
}
