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
        editor.CEditorEntry.CEntryDraftChanged += draft => shown.Add(draft.CEntryDraftHeadword);

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
        editor.CEditorEntry.CEntryHeadwordSet("salt");

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
        editor.CEditorEntry.CEntryHeadwordSet("waters");

        editor.CEditorEntrySave();

        Assert.Equal(entry.LEntryId, editor.CEditorDesk.CDeskStoredRead());
        Assert.Equal("waters", editor.TEditorDraftRead()?.CEntryDraftHeadword);
        Assert.False(editor.CEditorDesk.CDeskDraft.CDeskDraftStorable);
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
        editor.CEditorEntry.CEntryHeadwordSet("salt");

        Assert.True(editor.TEditorFinish(true));

        Assert.True(editor.CEditorDesk.CDeskHeld);
        Assert.Null(editor.CEditorDesk.CDeskStoredRead());
        Assert.Contains(engine.TEngineEntryFind("salt"), row => row.LEntryHeadword == "salt");

        Assert.True(editor.TEditorFinish(false));

        Assert.False(editor.CEditorDesk.CDeskHeld);
        Assert.False(editor.CEditorDesk.CDeskChronicle.CDeskChronicleRunning);
    }

    [Fact]
    public void EntryUndo_StoredEntry_DropsTheTyping()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        CEditor editor = TEditorPrepare(engine, "library");
        editor.CEditorEntryOpen(entry.LEntryId);
        editor.CEditorEntry.CEntryHeadwordSet("waters");

        editor.CEditorEntryUndo();

        Assert.Equal(entry.LEntryId, editor.CEditorDesk.CDeskStoredRead());
        Assert.Equal("water", editor.TEditorDraftRead()?.CEntryDraftHeadword);
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

        Assert.False(editor.CEditorDesk.CDeskDraft.CDeskDraftAltered);
    }

    [Fact]
    public void Prepare_WhileFilling_DefersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorPrepare(engine, "input");
        bool filling = false;
        editor.CEditorDesk.CDeskDraft.CDeskDraftChanged += _ =>
        {
            filling = editor.CEditorDesk.CDeskDraft.CDeskDraftFilling;
            editor.CEditorEntry.CEntryHeadwordSet("echo");
        };

        editor.CEditorEntryOpen(null);
        editor.CEditorDesk.CDeskDraft.CDeskDraftPersist();

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
