using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCitationItem
{
    internal QCitationItem(long id, string name)
    {
        QCitationItemId = id;
        QCitationItemName = name;
    }

    public long QCitationItemId { get; }

    public string QCitationItemName { get; }

    internal static QCitationItem QCitationItemCreate(CCatalogReference row)
    {
        return new QCitationItem(row.CCatalogReferenceId, row.CCatalogReferenceByline);
    }
}
