using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PLangcodeItem
{
    internal PLangcodeItem(string name, ImageSource? flag)
    {
        PLangcodeItemName = name;
        PLangcodeItemFlag = flag;
    }

    public string PLangcodeItemName { get; }

    public ImageSource? PLangcodeItemFlag { get; }
}
