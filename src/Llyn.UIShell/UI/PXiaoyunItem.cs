using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PXiaoyunItem : INotifyPropertyChanged
{
    private bool _pXiaoyunItemChosen;

    internal PXiaoyunItem(long id, string headword, string language)
    {
        PXiaoyunItemId = id;
        PXiaoyunItemHeadword = headword;
        PXiaoyunItemName = headword;
        PXiaoyunItemFlag = PEnsign.PEnsignFind(language);
    }

    public long PXiaoyunItemId { get; }

    public string PXiaoyunItemHeadword { get; }

    public string PXiaoyunItemName { get; internal set; }

    public ImageSource? PXiaoyunItemFlag { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PXiaoyunItemChosen
    {
        get => _pXiaoyunItemChosen;

        set
        {
            if (_pXiaoyunItemChosen == value)
            {
                return;
            }

            _pXiaoyunItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PXiaoyunItemChosen)));
        }
    }
}
