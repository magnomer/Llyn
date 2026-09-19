using System.Windows.Media;

namespace Llyn.UIVeneer;

internal sealed class PLanguageItem
{
    internal PLanguageItem(string name, ImageSource? flag)
    {
        PLanguageItemName = name;
        PLanguageItemFlag = flag;
    }

    public string PLanguageItemName { get; }

    public ImageSource? PLanguageItemFlag { get; }
}
