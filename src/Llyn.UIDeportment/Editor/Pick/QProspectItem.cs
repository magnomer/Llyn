using System.Windows.Media;

namespace Llyn.UIDeportment;

internal sealed class QProspectItem
{
    internal QProspectItem(
        long? id, string headword, string language, string epithet = "", string? name = null)
    {
        QProspectItemId = id;
        QProspectItemHeadword = headword;
        QProspectItemName = name ?? headword;
        QProspectItemEpithet = epithet ?? string.Empty;
        QProspectItemLanguage = language;
        QProspectItemFlag = QEnsignImage.QEnsignRead(language);
    }

    public long? QProspectItemId { get; }

    public string QProspectItemHeadword { get; }

    public string QProspectItemEpithet { get; }

    public string QProspectItemName { get; }

    public string QProspectItemLanguage { get; }

    public ImageSource? QProspectItemFlag { get; }

    public bool QProspectItemFresh => QProspectItemId is null;
}
