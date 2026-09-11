using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PCitationItem
{
    internal PCitationItem(long id, string name)
    {
        PCitationItemId = id;
        PCitationItemName = name;
    }

    public long PCitationItemId { get; }

    public string PCitationItemName { get; }

    internal static PCitationItem PCitationItemCreate(LReference reference)
    {
        return new PCitationItem(reference.LReferenceId, reference.LReferenceNameRead());
    }
}
