using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PInventoryItem : INotifyPropertyChanged
{
    private bool _pInventoryItemChosen;

    internal PInventoryItem(string id, string headword, string language, string sound)
    {
        PInventoryItemId = id;
        PInventoryItemHeadword = headword;
        PInventoryItemLanguage = language;
        PInventoryItemSound = sound;
        PInventoryItemPronunciation = sound.Length == 0 ? "[ ]" : $"[{sound}]";
        PInventoryItemFlag = PEnsign.PEnsignFind(language);
    }

    public string PInventoryItemId { get; }

    public string PInventoryItemHeadword { get; }

    public string PInventoryItemLanguage { get; }

    public string PInventoryItemSound { get; }

    public string PInventoryItemPronunciation { get; }

    public ImageSource? PInventoryItemFlag { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PInventoryItemChosen
    {
        get => _pInventoryItemChosen;

        set
        {
            if (_pInventoryItemChosen == value)
            {
                return;
            }

            _pInventoryItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PInventoryItemChosen)));
        }
    }
}
