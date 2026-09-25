using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDraftCommit
{
    [Fact]
    public void DraftCommit_OwnerRefused_LeavesTargetFileUntouched()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft owner = engine.TEngineDraftStart("Input", null);
        LDraft target = engine.TEngineDraftStart("Library", null);

        engine.TRequestContentApply(target.LDraftId, TDraftContentCreate("ember"));
        engine.TEngineCourtSave(owner.LDraftId, target.LDraftId, "ember", "English");

        LRefusal refused = Assert.Throws<LRefusal>(() => engine.TEngineDraftCommit(owner.LDraftId));
        LDraft? kept = engine.TEngineDraftRead(target.LDraftId);

        Assert.Equal(LRefusal.LRefusalHeadword, refused.LRefusalReason);
        Assert.NotNull(kept);
        Assert.Equal(0, kept.LDraftEntryId);
        Assert.Empty(engine.TEngineEntryFind("ember"));
        Assert.NotNull(engine.TEngineCourtFind(owner.LDraftId, target.LDraftId));
    }

    [Fact]
    public void DraftCommit_PaddedHeadword_StoresItTrimmed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TRequestContentApply(started.LDraftId, TDraftContentCreate("  kindle  "));

        LEntry stored = engine.TEngineDraftCommit(started.LDraftId);

        Assert.Equal("kindle", stored.LEntryHeadword);
    }

    [Fact]
    public void DraftCheck_HeadwordPaddingOnly_ReportsUnchanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TRequestContentApply(
            started.LDraftId, TDraftContentCreate("kindle") with { LEntryDraftNote = "a line\nanother" });
        LEntry stored = engine.TEngineDraftCommit(started.LDraftId);

        LDraft opened = engine.TEngineDraftStart("Input", stored.LEntryId);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(opened.LDraftId, "kindle "));
        engine.TEngineRequestApply(TInterface.TRequestNoteCreate(opened.LDraftId, "a line\r\nanother  "));

        Assert.False(engine.TEngineDraftCheck(opened.LDraftId));
    }

    [Fact]
    public void DraftCheck_BlankHeadword_NamesTheRefusal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestNoteCreate(started.LDraftId, "a note without a word"));

        bool changed = engine.TEngineDraftCheck(started.LDraftId, out string? refusal);

        Assert.True(changed);
        Assert.Equal(LRefusal.LRefusalHeadword, refusal);
    }

    [Fact]
    public void EntryUpdate_NothingChanged_RecordsNoRevision()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TRequestContentApply(started.LDraftId, TDraftContentCreate("kindle"));
        LEntry stored = engine.TEngineDraftCommit(started.LDraftId);
        long? before = engine.TEngineRevisionRead();

        engine.TEngineEntryUpdate(stored.LEntryId, engine.TEngineEntryLoad(stored.LEntryId)!);
        long? after = engine.TEngineRevisionRead();

        Assert.NotNull(before);
        Assert.NotNull(after);
        Assert.Equal(before, after);
    }

    [Fact]
    public void RequestApply_TagPickOnCardZero_LandsOnFirstMeaning()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LTag tag = engine.TEngineTagCreate("literal");
        LDraft started = engine.TEngineDraftStart("Input", null);

        LDraft held = engine.TEngineRequestApply(TInterface.TTagPickCreate(started.LDraftId, 0, tag.LTagId, 0));

        LCardDraft first = Assert.Single(held.LDraftContent.LEntryDraftMeanings);
        Assert.Equal(tag.LTagId, Assert.Single(first.LCardDraftTag).LTagDraftId);
    }

    [Fact]
    public void RequestApply_ExampleOnSentenceZero_LandsOnFirstRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft sentence = engine.TEngineExampleStart("Corpus", null);
        engine.TEngineRequestApply(TInterface.TExampleLanguageCreate(sentence.LDraftId, "English"));
        engine.TEngineRequestApply(
            TInterface.TExampleTextCreate(
                sentence.LDraftId, TInterface.TStateValueCreate("she knelt to kindle the logs")));
        LExample example = engine.TEngineExampleCommit(sentence.LDraftId);

        LDraft started = engine.TEngineDraftStart("Input", null);
        LDraft held = engine.TEngineRequestApply(
            TInterface.TSentenceExampleCreate(started.LDraftId, 0, 0, example.LExampleId));

        LCardDraft first = Assert.Single(held.LDraftContent.LEntryDraftMeanings);
        LSentenceDraft row = Assert.Single(first.LCardDraftSentence);
        Assert.Equal(example.LExampleId, row.LSentenceDraftExample?.LExampleDraftId);
    }

    [Fact]
    public void ExampleCommit_FreshSentence_RecordsRevision()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft sentence = engine.TEngineExampleStart("Corpus", null);
        engine.TEngineRequestApply(TInterface.TExampleLanguageCreate(sentence.LDraftId, "English"));
        engine.TEngineRequestApply(
            TInterface.TExampleTextCreate(
                sentence.LDraftId, TInterface.TStateValueCreate("she knelt to kindle the logs")));
        LExample example = engine.TEngineExampleCommit(sentence.LDraftId);

        long? revision = engine.TEngineRevisionRead();

        Assert.NotNull(revision);
        LRevisionChange change = Assert.Single(workspace.TRevisionChangeRead(revision.Value));
        Assert.Equal("example", change.LRevisionChangeSubject);
        Assert.Equal("create", change.LRevisionChangeKind);
        Assert.Equal(example.LExampleId, change.LRevisionChangeTarget);
    }

    private static LEntryDraft TDraftContentCreate(string headword)
    {
        LCardDraft meaning = TInterface.TCardDraftCreate(
            TInterface.TStateValueCreate("set alight"),
            LStateValue.LStateValueUnspecified,
            TInterface.TStateValueCreate("to set something burning"),
            [],
            [],
            [],
            [],
            [],
            1);

        return TInterface.TEntryDraftCreate(headword, "English", string.Empty, string.Empty, [meaning], []);
    }
}
