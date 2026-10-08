using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntryClerk
{
    [Fact]
    public void EntryClerkRead_AfterCreate_ReturnsStoredEntry()
    {
        LRig rig = TInterface.TRigClerkCreate(new TVaultFake());
        LEntryClerk clerk = TInterface.TEntryClerkCreate(rig);

        LEntry stored = rig.TEntryClerkAdd(TInterface.TEntryCreate(0, "kindle", "English", 0, null, null));

        LEntry? read = clerk.TEntryClerkRead(stored.LEntryId);
        Assert.NotNull(read);
        Assert.Equal("kindle", read.LEntryHeadword);
    }

    [Fact]
    public void EntryClerkDelete_StoredEntry_LeavesTombstoneAndRevision()
    {
        LRig rig = TInterface.TRigClerkCreate(new TVaultFake());
        LEntryClerk clerk = TInterface.TEntryClerkCreate(rig);
        LEntry stored = rig.TEntryClerkAdd(TInterface.TEntryCreate(0, "kindle", "English", 0, null, null));

        LRevision revision = clerk.TEntryClerkDelete(stored.LEntryId);

        Assert.Null(clerk.TEntryClerkRead(stored.LEntryId));
        LTombstone? stone = rig.TTombstoneRead(stored.LEntryId);
        Assert.NotNull(stone);
        Assert.Equal(revision.LRevisionId, stone.LTombstoneRevisionId);
        Assert.Equal(revision.LRevisionId, rig.TRevisionRead());
        LRevisionDelta change = Assert.Single(rig.TRevisionChangeRead(revision.LRevisionId));
        Assert.Equal("kindle", change.LRevisionDeltaSummary);
    }

    [Fact]
    public void EntryClerkDelete_ZeroId_ThrowsOutOfRange()
    {
        LEntryClerk clerk = TInterface.TEntryClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));

        Assert.Throws<ArgumentOutOfRangeException>(() => clerk.TEntryClerkDelete(0));
    }
}
