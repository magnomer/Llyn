using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PIndexItem : INotifyPropertyChanged
{
    private bool _pIndexItemChosen;

    internal PIndexItem(string id, string headword, string language)
    {
        PIndexItemId = id;
        PIndexItemHeadword = headword;
        PIndexItemName = headword;
        PIndexItemLanguage = language;
        PIndexItemFlag = PEnsign.PEnsignFind(language);
    }

    public string PIndexItemId { get; }

    public string PIndexItemHeadword { get; }

    public string PIndexItemName { get; internal set; }

    public string PIndexItemLanguage { get; }

    public ImageSource? PIndexItemFlag { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PIndexItemChosen
    {
        get => _pIndexItemChosen;

        set
        {
            if (_pIndexItemChosen == value)
            {
                return;
            }

            _pIndexItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PIndexItemChosen)));
        }
    }
}
