using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditor
{
    private const string TEditorClipPack =
        """
        { "varieties": { "list": [ { "name": "British" } ] },
          "audio": [
            { "name": "Tagged", "attempts": [ { "urls": ["https://example.test/{word}"],
                "strategy": "regex", "match": "uk=(\\S+)", "group": 1 } ] } ] }
        """;

    [Fact]
    public void EntryOpen_StoredEntry_HoldsItOnTheDesk()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        CEditor editor = TEditorPrepare(engine, "library");

        editor.CEditorEntryOpen(entry.LEntryId);

        Assert.True(editor.CEditorDesk.CDeskHeld);
        Assert.Equal(entry.LEntryId, editor.CEditorDesk.CDeskStoredRead());
        Assert.Equal("water", editor.CEditorDraftRead()?.CEntryDraftHeadword);
        Assert.Equal("English", editor.CEditorDraftRead()?.CEntryDraftLanguage);
        Assert.False(editor.CEditorOwned);
        Assert.Equal("library", editor.CEditorOrigin);
    }

    [Fact]
    public void EntryOpen_Null_ResetsToAFreshDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        CEditor editor = TEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(entry.LEntryId);

        editor.CEditorEntryOpen(null);

        Assert.True(editor.CEditorDesk.CDeskHeld);
        Assert.Null(editor.CEditorDesk.CDeskStoredRead());
        Assert.Equal(string.Empty, editor.CEditorDraftRead()?.CEntryDraftHeadword);
        Assert.True(editor.CEditorOwned);
    }

    [Fact]
    public void EntryOpen_MissingEntry_FallsBackToAFreshDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorPrepare(engine, "library");

        editor.CEditorEntryOpen(987654);

        Assert.True(editor.CEditorDesk.CDeskHeld);
        Assert.Null(editor.CEditorDesk.CDeskStoredRead());
    }

    [Fact]
    public void EntryOpen_FreshDraft_PreparesOneCardOfEachKind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorPrepare(engine, "input");

        editor.CEditorEntryOpen(null);

        CEntryDraft? draft = editor.CEditorDraftRead();
        Assert.NotNull(draft);
        Assert.Single(draft.CEntryDraftMeanings);
        Assert.Single(draft.CEntryDraftCollocations);
        Assert.Single(draft.CEntryDraftMeanings[0].CCardDraftSentence);
    }

    [Fact]
    public void EntryOpen_AnyDraft_AnnouncesTheShapedContent()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        CEditor editor = TEditorPrepare(engine, "library");
        List<string> shown = [];
        editor.CEditorDraftChanged += draft => shown.Add(draft.CEntryDraftHeadword);

        editor.CEditorEntryOpen(entry.LEntryId);

        Assert.Equal(["water"], shown);
    }

    [Fact]
    public void EntrySave_FreshOwnedDraft_StoresAndOpensBlank()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);
        editor.CEditorHeadwordSet("salt");

        editor.CEditorEntrySave();

        Assert.Contains(engine.TEngineEntryFind("salt"), row => row.LEntryHeadword == "salt");
        Assert.True(editor.CEditorDesk.CDeskHeld);
        Assert.Null(editor.CEditorDesk.CDeskStoredRead());
    }

    [Fact]
    public void EntrySave_StoredEntry_ReopensIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        CEditor editor = TEditorPrepare(engine, "library");
        editor.CEditorEntryOpen(entry.LEntryId);
        editor.CEditorHeadwordSet("waters");

        editor.CEditorEntrySave();

        Assert.Equal(entry.LEntryId, editor.CEditorDesk.CDeskStoredRead());
        Assert.Equal("waters", editor.CEditorDraftRead()?.CEntryDraftHeadword);
        Assert.False(editor.CEditorDesk.CDeskStorable);
    }

    [Fact]
    public void EntrySave_Unchanged_StoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorPrepare(engine, "input");
        int finished = 0;
        editor.CEditorDesk.CDeskFinished += _ => finished++;
        editor.CEditorEntryOpen(null);

        editor.CEditorEntrySave();

        Assert.Equal(0, finished);
        Assert.True(editor.CEditorDesk.CDeskHeld);
    }

    [Fact]
    public void Finish_StoringThenDropping_SavesReopensAndThenEnds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);
        editor.CEditorHeadwordSet("salt");

        Assert.True(editor.CEditorFinish(true));

        Assert.True(editor.CEditorDesk.CDeskHeld);
        Assert.Null(editor.CEditorDesk.CDeskStoredRead());
        Assert.Contains(engine.TEngineEntryFind("salt"), row => row.LEntryHeadword == "salt");

        Assert.True(editor.CEditorFinish(false));

        Assert.False(editor.CEditorDesk.CDeskHeld);
        Assert.False(editor.CEditorDesk.CDeskRunning);
    }

    [Fact]
    public void EntryUndo_StoredEntry_DropsTheTyping()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        CEditor editor = TEditorPrepare(engine, "library");
        editor.CEditorEntryOpen(entry.LEntryId);
        editor.CEditorHeadwordSet("waters");

        editor.CEditorEntryUndo();

        Assert.Equal(entry.LEntryId, editor.CEditorDesk.CDeskStoredRead());
        Assert.Equal("water", editor.CEditorDraftRead()?.CEntryDraftHeadword);
    }

    [Fact]
    public void HeadwordSet_ThenPersist_ChangesTheDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);

        editor.CEditorHeadwordSet("salt");
        editor.CEditorDesk.CDeskPersist();

        Assert.Equal("salt", editor.CEditorDraftRead()?.CEntryDraftHeadword);
        Assert.True(editor.CEditorDesk.CDeskChanged);
        Assert.True(editor.CEditorDesk.CDeskStorable);
    }

    [Fact]
    public void NoteSet_TrailingNewlines_AreDropped()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);

        editor.CEditorNoteSet("a note\r\n\n");
        editor.CEditorDesk.CDeskPersist();

        Assert.Equal("a note", editor.CEditorDraftRead()?.CEntryDraftNote);
    }

    [Fact]
    public void LanguageSet_Empty_KeepsTheLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);
        string? before = editor.CEditorDraftRead()?.CEntryDraftLanguage;

        editor.CEditorLanguageSet(string.Empty);

        Assert.Equal(before, editor.CEditorDraftRead()?.CEntryDraftLanguage);
        Assert.False(editor.CEditorDesk.CDeskStorable);
    }

    [Fact]
    public void PronunciationSet_UnrespelledLanguage_WritesThePhoneticReading()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        CEditor editor = TEditorPrepare(engine, "library");
        editor.CEditorEntryOpen(entry.LEntryId);

        editor.CEditorPronunciationSet("ˈwɒtə");
        editor.CEditorDesk.CDeskPersist();

        Assert.Equal("ˈwɒtə", editor.CEditorPronunciationRead());
        Assert.Equal("ˈwɒtə", editor.CEditorDraftRead()?.CEntryDraftPronunciation?.CPronunciationDraftIpa);
    }

    [Fact]
    public void VarietySet_PrimaryReading_TagsWhateverPrimaryTheDraftHolds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        CEditor editor = TEditorPrepare(engine, "library");
        editor.CEditorEntryOpen(entry.LEntryId);

        editor.CEditorVarietySet(true, 0, "British");

        Assert.Equal("British", editor.CEditorDraftRead()?.CEntryDraftPronunciation?.CPronunciationDraftVariety);
    }

    [Fact]
    public void VarietySet_BlankVarietyOrMissingRow_ChangesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        CEditor editor = TEditorPrepare(engine, "library");
        editor.CEditorEntryOpen(entry.LEntryId);

        editor.CEditorVarietySet(true, 0, string.Empty);
        editor.CEditorVarietySet(false, 0, "British");

        Assert.False(editor.CEditorDesk.CDeskChanged);
    }

    [Fact]
    public void PronunciationRead_EmptyDesk_ReadsEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorPrepare(engine, "input");

        Assert.Equal(string.Empty, editor.CEditorPronunciationRead());
        Assert.Empty(editor.CEditorEtymonRead());
        Assert.Empty(editor.CEditorTargetRead());
    }

    [Fact]
    public void EtymonRead_AddedSource_ListsItsHeadword()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long cat = engine.TEngineTranslationCreate("cat", "English").LEntryId;
        CEditor editor = TEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);

        editor.CEditorCard.CCardEtymonAdd(cat);

        CTranslationTarget etymon = Assert.Single(editor.CEditorEtymonRead());
        Assert.Equal("cat", etymon.CTranslationTargetHeadword);
    }

    [Fact]
    public void Prepare_WhileFilling_DefersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorPrepare(engine, "input");
        bool filling = false;
        editor.CEditorDesk.CDeskDraftChanged += _ =>
        {
            filling = editor.CEditorDesk.CDeskFilling;
            editor.CEditorHeadwordSet("echo");
        };

        editor.CEditorEntryOpen(null);
        editor.CEditorDesk.CDeskPersist();

        Assert.True(filling);
        Assert.Equal(string.Empty, editor.CEditorDraftRead()?.CEntryDraftHeadword);
    }

    [Fact]
    public void MembershipStart_TagGiven_OpensAFreshDraftCarryingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("botany");
        CEditor editor = TEditorPrepare(engine, "membership");
        editor.CEditorEntryOpen(null);
        long replaced = editor.CEditorDesk.CDeskId;

        editor.CEditorDesk.TDeskMembershipStart(tag.LTagId);

        Assert.Contains(
            editor.CEditorDraftRead()?.CEntryDraftMeanings[0].CCardDraftTag ?? [],
            row => row.CTagDraftId == tag.LTagId);
        Assert.NotEqual(replaced, editor.CEditorDesk.CDeskId);
        Assert.Null(engine.TEngineDraftRead(replaced));
    }

    [Fact]
    public void RegisterAdd_FreshDraft_LinksTheFirstCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LRegister register = engine.TEngineRegisterCreate("formal");
        CEditor editor = TEditorPrepare(engine, "cohort");
        editor.CEditorEntryOpen(null);

        editor.CEditorRegisterAdd(register.LRegisterId);

        Assert.Contains(
            editor.CEditorDraftRead()?.CEntryDraftMeanings[0].CCardDraftRegister ?? [],
            row => row.CRegisterDraftId == register.LRegisterId);
    }

    [Fact]
    public void OccurrenceStart_SituationGiven_OpensAFreshDraftAlreadyLinked()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LStateValue text = TInterface.TStateValueCreate("at the market");
        LSituation situation = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, text, text, text));
        CEditor editor = TEditorPrepare(engine, "occurrence");
        List<CEntryDraft> shown = [];
        editor.CEditorDraftChanged += shown.Add;

        editor.CEditorDesk.TDeskOccurrenceStart(situation.LSituationId);

        Assert.True(editor.CEditorDesk.CDeskHeld);
        Assert.False(editor.CEditorDesk.CDeskStored);
        Assert.Contains(
            Assert.Single(shown).CEntryDraftMeanings[0].CCardDraftSituation,
            row => row.CSituationDraftId == situation.LSituationId);
    }

    [Fact]
    public void OccurrenceStart_NoSituation_OpensABlankDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorPrepare(engine, "occurrence");

        editor.CEditorDesk.TDeskOccurrenceStart(null);

        Assert.True(editor.CEditorDesk.CDeskHeld);
        Assert.False(editor.CEditorDesk.CDeskChanged);
    }

    [Fact]
    public void QuotationStart_ExampleGiven_OpensAFreshDraftCitingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LExample example = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0, "English", TInterface.TStateValueCreate("Water is wet."), null, LStateAnchor.LStateAnchorUnspecified));
        CEditor editor = TEditorPrepare(engine, "quotation");
        editor.CEditorEntryOpen(null);
        long replaced = editor.CEditorDesk.CDeskId;

        editor.CEditorDesk.TDeskQuotationStart(example.LExampleId);

        CExampleDraft? cited =
            editor.CEditorDraftRead()?.CEntryDraftMeanings[0].CCardDraftSentence[0].CSentenceDraftExample;
        Assert.Equal("Water is wet.", cited?.CExampleDraftText.CStateValueText);
        Assert.NotEqual(replaced, editor.CEditorDesk.CDeskId);
        Assert.Null(engine.TEngineDraftRead(replaced));
    }

    [Fact]
    public void FootnoteStart_SourceGiven_OpensAFreshDraftCitingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        CEditor editor = TEditorPrepare(engine, "footnote");
        editor.CEditorEntryOpen(null);

        editor.CEditorDesk.TDeskFootnoteStart(book.LReferenceId);

        CExampleDraft? cited =
            editor.CEditorDraftRead()?.CEntryDraftMeanings[0].CCardDraftSentence[0].CSentenceDraftExample;
        Assert.Equal(book.LReferenceId, cited?.CExampleDraftReference);
    }

    [Fact]
    public async Task RecordingStart_HeldDraft_DeliversEveryStepToTheSink()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEditorClipPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=https://example.test/gb.mp3", Task.CompletedTask));
        engine.TEngineDelaySet(0);
        CEditor editor = TEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);
        editor.CEditorLanguageSet(pack.TLanguageFixtureName);
        editor.CEditorHeadwordSet("tomato");
        List<CHarvestStep> steps = [];
        TaskCompletionSource finished = new(TaskCreationOptions.RunContinuationsAsynchronously);

        editor.CEditorDesk.CDeskErrand.CErrandRecordingStart("tomato", 0, step =>
        {
            lock (steps)
            {
                steps.Add(step);
            }

            if (step.CHarvestStepEnded)
            {
                finished.TrySetResult();
            }
        });
        await finished.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(
            [(false, false), (true, false), (false, true)],
            steps.Select(step => (step.CHarvestStepRecording is not null, step.CHarvestStepEnded)));
        Assert.Equal("Tagged", steps[0].CHarvestStepSource);
        Assert.Equal("https://example.test/gb.mp3", steps[1].CHarvestStepRecording?.CRecordingAddress);
        editor.CEditorFinish(false);
    }

    private static LEntry TEditorEntryPrepare(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
    }

    private static CEditor TEditorPrepare(LEngine engine, string tab)
    {
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart(tab, LCatalogOrder.LCatalogOrderHeadword));
        return editor;
    }
}
