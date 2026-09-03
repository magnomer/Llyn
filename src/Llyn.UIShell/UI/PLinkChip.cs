using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PLinkChip
{
    internal PLinkChip(string id, string headword, string language)
    {
        PLinkChipId = id;
        PLinkChipHeadword = headword;
        PLinkChipLanguage = language;
        PLinkChipFlag = PEnsign.PEnsignFind(language);
    }

    public string PLinkChipId { get; }

    public string PLinkChipHeadword { get; }

    public string PLinkChipLanguage { get; }

    public ImageSource? PLinkChipFlag { get; }
}
