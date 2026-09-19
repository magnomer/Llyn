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
        LEntry entry = TEditorOpen.TEntryPrepare(engine);
        LEditor editor = TEditorOpen.TEditorPrepare(engine, "library");
        editor.TEditorOpen(entry.LEntryId);

        editor.TEditorGraspSet(4);

        Assert.Equal(4, editor.LEditorGrasp);
        Assert.Equal(4, engine.TEngineGraspRead(entry.LEntryId));
        Assert.NotEqual(string.Empty, editor.TEditorGraspFormat(4));
    }

    [Fact]
    public void GraspSet_FreshDraft_IgnoredAndAnnounced()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEditor editor = TEditorOpen.TEditorPrepare(engine, "input");
        editor.TEditorOpen(null);
        int announced = 0;
        editor.LEditorGraspChanged += () => announced++;

        editor.TEditorGraspSet(4);

        Assert.Equal(0, editor.LEditorGrasp);
        Assert.Equal(1, announced);
        Assert.Equal(string.Empty, editor.TEditorGraspFormat(4));
    }

    [Fact]
    public void FavoriteSet_StoredEntry_MarksAndClears()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEditorOpen.TEntryPrepare(engine);
        LEditor editor = TEditorOpen.TEditorPrepare(engine, "library");
        editor.TEditorOpen(entry.LEntryId);

        editor.TEditorFavoriteSet(true);
        Assert.True(editor.LEditorFavorite);
        Assert.True(engine.TEngineFavoriteCheck(entry.LEntryId));

        editor.TEditorFavoriteSet(false);
        Assert.False(editor.LEditorFavorite);
    }
}
