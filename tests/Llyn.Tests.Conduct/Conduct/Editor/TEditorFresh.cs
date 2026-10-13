using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorFresh
{
    [Fact]
    public void EntryOpen_Null_ResetsToAFreshDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditor.TEditorEntryPrepare(engine);
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "input");
        editor.TEditorFixtureOpen(entry.LEntryId);

        editor.TEditorFixtureOpen(null);

        Assert.True(editor.TEditorFixtureDesk.CDeskHeld);
        Assert.Null(editor.TEditorFixtureDesk.CDeskStoredRead());
        Assert.Equal(string.Empty, editor.TEditorDraftRead()?.CEntryDraftHeadword);
        Assert.True(editor.TEditorFixtureOwned);
    }

    [Fact]
    public void EntryOpen_MissingEntry_FallsBackToAFreshDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");

        editor.TEditorFixtureOpen(987654);

        Assert.True(editor.TEditorFixtureDesk.CDeskHeld);
        Assert.Null(editor.TEditorFixtureDesk.CDeskStoredRead());
    }

    [Fact]
    public void EntryOpen_FreshDraft_PreparesOneCardOfEachKind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "input");

        editor.TEditorFixtureOpen(null);

        CEntryDraft? draft = editor.TEditorDraftRead();
        Assert.NotNull(draft);
        Assert.Single(draft.CEntryDraftMeanings);
        Assert.Single(draft.CEntryDraftCollocations);
        Assert.Single(draft.CEntryDraftMeanings[0].CCardDraftSentence);
    }

    [Fact]
    public void MembershipStart_TagGiven_OpensAFreshDraftCarryingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("botany");
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "membership");
        editor.TEditorFixtureOpen(null);
        long replaced = editor.TEditorFixtureDesk.CDeskId;

        editor.TEditorFixtureDesk.TDeskMembershipStart(tag.LTagId);

        Assert.Contains(
            editor.TEditorDraftRead()?.CEntryDraftMeanings[0].CCardDraftTag ?? [],
            row => row.CTagDraftId == tag.LTagId);
        Assert.NotEqual(replaced, editor.TEditorFixtureDesk.CDeskId);
        Assert.Null(engine.TEngineDraftRead(replaced));
    }

    [Fact]
    public void CohortStart_RegisterGiven_OpensAFreshDraftCarryingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LRegister register = engine.TEngineRegisterCreate("formal");
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "cohort");
        editor.TEditorFixtureOpen(null);
        long replaced = editor.TEditorFixtureDesk.CDeskId;

        editor.TEditorFixtureDesk.TDeskCohortStart(register.LRegisterId);

        Assert.Contains(
            editor.TEditorDraftRead()?.CEntryDraftMeanings[0].CCardDraftRegister ?? [],
            row => row.CRegisterDraftId == register.LRegisterId);
        Assert.NotEqual(replaced, editor.TEditorFixtureDesk.CDeskId);
        Assert.Null(engine.TEngineDraftRead(replaced));
    }

    [Fact]
    public void OccurrenceStart_SituationGiven_OpensAFreshDraftAlreadyLinked()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LStateValue text = TInterfaceState.TStateValueCreate("at the market");
        LSituation situation = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, text, text, text));
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "occurrence");
        List<CEntryDraft> shown = [];
        editor.TEditorFixtureEntry.CEntryDraftChanged += shown.Add;

        editor.TEditorFixtureDesk.TDeskOccurrenceStart(situation.LSituationId);

        Assert.True(editor.TEditorFixtureDesk.CDeskHeld);
        Assert.False(editor.TEditorFixtureDesk.CDeskStored);
        Assert.Contains(
            Assert.Single(shown).CEntryDraftMeanings[0].CCardDraftSituation,
            row => row.CSituationDraftId == situation.LSituationId);
    }

    [Fact]
    public void OccurrenceStart_NoSituation_OpensABlankDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "occurrence");

        editor.TEditorFixtureDesk.TDeskOccurrenceStart(null);

        Assert.True(editor.TEditorFixtureDesk.CDeskHeld);
        Assert.False(editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftAltered);
    }

    [Fact]
    public void QuotationStart_ExampleGiven_OpensAFreshDraftCitingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LExample example = engine.TEngineExampleCreate(TInterfaceExample.TExampleCreate(
            0,
            "English",
            TInterfaceState.TStateValueCreate("Water is wet."),
            null,
            LStateAnchor.LStateAnchorUnspecified));
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "quotation");
        editor.TEditorFixtureOpen(null);
        long replaced = editor.TEditorFixtureDesk.CDeskId;

        editor.TEditorFixtureDesk.TDeskQuotationStart(example.LExampleId);

        CExampleDraft? cited =
            editor.TEditorDraftRead()?.CEntryDraftMeanings[0].CCardDraftSentence[0].CSentenceDraftExample;
        Assert.Equal("Water is wet.", cited?.CExampleDraftText.CStateValueText);
        Assert.NotEqual(replaced, editor.TEditorFixtureDesk.CDeskId);
        Assert.Null(engine.TEngineDraftRead(replaced));
    }

    [Fact]
    public void FootnoteStart_SourceGiven_OpensAFreshDraftCitingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "footnote");
        editor.TEditorFixtureOpen(null);

        editor.TEditorFixtureDesk.TDeskFootnoteStart(book.LReferenceId);

        CExampleDraft? cited =
            editor.TEditorDraftRead()?.CEntryDraftMeanings[0].CCardDraftSentence[0].CSentenceDraftExample;
        Assert.Equal(book.LReferenceId, cited?.CExampleDraftReference);
    }
}
