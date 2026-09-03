using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PTranslationChip
{
    internal PTranslationChip(string id, string headword, string language)
    {
        PTranslationChipId = id;
        PTranslationChipHeadword = headword;
        PTranslationChipLanguage = language;
        PTranslationChipFlag = PLangcodeIndicator.PLangcodeIndicatorFind(language);
    }

    public string PTranslationChipId { get; }

    public string PTranslationChipHeadword { get; }

    public string PTranslationChipLanguage { get; }

    public ImageSource? PTranslationChipFlag { get; }
}
