using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntryClerk
{
    [Fact]
    public void EntryClerkRead_AfterCreate_ReturnsStoredEntry()
    {
        LEntryClerk clerk = TInterface.TEntryClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));

        LEntry stored = clerk.TEntryClerkAdd(TInterface.TEntryCreate(0, "kindle", "English", 0, null, null));

        LEntry? read = clerk.TEntryClerkRead(stored.LEntryId);
        Assert.NotNull(read);
        Assert.Equal("kindle", read.LEntryHeadword);
    }

    [Fact]
    public void EntryClerkFind_ReverseOrder_ListsLastHeadwordFirst()
    {
        LEntryClerk clerk = TInterface.TEntryClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        clerk.TEntryClerkAdd(TInterface.TEntryCreate(0, "ash", "English", 0, null, null));
        clerk.TEntryClerkAdd(TInterface.TEntryCreate(0, "kindle", "English", 0, null, null));

        IReadOnlyList<LEntry> found = clerk.TEntryClerkFind(string.Empty, LCatalogOrder.LCatalogOrderReverse);

        Assert.Equal(["kindle", "ash"], found.Select(entry => entry.LEntryHeadword));
    }

    [Fact]
    public void EntryClerkDelete_StoredEntry_LeavesTombstoneAndRevision()
    {
        LEntryClerk clerk = TInterface.TEntryClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));
        LEntry stored = clerk.TEntryClerkAdd(TInterface.TEntryCreate(0, "kindle", "English", 0, null, null));

        LRevision revision = clerk.TEntryClerkDelete(stored.LEntryId);

        Assert.Null(clerk.TEntryClerkRead(stored.LEntryId));
        LTombstone? stone = clerk.TEntryTombstoneRead(stored.LEntryId);
        Assert.NotNull(stone);
        Assert.Equal(revision.LRevisionId, stone.LTombstoneRevisionId);
        Assert.Equal(revision.LRevisionId, clerk.TEntryRevisionRead()?.LRevisionId);
        LRevisionChange change = Assert.Single(clerk.TEntryChangeRead(revision.LRevisionId));
        Assert.Equal("kindle", change.LRevisionChangeSummary);
    }

    [Fact]
    public void EntryClerkDelete_ZeroId_ThrowsOutOfRange()
    {
        LEntryClerk clerk = TInterface.TEntryClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));

        Assert.Throws<ArgumentOutOfRangeException>(() => clerk.TEntryClerkDelete(0));
    }
}
