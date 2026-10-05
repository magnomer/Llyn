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
    public void EsteemGraspSet_StandingStepPressedAgain_ClearsTheGrasp()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEsteemEntryPrepare(engine);
        CEditor editor = TEsteemEditorPrepare(engine, "library");
        editor.CEditorEntryOpen(entry.LEntryId);
        editor.CEditorEsteem.CEsteemGraspSet(5);
        Assert.Equal(5, editor.CEditorEsteem.CEsteemGrasp);

        editor.CEditorEsteem.CEsteemGraspSet(5);

        Assert.Equal(0, editor.CEditorEsteem.CEsteemGrasp);
        Assert.Equal(0, engine.TEngineGraspRead(entry.LEntryId));
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
        CEditor draftEditor = TEsteemEditorPrepare(engine, "input");
        draftEditor.CEditorEntryOpen(null);
        CEditor storedEditor = TEsteemEditorPrepare(engine, "library");
        storedEditor.CEditorEntryOpen(TEsteemEntryPrepare(engine).LEntryId);

        Assert.Equal(LGrasp.LGraspStep, draftEditor.CEditorEsteem.CEsteemGraspStep);
        Assert.Equal(LGrasp.LGraspStep, storedEditor.CEditorEsteem.CEsteemGraspStep);
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

    [Theory]
    [InlineData(int.MinValue, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(7, 7)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void EsteemGraspStep_HostileLimit_ReadsNoneBelowZero(int limit, int read)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEsteem esteem = TInterfaceConductDesk.TEsteemCreate(
            engine, TDisplay.TGraspPortCreate(limit, 0), TEsteemStoredPrepare(engine));

        Assert.Equal(read, esteem.CEsteemGraspStep);
    }

    [Theory]
    [InlineData(5, int.MinValue, 0)]
    [InlineData(5, -1, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(5, 1, 1)]
    [InlineData(5, 3, 3)]
    [InlineData(5, 5, 5)]
    [InlineData(5, 6, 5)]
    [InlineData(5, int.MaxValue, 5)]
    [InlineData(0, 4, 0)]
    [InlineData(-3, 2, 0)]
    [InlineData(int.MinValue, int.MaxValue, 0)]
    public void EsteemGrasp_HostileStep_ClampsBetweenZeroAndLimit(int limit, int stored, int read)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEsteem esteem = TInterfaceConductDesk.TEsteemCreate(
            engine, TDisplay.TGraspPortCreate(limit, stored), TEsteemStoredPrepare(engine));

        Assert.Equal(read, esteem.CEsteemGrasp);
        Assert.InRange(esteem.CEsteemGrasp, 0, esteem.CEsteemGraspStep);
    }

    private static long TEsteemStoredPrepare(LEngine engine)
    {
        return TExemplar.TExemplarSave(engine, TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []))[0];
    }

    private static LEntry TEsteemEntryPrepare(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
    }

    private static CEditor TEsteemEditorPrepare(LEngine engine, string tab)
    {
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart(tab, LCatalogOrder.LCatalogOrderHeadword));
        return editor;
    }
}
