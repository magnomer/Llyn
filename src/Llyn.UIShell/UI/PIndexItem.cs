using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PIndexItem
{
    internal PIndexItem(string id, string headword, string language)
    {
        PIndexItemId = id;
        PIndexItemHeadword = headword;
        PIndexItemLanguage = language;
        PIndexItemFlag = PLangcodeIndicator.PLangcodeIndicatorFind(language);
    }

    public string PIndexItemId { get; }

    public string PIndexItemHeadword { get; }

    public string PIndexItemLanguage { get; }

    public ImageSource? PIndexItemFlag { get; }
}
