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
        CEditor editor = TEditor.TEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);

        editor.CEditorEntry.CEntryHeadwordSet("salt");
        editor.CEditorDesk.CDeskDraft.CDeskDraftPersist();

        Assert.Equal("salt", editor.TEditorDraftRead()?.CEntryDraftHeadword);
        Assert.True(editor.CEditorDesk.CDeskDraft.CDeskDraftAltered);
        Assert.True(editor.CEditorDesk.CDeskDraft.CDeskDraftStorable);
    }

    [Fact]
    public void NoteSet_TrailingNewlines_AreDropped()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditor.TEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);

        editor.CEditorEntry.CEntryNoteSet("a note\r\n\n");
        editor.CEditorDesk.CDeskDraft.CDeskDraftPersist();

        Assert.Equal("a note", editor.TEditorDraftRead()?.CEntryDraftNote);
        Assert.True(CEntry.CEntryNoteCheck("a note\r\n\n", editor.TEditorDraftRead()!.CEntryDraftNote));
        Assert.False(CEntry.CEntryNoteCheck("a note, longer", editor.TEditorDraftRead()!.CEntryDraftNote));
    }

    [Fact]
    public void LanguageSet_Empty_KeepsTheLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditor.TEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);
        string? before = engine.TEngineDraftRead(editor.CEditorDesk.CDeskId)?.LDraftContent.LEntryDraftLanguage;

        editor.CEditorEntry.CEntryLanguageSet(string.Empty);

        Assert.Equal(
            before, engine.TEngineDraftRead(editor.CEditorDesk.CDeskId)?.LDraftContent.LEntryDraftLanguage);
        Assert.False(editor.CEditorDesk.CDeskDraft.CDeskDraftStorable);
    }

    [Fact]
    public void PronunciationSet_UnrespelledLanguage_WritesThePhoneticReading()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditor.TEditorEntryPrepare(engine);
        CEditor editor = TEditor.TEditorPrepare(engine, "library");
        editor.CEditorEntryOpen(entry.LEntryId);

        editor.CEditorEntry.CEntryPronunciationSet("ˈwɒtə");
        editor.CEditorDesk.CDeskDraft.CDeskDraftPersist();

        Assert.Equal("ˈwɒtə", editor.CEditorEntry.CEntryPronunciationRead());
        Assert.Equal(
            "ˈwɒtə", editor.CEditorDesk.TDeskRead()?.LDraftContent.LEntryDraftPronunciation?.LPronunciationDraftIpa);
    }

    [Fact]
    public void PronunciationRead_EmptyDesk_ReadsEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditor.TEditorPrepare(engine, "input");

        Assert.Equal(string.Empty, editor.CEditorEntry.CEntryPronunciationRead());
        Assert.Empty(editor.CEditorEntry.CEntryEtymonRead());
    }

    [Fact]
    public void EtymonRead_AddedSource_ListsItsHeadword()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long cat = engine.TEngineTranslationCreate("cat", "English").LEntryId;
        CEditor editor = TEditor.TEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);

        editor.CEditorCard.CCardEtymonAdd(cat);

        CTranslationTarget etymon = Assert.Single(editor.CEditorEntry.CEntryEtymonRead());
        Assert.Equal("cat", etymon.CTranslationTargetHeadword);
    }
}
