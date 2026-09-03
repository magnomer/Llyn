using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PTranslationItem
{
    internal PTranslationItem(string id, string headword, string language, bool fresh)
    {
        PTranslationItemId = id;
        PTranslationItemHeadword = headword;
        PTranslationItemLanguage = language;
        PTranslationItemFlag = PLangcodeIndicator.PLangcodeIndicatorFind(language);
        PTranslationItemFresh = fresh;
    }

    public string PTranslationItemId { get; }

    public string PTranslationItemHeadword { get; }

    public string PTranslationItemLanguage { get; }

    public ImageSource? PTranslationItemFlag { get; }

    public bool PTranslationItemFresh { get; }
}
