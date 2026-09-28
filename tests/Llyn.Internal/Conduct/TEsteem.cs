using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEsteem
{
    [Fact]
    public void EsteemGraspSet_StoredEntry_WritesAndReadsBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEsteemEntryPrepare(engine);
        CEditor editor = TEsteemEditorPrepare(engine, "library");
        editor.CEditorEntryOpen(entry.LEntryId);

        editor.CEditorEsteem.CEsteemGraspSet(4);

        Assert.Equal(4, editor.CEditorEsteem.CEsteemGrasp);
        Assert.Equal(4, engine.TEngineGraspRead(entry.LEntryId));
        Assert.NotEqual(string.Empty, editor.CEditorEsteem.CEsteemGraspRead(4));
    }

    [Fact]
    public void EsteemGraspSet_FreshDraft_IgnoredAndAnnounced()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEsteemEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);
        int announced = 0;
        editor.CEditorEsteem.CEsteemGraspChanged += () => announced++;

        editor.CEditorEsteem.CEsteemGraspSet(4);

        Assert.Equal(0, editor.CEditorEsteem.CEsteemGrasp);
        Assert.Equal(1, announced);
        Assert.Equal(string.Empty, editor.CEditorEsteem.CEsteemGraspRead(4));
    }

    [Fact]
    public void EsteemFavoriteSet_StoredEntry_MarksAndClears()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEsteemEntryPrepare(engine);
        CEditor editor = TEsteemEditorPrepare(engine, "library");
        editor.CEditorEntryOpen(entry.LEntryId);
        int announced = 0;
        editor.CEditorEsteem.CEsteemFavoriteChanged += () => announced++;

        editor.CEditorEsteem.CEsteemFavoriteSet(true);
        Assert.True(editor.CEditorEsteem.CEsteemFavorite);
        Assert.True(engine.TEngineFavoriteCheck(entry.LEntryId));

        editor.CEditorEsteem.CEsteemFavoriteSet(false);
        Assert.False(editor.CEditorEsteem.CEsteemFavorite);
        Assert.Equal(2, announced);
    }

    [Fact]
    public void EsteemGraspStep_AnyEditor_ReadsTheEngineLimit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEsteemEditorPrepare(engine, "input");

        Assert.True(editor.CEditorEsteem.CEsteemGraspStep > 0);
    }

    [Fact]
    public void EsteemFrequencyRead_FreshDraft_ReadsNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEsteemEditorPrepare(engine, "input");
        editor.CEditorEntryOpen(null);

        Assert.Null(editor.CEditorEsteem.CEsteemFrequencyRead("once"));
    }

    private static LEntry TEsteemEntryPrepare(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
    }

    private static CEditor TEsteemEditorPrepare(LEngine engine, string tab)
    {
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart(tab, LCatalogOrder.LCatalogOrderHeadword));
        return editor;
    }
}
