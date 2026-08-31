using System.Windows.Media;

namespace Llyn.UIShell;

/// <summary>
/// Presentation item for one language row in the <c>PLangcode</c> dropdown. Carries the language name
/// the row shows and the resolved flag image beside it, or <c>null</c> when the pack declares no flag
/// (the row then falls back to a neutral globe). The flag is resolved once when the list is built so
/// the menu paints without a per-row download.
/// </summary>
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
