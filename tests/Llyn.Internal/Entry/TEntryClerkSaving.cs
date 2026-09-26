using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntryClerkSaving
{
    [Fact]
    public void EntryClerkSave_HeadwordOnlyDraft_StoresEntryAndRecordsCreate()
    {
        LRig rig = TInterface.TRigClerkCreate(new TVaultFake());
        LEntryClerk clerk = TInterface.TEntryClerkCreate(rig);

        LEntry stored = clerk.TEntryClerkSave(
            TInterface.TEntryDraftCreate("kindle", string.Empty, string.Empty, string.Empty, [], []));

        Assert.NotEqual(0, stored.LEntryId);
        Assert.Equal("kindle", clerk.TEntryClerkRead(stored.LEntryId)?.LEntryHeadword);
        long? revision = rig.TRevisionRead();
        Assert.NotNull(revision);
        LRevisionDelta change = Assert.Single(rig.TRevisionChangeRead(revision.Value));
        Assert.Equal("entry", change.LRevisionDeltaSubject);
        Assert.Equal("create", change.LRevisionDeltaKind);
        Assert.Equal(stored.LEntryId, change.LRevisionDeltaTarget);
    }

    [Fact]
    public void EntryClerkSave_PronunciationDraft_WritesRowAndRecordsCreate()
    {
        LRig rig = TInterface.TRigClerkCreate(new TVaultFake());
        LEntryClerk clerk = TInterface.TEntryClerkCreate(rig);

        LEntry first = clerk.TEntryClerkSave(
            TInterface.TEntryDraftCreate("kindle", string.Empty, "/ˈkɪnd(ə)l/", string.Empty, [], []));
        LEntry second = clerk.TEntryClerkSave(
            TInterface.TEntryDraftCreate("ash", string.Empty, string.Empty, string.Empty, [], []));

        Assert.NotEqual(first.LEntryId, second.LEntryId);
        Assert.Equal(2, rig.TRevisionRead());
    }
}
