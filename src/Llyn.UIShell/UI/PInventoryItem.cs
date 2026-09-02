using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PInventoryItem
{
    internal PInventoryItem(string id, string headword, string language, string sound)
    {
        PInventoryItemId = id;
        PInventoryItemHeadword = headword;
        PInventoryItemLanguage = language;
        PInventoryItemSound = sound;
        PInventoryItemPronunciation = sound.Length == 0 ? "[ ]" : $"[{sound}]";
        PInventoryItemFlag = PLangcodeIndicator.PLangcodeIndicatorFind(language);
    }

    public string PInventoryItemId { get; }

    public string PInventoryItemHeadword { get; }

    public string PInventoryItemLanguage { get; }

    public string PInventoryItemSound { get; }

    public string PInventoryItemPronunciation { get; }

    public ImageSource? PInventoryItemFlag { get; }
}
