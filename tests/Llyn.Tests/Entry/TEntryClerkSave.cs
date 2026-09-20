using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntryClerkSave
{
    [Fact]
    public void EntryClerkSave_HeadwordOnlyDraft_StoresEntryAndRecordsCreate()
    {
        LEntryClerk clerk = TInterface.TEntryClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));

        LEntry stored = clerk.TEntryClerkSave(
            TInterface.TEntryDraftCreate("kindle", string.Empty, string.Empty, string.Empty, [], []));

        Assert.NotEqual(0, stored.LEntryId);
        Assert.Equal("kindle", clerk.TEntryClerkRead(stored.LEntryId)?.LEntryHeadword);
        LRevision? revision = clerk.TEntryRevisionRead();
        Assert.NotNull(revision);
        LRevisionChange change = Assert.Single(clerk.TEntryChangeRead(revision.LRevisionId));
        Assert.Equal("entry", change.LRevisionChangeSubject);
        Assert.Equal("create", change.LRevisionChangeKind);
        Assert.Equal(stored.LEntryId, change.LRevisionChangeTarget);
    }

    [Fact]
    public void EntryClerkSave_PronunciationDraft_WritesRowAndRecordsCreate()
    {
        LEntryClerk clerk = TInterface.TEntryClerkCreate(TInterface.TRigClerkCreate(new TVaultFake()));

        LEntry first = clerk.TEntryClerkSave(
            TInterface.TEntryDraftCreate("kindle", string.Empty, "/ˈkɪnd(ə)l/", string.Empty, [], []));
        LEntry second = clerk.TEntryClerkSave(
            TInterface.TEntryDraftCreate("ash", string.Empty, string.Empty, string.Empty, [], []));

        Assert.NotEqual(first.LEntryId, second.LEntryId);
        Assert.Equal(2, clerk.TEntryRevisionRead()?.LRevisionId);
    }
}
