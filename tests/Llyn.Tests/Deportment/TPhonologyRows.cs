using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TPhonologyRows
{
    [Fact]
    public void RowsRead_QuerySet_NarrowsToMatchingEntries()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPhonology panel = TPhonologyPrepare(engine);

        panel.TPhonologyQuerySet("wat");

        Assert.Equal(
            ["water"],
            panel.TPhonologyRowsRead().Select(row => row.LCatalogPronunciationEntry.LEntryHeadword));
        Assert.False(panel.LPhonologyInventoryEmpty);
    }

    [Fact]
    public void RowsRead_QueryMatchesNothing_ReportsEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPhonology panel = TPhonologyPrepare(engine);

        panel.TPhonologyQuerySet("zzz");

        Assert.Empty(panel.TPhonologyRowsRead());
        Assert.True(panel.LPhonologyInventoryEmpty);
    }

    [Fact]
    public void RowsRead_ChosenRow_MarksThatRowAlone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LPhonology panel = TPhonologyPrepare(engine);
        LEntry fire = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "fire", "English", "faɪə", string.Empty, [TInterface.TCardCreate("a flame", 1)], []));

        panel.LPhonologyPanel.TPanelRowSelect(fire.LEntryId);

        Assert.Equal(["fire"], panel.TPhonologyRowsRead()
            .Where(row => row.LCatalogPronunciationChosen)
            .Select(row => row.LCatalogPronunciationEntry.LEntryHeadword));
    }

    private static LPhonology TPhonologyPrepare(LEngine engine)
    {
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        LPhonology panel = TInterfaceDeportment.TPhonologyCreate(engine, () => true, () => true);
        panel.TPhonologyVistaRestore(engine.TEngineVistaStart("phonology", LCatalogOrder.LCatalogOrderHeadword));
        return panel;
    }
}
