using System.Windows.Media;

namespace Llyn.UIShell;

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
