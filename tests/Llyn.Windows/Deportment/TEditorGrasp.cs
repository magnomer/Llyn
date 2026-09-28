using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorGrasp
{
    [Fact]
    public void GraspSet_StoredEntry_WritesAndReadsBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorFixture.TEntryPrepare(engine);
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "library");
        editor.LEditorStudio.CEditorEntryOpen(entry.LEntryId);

        editor.TEditorGraspSet(4);

        Assert.Equal(4, editor.LEditorEsteem.QEsteemGrasp);
        Assert.Equal(4, engine.TEngineGraspRead(entry.LEntryId));
        Assert.NotEqual(string.Empty, editor.TEditorGraspFormat(4));
    }

    [Fact]
    public void GraspSet_FreshDraft_IgnoredAndAnnounced()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "input");
        editor.LEditorStudio.CEditorEntryOpen(null);
        int announced = 0;
        editor.LEditorEsteem.QEsteemGraspChanged += () => announced++;

        editor.TEditorGraspSet(4);

        Assert.Equal(0, editor.LEditorEsteem.QEsteemGrasp);
        Assert.Equal(1, announced);
        Assert.Equal(string.Empty, editor.TEditorGraspFormat(4));
    }

    [Fact]
    public void FavoriteSet_StoredEntry_MarksAndClears()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorFixture.TEntryPrepare(engine);
        LEditor editor = TEditorFixture.TEditorPrepare(engine, "library");
        editor.LEditorStudio.CEditorEntryOpen(entry.LEntryId);

        editor.TEditorFavoriteSet(true);
        Assert.True(editor.LEditorEsteem.QEsteemFavorite);
        Assert.True(engine.TEngineFavoriteCheck(entry.LEntryId));

        editor.TEditorFavoriteSet(false);
        Assert.False(editor.LEditorEsteem.QEsteemFavorite);
    }
}
