using System.Windows.Media;

namespace Llyn.UIVeneer;

internal sealed class PEtymologyChip
{
    internal PEtymologyChip(long id, string headword, string language, bool closable)
    {
        PEtymologyChipId = id;
        PEtymologyChipHeadword = headword;
        PEtymologyChipLanguage = language;
        PEtymologyChipClosable = closable;
        PEtymologyChipFlag = PEnsign.PEnsignFind(language);
    }

    public long PEtymologyChipId { get; }

    public string PEtymologyChipHeadword { get; }

    public string PEtymologyChipLanguage { get; }

    public bool PEtymologyChipClosable { get; }

    public ImageSource? PEtymologyChipFlag { get; }
}
