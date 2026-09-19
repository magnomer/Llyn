using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorField
{
    [Fact]
    public void HeadwordSet_ThenSave_ChangesDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEditor editor = TEditorOpen.TEditorPrepare(engine, "input");
        editor.TEditorOpen(null);

        editor.TEditorHeadwordSet("salt");
        editor.TEditorPersist();

        Assert.Equal("salt", editor.TEditorDraftRead()?.LEntryDraftHeadword);
        Assert.True(editor.LEditorChanged);
        Assert.True(editor.LEditorStorable);
    }

    [Fact]
    public void NoteSet_TrailingNewlines_AreDropped()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEditor editor = TEditorOpen.TEditorPrepare(engine, "input");
        editor.TEditorOpen(null);

        editor.TEditorNoteSet("a note\r\n\n");
        editor.TEditorPersist();

        Assert.Equal("a note", editor.TEditorDraftRead()?.LEntryDraftNote);
    }

    [Fact]
    public void LanguageSet_Empty_KeepsLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEditor editor = TEditorOpen.TEditorPrepare(engine, "input");
        editor.TEditorOpen(null);
        string before = editor.LEditorLanguage;

        editor.TEditorLanguageSet(string.Empty);

        Assert.Equal(before, editor.LEditorLanguage);
        Assert.False(editor.LEditorStorable);
    }

    [Fact]
    public void Prepare_WhileFilling_DefersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEditor editor = TEditorOpen.TEditorPrepare(engine, "input");
        bool filling = false;
        editor.LEditorDesk.LDeskDraftChanged += _ =>
        {
            filling = editor.LEditorDesk.LDeskFilling;
            editor.TEditorHeadwordSet("echo");
        };

        editor.TEditorOpen(null);
        editor.TEditorPersist();

        Assert.True(filling);
        Assert.Equal(string.Empty, editor.TEditorDraftRead()?.LEntryDraftHeadword);
    }
}
