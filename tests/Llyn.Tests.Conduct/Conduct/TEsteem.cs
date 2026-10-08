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
        TEditorFixture editor = TEsteemEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(entry.LEntryId);
        CEsteem esteem = editor.TEditorFixtureEsteem;

        esteem.CEsteemGraspSet(4);

        Assert.Equal(4, esteem.CEsteemGrasp);
        Assert.Equal(4, engine.TEngineGraspRead(entry.LEntryId));
        Assert.NotEqual(string.Empty, esteem.CEsteemGraspRead(4));
    }

    [Fact]
    public void EsteemGraspSet_StandingStepPressedAgain_ClearsTheGrasp()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEsteemEntryPrepare(engine);
        TEditorFixture editor = TEsteemEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(entry.LEntryId);
        CEsteem esteem = editor.TEditorFixtureEsteem;
        esteem.CEsteemGraspSet(5);
        Assert.Equal(5, esteem.CEsteemGrasp);

        esteem.CEsteemGraspSet(5);

        Assert.Equal(0, esteem.CEsteemGrasp);
        Assert.Equal(0, engine.TEngineGraspRead(entry.LEntryId));
    }

    [Fact]
    public void EsteemGraspSet_FreshDraft_IgnoredAndAnnounced()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEsteemEditorPrepare(engine, "input");
        editor.TEditorFixtureOpen(null);
        CEsteem esteem = editor.TEditorFixtureEsteem;
        int announced = 0;
        esteem.CEsteemGraspChanged += () => announced++;

        esteem.CEsteemGraspSet(4);

        Assert.Equal(0, esteem.CEsteemGrasp);
        Assert.Equal(1, announced);
        Assert.Equal(string.Empty, esteem.CEsteemGraspRead(4));
    }

    [Fact]
    public void EsteemFavoriteSet_StoredEntry_MarksAndClears()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TEsteemEntryPrepare(engine);
        TEditorFixture editor = TEsteemEditorPrepare(engine, "library");
        editor.TEditorFixtureOpen(entry.LEntryId);
        CEsteem esteem = editor.TEditorFixtureEsteem;
        int announced = 0;
        esteem.CEsteemFavoriteChanged += () => announced++;

        esteem.CEsteemFavoriteSet(true);
        Assert.True(esteem.CEsteemFavorite);
        Assert.True(engine.TEngineFavoriteCheck(entry.LEntryId));

        esteem.CEsteemFavoriteSet(false);
        Assert.False(esteem.CEsteemFavorite);
        Assert.Equal(2, announced);
    }

    [Fact]
    public void EsteemGraspStep_AnyEditor_ReadsTheEngineLimit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture draftEditor = TEsteemEditorPrepare(engine, "input");
        draftEditor.TEditorFixtureOpen(null);
        TEditorFixture storedEditor = TEsteemEditorPrepare(engine, "library");
        storedEditor.TEditorFixtureOpen(TEsteemEntryPrepare(engine).LEntryId);

        Assert.Equal(LGrasp.LGraspStep, draftEditor.TEditorFixtureEsteem.CEsteemGraspStep);
        Assert.Equal(LGrasp.LGraspStep, storedEditor.TEditorFixtureEsteem.CEsteemGraspStep);
    }

    [Fact]
    public void EsteemFrequencyRead_FreshDraft_ReadsNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEsteemEditorPrepare(engine, "input");
        editor.TEditorFixtureOpen(null);

        Assert.Null(editor.TEditorFixtureEsteem.CEsteemFrequencyRead("once"));
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
            engine, TDisplayGrasp.TGraspPortCreate(limit, 0), TEsteemStoredPrepare(engine));

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
            engine, TDisplayGrasp.TGraspPortCreate(limit, stored), TEsteemStoredPrepare(engine));

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

    private static TEditorFixture TEsteemEditorPrepare(LEngine engine, string tab)
    {
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart(tab, LCatalogOrder.LCatalogOrderHeadword));
        return editor;
    }
}
