using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
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
        LPronunciation row = Assert.Single(rig.TPronunciationRead(first.LEntryId));
        Assert.Equal("/ˈkɪnd(ə)l/", row.LPronunciationIpa);
        Assert.Empty(rig.TPronunciationRead(second.LEntryId));
        Assert.Equal(2, rig.TRevisionRead());
        Assert.Equal(
            [("entry", "create", first.LEntryId), ("pronunciation", "create", row.LPronunciationId)],
            rig.TRevisionChangeRead(1)
                .Select(change =>
                    (change.LRevisionDeltaSubject, change.LRevisionDeltaKind, change.LRevisionDeltaTarget)));
    }

    [Fact]
    public void EntryClerkSave_FullDraft_RecordsEveryChildCreate()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LCardDraft meaning = TInterface.TCardDraftCreate(
            string.Empty, string.Empty, "to set something burning", [], [], [], [], [], 1);

        LEntry stored = engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate(
                "kindle",
                "English",
                string.Empty,
                string.Empty,
                [meaning],
                [],
                transcriptions: [TInterface.TTranscriptionDraftCreate("exemplar-scheme", "kɪndl")],
                reflexes: [TInterface.TReflexDraftCreate("Korean", "", "롱")]) with
            {
                LEntryDraftEtymology = TInterface.TEtymologyDraftCreate("from Old Norse kynda"),
            });

        long? revision = engine.TEngineRevisionRead();
        Assert.NotNull(revision);
        Assert.Equal(
            ["entry/create", "sense/create", "transcription/create", "reflex/create", "etymology/create"],
            workspace.TRevisionChangeRead(revision.Value)
                .Select(change => change.LRevisionDeltaSubject + "/" + change.LRevisionDeltaKind));
        Assert.Equal(stored.LEntryId, workspace.TRevisionChangeRead(revision.Value)[0].LRevisionDeltaTarget);
    }
}
