using System.Windows.Media;

namespace Llyn.UIDeportment;

public sealed class LLinkChip
{
    public LLinkChip(long id, string headword, string language)
    {
        LLinkChipId = id;
        LLinkChipHeadword = headword;
        LLinkChipLanguage = language;
        LLinkChipFlag = LEnsignImage.LEnsignFind(language);
    }

    public long LLinkChipId { get; }

    public string LLinkChipHeadword { get; }

    public string LLinkChipLanguage { get; }

    public ImageSource? LLinkChipFlag { get; }
}
