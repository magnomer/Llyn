using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PCitationItem
{
    internal PCitationItem(long id, string name)
    {
        PCitationItemId = id;
        PCitationItemName = name;
    }

    public long PCitationItemId { get; }

    public string PCitationItemName { get; }

    internal static PCitationItem PCitationItemCreate(LCatalogReference row)
    {
        return new PCitationItem(row.LCatalogReferenceStored.LReferenceId, row.LCatalogReferenceByline);
    }
}
