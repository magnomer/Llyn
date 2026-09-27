using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PCitationItem
{
    internal PCitationItem(long id, string name)
    {
        PCitationItemId = id;
        PCitationItemName = name;
    }

    public long PCitationItemId { get; }

    public string PCitationItemName { get; }

    internal static PCitationItem PCitationItemCreate(CCatalogReference row)
    {
        return new PCitationItem(row.CCatalogReferenceId, row.CCatalogReferenceByline);
    }
}
