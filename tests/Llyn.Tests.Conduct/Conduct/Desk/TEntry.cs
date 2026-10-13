using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntry
{
    [Fact]
    public void HeadwordSet_ThenPersist_ChangesTheDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "input");
        editor.TEditorFixtureOpen(null);

        editor.TEditorFixtureEntry.CEntryHeadwordSet("salt");
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();

        Assert.Equal("salt", editor.TEditorDraftRead()?.CEntryDraftHeadword);
        Assert.True(editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftAltered);
        Assert.True(editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftStorable);
    }

    [Fact]
    public void NoteSet_TrailingNewlines_AreDropped()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "input");
        editor.TEditorFixtureOpen(null);

        editor.TEditorFixtureEntry.CEntryNoteSet("a note\r\n\n");
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();

        Assert.Equal("a note", editor.TEditorDraftRead()?.CEntryDraftNote);
        Assert.True(CEntry.CEntryNoteCheck("a note\r\n\n", editor.TEditorDraftRead()!.CEntryDraftNote));
        Assert.False(CEntry.CEntryNoteCheck("a note, longer", editor.TEditorDraftRead()!.CEntryDraftNote));
    }

    [Fact]
    public void LanguageSet_Empty_KeepsTheLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "input");
        editor.TEditorFixtureOpen(null);
        CDesk desk = editor.TEditorFixtureDesk;
        string? before = engine.TEngineDraftRead(desk.CDeskId)?.LDraftContent.LEntryDraftLanguage;

        editor.TEditorFixtureEntry.CEntryLanguageSet(string.Empty);

        Assert.Equal(
            before, engine.TEngineDraftRead(desk.CDeskId)?.LDraftContent.LEntryDraftLanguage);
        Assert.False(desk.CDeskDraft.CDeskDraftStorable);
    }

    [Fact]
    public void PronunciationSet_UnrespelledLanguage_WritesThePhoneticReading()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditor.TEditorEntryPrepare(engine);
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(entry.LEntryId);

        editor.TEditorFixtureEntry.CEntryPronunciationSet("ˈwɒtə");
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();

        Assert.Equal("ˈwɒtə", editor.TEditorFixtureEntry.CEntryPronunciationRead());
        Assert.Equal(
            "ˈwɒtə",
            editor.TEditorFixtureDesk.TDeskRead()?.LDraftContent.LEntryDraftPronunciation?.LPronunciationDraftIpa);
    }

    [Fact]
    public void PronunciationRead_EmptyDesk_ReadsEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEntry entry = TEditor.TEditorPrepare(engine, "input").TEditorFixtureEntry;

        Assert.Equal(string.Empty, entry.CEntryPronunciationRead());
        Assert.Empty(entry.CEntryEtymonRead());
    }

    [Fact]
    public void EtymonRead_AddedSource_ListsItsHeadword()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long cat = engine.TEngineTranslationCreate("cat", "English").LEntryId;
        TEditorFixture editor = TEditor.TEditorPrepare(engine, "input");
        editor.TEditorFixtureOpen(null);

        editor.TEditorFixtureCard.CCardEtymonAdd(cat);

        CTranslationTarget etymon = Assert.Single(editor.TEditorFixtureEntry.CEntryEtymonRead());
        Assert.Equal("cat", etymon.CTranslationTargetHeadword);
    }
}
