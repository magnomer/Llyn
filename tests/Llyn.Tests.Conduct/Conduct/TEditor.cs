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
        TEditorFixture editor = TEditorPrepare(engine, "library");

        editor.TEditorFixtureOpen(entry.LEntryId);

        Assert.True(editor.TEditorFixtureDesk.CDeskHeld);
        Assert.Equal(entry.LEntryId, editor.TEditorFixtureDesk.CDeskStoredRead());
        Assert.Equal("water", editor.TEditorDraftRead()?.CEntryDraftHeadword);
        Assert.Equal(
            "English", engine.TEngineDraftRead(editor.TEditorFixtureDesk.CDeskId)?.LDraftContent.LEntryDraftLanguage);
        Assert.False(editor.TEditorFixtureOwned);
        Assert.Equal("library", engine.TEngineDraftRead(editor.TEditorFixtureDesk.CDeskId)?.LDraftOrigin);
    }

    [Fact]
    public void EntryOpen_AnyDraft_AnnouncesTheShapedContent()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        TEditorFixture editor = TEditorPrepare(engine, "library");
        List<string> shown = [];
        editor.TEditorFixtureEntry.CEntryDraftChanged += draft => shown.Add(draft.CEntryDraftHeadword);

        editor.TEditorFixtureOpen(entry.LEntryId);

        Assert.Equal(["water"], shown);
    }

    [Fact]
    public void EntrySave_FreshOwnedDraft_StoresAndOpensBlank()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorPrepare(engine, "input");
        editor.TEditorFixtureOpen(null);
        editor.TEditorFixtureEntry.CEntryHeadwordSet("salt");

        editor.TEditorFixtureSave();

        Assert.Contains(engine.TEngineEntryFind("salt"), row => row.LEntryHeadword == "salt");
        Assert.True(editor.TEditorFixtureDesk.CDeskHeld);
        Assert.Null(editor.TEditorFixtureDesk.CDeskStoredRead());
    }

    [Fact]
    public void EntrySave_StoredEntry_ReopensIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        TEditorFixture editor = TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(entry.LEntryId);
        editor.TEditorFixtureEntry.CEntryHeadwordSet("waters");

        editor.TEditorFixtureSave();

        Assert.Equal(entry.LEntryId, editor.TEditorFixtureDesk.CDeskStoredRead());
        Assert.Equal("waters", editor.TEditorDraftRead()?.CEntryDraftHeadword);
        Assert.False(editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftStorable);
    }

    [Fact]
    public void EntrySave_Unchanged_StoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorPrepare(engine, "input");
        int finished = 0;
        editor.TEditorFixtureDesk.CDeskFinished += _ => finished++;
        editor.TEditorFixtureOpen(null);

        editor.TEditorFixtureSave();

        Assert.Equal(0, finished);
        Assert.True(editor.TEditorFixtureDesk.CDeskHeld);
    }

    [Fact]
    public void Finish_StoringThenDropping_SavesReopensAndThenEnds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorPrepare(engine, "input");
        editor.TEditorFixtureOpen(null);
        editor.TEditorFixtureEntry.CEntryHeadwordSet("salt");

        Assert.True(editor.TEditorFinish(true));

        Assert.True(editor.TEditorFixtureDesk.CDeskHeld);
        Assert.Null(editor.TEditorFixtureDesk.CDeskStoredRead());
        Assert.Contains(engine.TEngineEntryFind("salt"), row => row.LEntryHeadword == "salt");

        Assert.True(editor.TEditorFinish(false));

        Assert.False(editor.TEditorFixtureDesk.CDeskHeld);
        Assert.False(editor.TEditorFixtureDesk.CDeskChronicle.CDeskChronicleRunning);
    }

    [Fact]
    public void EntryUndo_StoredEntry_DropsTheTyping()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        TEditorFixture editor = TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(entry.LEntryId);
        editor.TEditorFixtureEntry.CEntryHeadwordSet("waters");

        editor.TEditorFixtureUndo();

        Assert.Equal(entry.LEntryId, editor.TEditorFixtureDesk.CDeskStoredRead());
        Assert.Equal("water", editor.TEditorDraftRead()?.CEntryDraftHeadword);
    }

    [Fact]
    public void TenureVarietySet_PrimaryReading_TagsWhateverPrimaryTheDraftHolds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        TEditorFixture editor = TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(entry.LEntryId);

        editor.TEditorFixtureDesk.TDeskVarietySet(true, 0, "British");

        Assert.Equal("British", editor.TEditorFixtureTimbre.CTimbreAccentRead().CTimbreAccentPrimary.CVarietyName);
    }

    [Fact]
    public void TenureVarietySet_BlankVarietyOrMissingRow_ChangesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorEntryPrepare(engine);
        TEditorFixture editor = TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(entry.LEntryId);

        editor.TEditorFixtureDesk.TDeskVarietySet(true, 0, string.Empty);
        editor.TEditorFixtureDesk.TDeskVarietySet(false, 0, "British");

        Assert.False(editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftAltered);
    }

    [Fact]
    public void Prepare_WhileFilling_DefersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorPrepare(engine, "input");
        bool filling = false;
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftChanged += _ =>
        {
            filling = editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftFilling;
            editor.TEditorFixtureEntry.CEntryHeadwordSet("echo");
        };

        editor.TEditorFixtureOpen(null);
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();

        Assert.True(filling);
        Assert.Equal(string.Empty, editor.TEditorDraftRead()?.CEntryDraftHeadword);
    }

    internal static LEntry TEditorEntryPrepare(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
    }

    internal static TEditorFixture TEditorPrepare(LEngine engine, string tab)
    {
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart(tab, LCatalogOrder.LCatalogOrderHeadword));
        return editor;
    }
}
