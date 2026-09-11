using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TWorkspaceState
{
    [Fact]
    public void OrderSave_EveryBrowsePanel_KeepsEachOrderingApart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineOrderSave(LCatalogOrder.LCatalogOrderRecent);
        engine.TEngineSequenceSave(LCatalogOrder.LCatalogOrderSound);
        engine.TEngineSeriesSave(LCatalogOrder.LCatalogOrderMarked);
        engine.TEngineFunnelSave(LCatalogOrder.LCatalogOrderUsage);
        engine.TEngineTierSave(LCatalogOrder.LCatalogOrderKind);
        engine.TEngineGradeSave(LCatalogOrder.LCatalogOrderYear);
        engine.TEngineRankSave(LCatalogOrder.LCatalogOrderLanguage);

        LWorkspaceState state = engine.TEngineStateRead();

        Assert.Equal(LCatalogOrder.LCatalogOrderRecent, state.LWorkspaceStateOrder);
        Assert.Equal(LCatalogOrder.LCatalogOrderSound, state.LWorkspaceStateSequence);
        Assert.Equal(LCatalogOrder.LCatalogOrderMarked, state.LWorkspaceStateSeries);
        Assert.Equal(LCatalogOrder.LCatalogOrderUsage, state.LWorkspaceStateFunnel);
        Assert.Equal(LCatalogOrder.LCatalogOrderKind, state.LWorkspaceStateTier);
        Assert.Equal(LCatalogOrder.LCatalogOrderYear, state.LWorkspaceStateGrade);
        Assert.Equal(LCatalogOrder.LCatalogOrderLanguage, state.LWorkspaceStateRank);
    }

    [Fact]
    public void OrderSave_WorkspaceReopened_KeepsChosenOrdering()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            engine.TEngineOrderSave(LCatalogOrder.LCatalogOrderReverse);
            engine.TEngineModeSave("Corpus");
            engine.TEngineSplitSave(true);
        }

        using LEngine reopened = workspace.TWorkspaceEngineStart();
        LWorkspaceState state = reopened.TEngineStateRead();

        Assert.Equal(LCatalogOrder.LCatalogOrderReverse, state.LWorkspaceStateOrder);
        Assert.Equal("Corpus", state.LWorkspaceStateMode);
        Assert.True(state.LWorkspaceStateSplit);
    }

    [Fact]
    public void ModeSave_AfterOrderSave_KeepsBothChanges()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineOrderSave(LCatalogOrder.LCatalogOrderEarliest);
        engine.TEngineModeSave("Library");

        LWorkspaceState state = engine.TEngineStateRead();

        Assert.Equal(LCatalogOrder.LCatalogOrderEarliest, state.LWorkspaceStateOrder);
        Assert.Equal("Library", state.LWorkspaceStateMode);
    }

    [Fact]
    public void SplitSave_ReadArea_ReadsBackAsReadArea()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineSplitSave(true);
        engine.TEngineSplitSave(false);

        Assert.False(engine.TEngineStateRead().LWorkspaceStateSplit);
    }

    [Fact]
    public void LeftSave_BothDuplexSides_KeepsEachSideApart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry left = engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("left", "English", string.Empty, string.Empty, [], []));
        LEntry right = engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("right", "English", string.Empty, string.Empty, [], []));

        engine.TEngineLeftSave(left.LEntryId);
        engine.TEngineRightSave(right.LEntryId);

        LWorkspaceState state = engine.TEngineStateRead();

        Assert.Equal(left.LEntryId, state.LWorkspaceStateLeft);
        Assert.Equal(right.LEntryId, state.LWorkspaceStateRight);
    }

    [Fact]
    public void LeftSave_EntryDeleted_EmptiesThatSide()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("word", "English", string.Empty, string.Empty, [], []));

        engine.TEngineLeftSave(entry.LEntryId);
        engine.TEngineEntryDelete(entry.LEntryId);

        Assert.Null(engine.TEngineStateRead().LWorkspaceStateLeft);
    }
}
