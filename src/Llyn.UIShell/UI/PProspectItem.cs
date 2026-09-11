using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PProspectItem
{
    internal PProspectItem(long id, string headword, string language, bool fresh)
    {
        PProspectItemId = id;
        PProspectItemHeadword = headword;
        PProspectItemName = headword;
        PProspectItemLanguage = language;
        PProspectItemFlag = PEnsign.PEnsignFind(language);
        PProspectItemFresh = fresh;
    }

    public long PProspectItemId { get; }

    public string PProspectItemHeadword { get; }

    public string PProspectItemName { get; internal set; }

    public string PProspectItemLanguage { get; }

    public ImageSource? PProspectItemFlag { get; }

    public bool PProspectItemFresh { get; }
}
