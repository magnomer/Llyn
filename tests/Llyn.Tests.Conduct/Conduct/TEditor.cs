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
        Assert.Equal("water", editor.TEditorDraftRead()?.CEntryDraftHeadword);
        Assert.Equal(
            "English", engine.TEngineDraftRead(editor.CEditorDesk.CDeskId)?.LDraftContent.LEntryDraftLanguage);
        Assert.False(editor.CEditorOwned);
        Assert.Equal("library", engine.TEngineDraftRead(editor.CEditorDesk.CDeskId)?.LDraftOrigin);
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
        Assert.Equal("waters", editor.TEditorDraftRead()?.CEntryDraftHeadword);
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

        Assert.True(editor.TEditorFinish(true));

        Assert.True(editor.CEditorDesk.CDeskHeld);
        Assert.Null(editor.CEditorDesk.CDeskStoredRead());
        Assert.Contains(engine.TEngineEntryFind("salt"), row => row.LEntryHeadword == "salt");

        Assert.True(editor.TEditorFinish(false));

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
        Assert.Equal("water", editor.TEditorDraftRead()?.CEntryDraftHeadword);
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

        Assert.Equal("salt", editor.TEditorDraftRead()?.CEntryDraftHeadword);
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

        Assert.Equal("a note", editor.TEditorDraftRead()?.CEntryDraftNote);
        Assert.True(CEditor.CEditorNoteCheck("a note\r\n\n", editor.TEditorDraftRead()!.CEntryDraftNote));
        Assert.False(CEditor.CEditorNoteCheck("a note, longer", editor.TEditorDraftRead()!.CEntryDraftNote));
    }

    [Fact]
    public void LanguageSet_Empty_KeepsTheLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);
        string? before = engine.TEngineDraftRead(editor.CEditorDesk.CDeskId)?.LDraftContent.LEntryDraftLanguage;

        editor.CEditorLanguageSet(string.Empty);

        Assert.Equal(
            before, engine.TEngineDraftRead(editor.CEditorDesk.CDeskId)?.LDraftContent.LEntryDraftLanguage);
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
        Assert.Equal(
            "ˈwɒtə", editor.CEditorDesk.TDeskRead()?.LDraftContent.LEntryDraftPronunciation?.LPronunciationDraftIpa);
    }

    [Fact]
    public void TenureVarietySet_PrimaryReading_TagsWhateverPrimaryTheDraftHolds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        CEditor editor = TEditorPrepare(engine, "library");
        editor.CEditorEntryOpen(entry.LEntryId);

        editor.CEditorDesk.TDeskVarietySet(true, 0, "British");

        Assert.Equal("British", editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentPrimary.CVarietyName);
    }

    [Fact]
    public void TenureVarietySet_BlankVarietyOrMissingRow_ChangesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        CEditor editor = TEditorPrepare(engine, "library");
        editor.CEditorEntryOpen(entry.LEntryId);

        editor.CEditorDesk.TDeskVarietySet(true, 0, string.Empty);
        editor.CEditorDesk.TDeskVarietySet(false, 0, "British");

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
        Assert.Equal(string.Empty, editor.TEditorDraftRead()?.CEntryDraftHeadword);
    }

    internal static LEntry TEditorEntryPrepare(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
    }

    internal static CEditor TEditorPrepare(LEngine engine, string tab)
    {
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart(tab, LCatalogOrder.LCatalogOrderHeadword));
        return editor;
    }
}
