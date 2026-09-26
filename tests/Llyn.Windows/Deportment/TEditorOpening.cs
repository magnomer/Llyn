using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorOpening
{
    [Fact]
    public void Open_StoredEntry_FillsDesk()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorFixture.TEntryPrepare(engine);
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "library");

        editor.TEditorOpen(entry.LEntryId);

        Assert.True(editor.LEditorHeld);
        Assert.Equal(entry.LEntryId, editor.LEditorEntry);
        Assert.Equal("water", editor.TEditorDraftRead()?.LEntryDraftHeadword);
        Assert.Equal("English", editor.LEditorLanguage);
        Assert.False(editor.LEditorOwned);
    }

    [Fact]
    public void Open_Null_ResetsToFreshDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorFixture.TEntryPrepare(engine);
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "input");
        editor.TEditorOpen(entry.LEntryId);

        editor.TEditorOpen(null);

        Assert.True(editor.LEditorHeld);
        Assert.Null(editor.LEditorEntry);
        Assert.Equal(string.Empty, editor.TEditorDraftRead()?.LEntryDraftHeadword);
        Assert.True(editor.LEditorOwned);
    }

    [Fact]
    public void Open_FreshDraft_PreparesOneCardOfEachKind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "input");

        editor.TEditorOpen(null);

        LEntryDraft? draft = editor.TEditorDraftRead();
        Assert.NotNull(draft);
        Assert.Single(draft.LEntryDraftMeanings);
        Assert.Single(draft.LEntryDraftCollocations);
        Assert.Single(draft.LEntryDraftMeanings[0].LCardDraftSentence);
    }

    [Fact]
    public void Store_FreshOwnedDraft_SavesAndOpensBlank()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "input");
        editor.TEditorOpen(null);
        editor.TEditorHeadwordSet("salt");

        editor.TEditorSave();

        Assert.Contains(engine.TEngineEntryFind("salt"), row => row.LEntryHeadword == "salt");
        Assert.True(editor.LEditorHeld);
        Assert.Null(editor.LEditorEntry);
    }

    [Fact]
    public void Store_StoredEntry_ReopensIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorFixture.TEntryPrepare(engine);
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "library");
        editor.TEditorOpen(entry.LEntryId);
        editor.TEditorHeadwordSet("waters");

        editor.TEditorSave();

        Assert.Equal(entry.LEntryId, editor.LEditorEntry);
        Assert.Equal("waters", editor.TEditorDraftRead()?.LEntryDraftHeadword);
        Assert.False(editor.LEditorStorable);
    }

    [Fact]
    public void Finish_Storing_SavesAndReopens()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "input");
        editor.TEditorOpen(null);
        editor.TEditorHeadwordSet("salt");

        Assert.True(editor.TEditorFinish(true));

        Assert.True(editor.LEditorHeld);
        Assert.Null(editor.LEditorEntry);
        Assert.Contains(engine.TEngineEntryFind("salt"), row => row.LEntryHeadword == "salt");

        Assert.True(editor.TEditorFinish(false));

        Assert.False(editor.LEditorHeld);
        Assert.False(editor.LEditorRunning);
    }

    [Fact]
    public void Discard_StoredEntry_DropsTyping()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorFixture.TEntryPrepare(engine);
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "library");
        editor.TEditorOpen(entry.LEntryId);
        editor.TEditorHeadwordSet("waters");

        editor.TEditorReset();

        Assert.Equal(entry.LEntryId, editor.LEditorEntry);
        Assert.Equal("water", editor.TEditorDraftRead()?.LEntryDraftHeadword);
    }
}
