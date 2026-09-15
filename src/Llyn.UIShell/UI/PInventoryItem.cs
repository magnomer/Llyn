using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PInventoryItem : INotifyPropertyChanged
{
    private bool _pInventoryItemChosen;

    internal PInventoryItem(long id, string headword, string language, string sound, string epithet = "")
    {
        PInventoryItemId = id;
        PInventoryItemHeadword = headword;
        PInventoryItemName = headword;
        PInventoryItemEpithet = epithet ?? string.Empty;
        PInventoryItemLanguage = language;
        PInventoryItemSound = sound;
        PInventoryItemPronunciation = sound.Length == 0 ? "[ ]" : $"[{sound}]";
        PInventoryItemFlag = PEnsign.PEnsignFind(language);
    }

    public long PInventoryItemId { get; }

    public string PInventoryItemHeadword { get; }

    public string PInventoryItemEpithet { get; }

    public string PInventoryItemName { get; internal set; }

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
