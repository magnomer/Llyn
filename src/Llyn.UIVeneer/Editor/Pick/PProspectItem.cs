using System.Windows.Media;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

internal sealed class PProspectItem
{
    internal PProspectItem(
        long id, string headword, string language, bool fresh, string epithet = "", string? name = null)
    {
        PProspectItemId = id;
        PProspectItemHeadword = headword;
        PProspectItemName = name ?? headword;
        PProspectItemEpithet = epithet ?? string.Empty;
        PProspectItemLanguage = language;
        PProspectItemFlag = LEnsignImage.LEnsignFind(language);
        PProspectItemFresh = fresh;
    }

    public long PProspectItemId { get; }

    public string PProspectItemHeadword { get; }

    public string PProspectItemEpithet { get; }

    public string PProspectItemName { get; }

    public string PProspectItemLanguage { get; }

    public ImageSource? PProspectItemFlag { get; }

    public bool PProspectItemFresh { get; }
}
