using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PProspectItem
{
    internal PProspectItem(long id, string headword, string language, bool fresh, string epithet = "")
    {
        PProspectItemId = id;
        PProspectItemHeadword = headword;
        PProspectItemName = headword;
        PProspectItemEpithet = epithet ?? string.Empty;
        PProspectItemLanguage = language;
        PProspectItemFlag = PEnsign.PEnsignFind(language);
        PProspectItemFresh = fresh;
    }

    public long PProspectItemId { get; }

    public string PProspectItemHeadword { get; }

    public string PProspectItemEpithet { get; }

    public string PProspectItemName { get; internal set; }

    public string PProspectItemLanguage { get; }

    public ImageSource? PProspectItemFlag { get; }

    public bool PProspectItemFresh { get; }
}
