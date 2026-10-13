using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineFrequencyOrder
{
    [Fact]
    public void FrequencyRead_StoredOutOfPackOrder_ListsInDeclaredOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(
            TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        TEngineFrequency.TFrequencyStoredSet(workspace, entry.LEntryId, "Zeta", "5", null);
        TEngineFrequency.TFrequencyStoredSet(workspace, entry.LEntryId, "Third", "5", null);
        TEngineFrequency.TFrequencyStoredSet(workspace, entry.LEntryId, "Alpha", "5", null);
        TEngineFrequency.TFrequencyStoredSet(workspace, entry.LEntryId, "Second", "100", null);
        TEngineFrequency.TFrequencyStoredSet(workspace, entry.LEntryId, "First", "5", null);

        IReadOnlyList<LFrequency> read = engine.TEngineFrequencyRead(entry.LEntryId);

        Assert.Equal(
            ["First", "Second", "Third", "Alpha", "Zeta"],
            read.Select(static row => row.LFrequencySource));
    }

    [Fact]
    public void FrequencyResolve_FirstSourceRefetchedLast_GaugeShowsDeclaredFirstBand()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(
            TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        TEngineFrequency.TFrequencyStoredSet(workspace, entry.LEntryId, "First", "500", null);
        TEngineFrequency.TFrequencyStoredSet(workspace, entry.LEntryId, "Second", "100", null);
        TEngineFrequency.TFrequencyStoredSet(workspace, entry.LEntryId, "First", "5", null);

        LFrequencyGauge gauge = Assert.IsType<LFrequencyGauge>(
            engine.TEngineFrequencyResolve(entry.LEntryId, "{0}"));

        Assert.Equal("Advanced", gauge.LFrequencyGaugeRank);
        Assert.StartsWith("First: ", gauge.LFrequencyGaugeSource, StringComparison.Ordinal);
        Assert.Contains("\nSecond: ", gauge.LFrequencyGaugeSource, StringComparison.Ordinal);
    }

    [Fact]
    public void FrequencySort_UndeclaredSupplementaryName_OrdersByCodepoint()
    {
        string supplementary = char.ConvertFromUtf32(0x20000);
        string wide = "Ａ";
        IReadOnlyList<LSourceSpec> declared =
            TLanguageFixture.TLanguageFixtureLoad(TEngineFrequency.TEngineFrequencyPack).LLanguageFrequencies;

        IReadOnlyList<LFrequency> sorted = TInterface.TFrequencySort(
            [
                TInterface.TFrequencyCreate(supplementary, "1", null),
                TInterface.TFrequencyCreate(wide, "1", null),
                TInterface.TFrequencyCreate("Second", "1", null),
                TInterface.TFrequencyCreate("First", "1", null),
            ],
            declared);

        Assert.Equal(
            ["First", "Second", wide, supplementary],
            sorted.Select(static row => row.LFrequencySource));
    }
}
