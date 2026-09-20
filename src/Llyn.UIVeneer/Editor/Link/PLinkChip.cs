using System.Windows.Media;

namespace Llyn.UIVeneer;

internal sealed class PLinkChip
{
    internal PLinkChip(long id, string headword, string language)
    {
        PLinkChipId = id;
        PLinkChipHeadword = headword;
        PLinkChipLanguage = language;
        PLinkChipFlag = PEnsign.PEnsignFind(language);
    }

    public long PLinkChipId { get; }

    public string PLinkChipHeadword { get; }

    public string PLinkChipLanguage { get; }

    public ImageSource? PLinkChipFlag { get; }
}
