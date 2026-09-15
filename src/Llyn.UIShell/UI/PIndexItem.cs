using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PIndexItem : INotifyPropertyChanged
{
    private bool _pIndexItemChosen;

    internal PIndexItem(long id, string headword, string language, string epithet = "")
    {
        PIndexItemId = id;
        PIndexItemHeadword = headword;
        PIndexItemName = headword;
        PIndexItemEpithet = epithet ?? string.Empty;
        PIndexItemLanguage = language;
        PIndexItemFlag = PEnsign.PEnsignFind(language);
    }

    public long PIndexItemId { get; }

    public string PIndexItemHeadword { get; }

    public string PIndexItemEpithet { get; }

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
