using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntryQueryClerk
{
    [Fact]
    public void EntryFind_ReverseOrder_ListsLastHeadwordFirst()
    {
        LRig rig = TInterface.TRigClerkCreate(new TVaultFake());
        rig.TEntryClerkAdd(TInterface.TEntryCreate(0, "ash", "English", 0, null, null));
        rig.TEntryClerkAdd(TInterface.TEntryCreate(0, "kindle", "English", 0, null, null));

        IReadOnlyList<LEntry> found = rig.TEntryQueryFind(string.Empty, LCatalogOrder.LCatalogOrderReverse);

        Assert.Equal(["kindle", "ash"], found.Select(entry => entry.LEntryHeadword));
    }
}
