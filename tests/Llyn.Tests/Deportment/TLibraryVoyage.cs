using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TLibraryVoyage
{
    [Fact]
    public void VoyageRead_NothingChosen_ReportsZero()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LLibrary panel = TLibraryPrepare(engine);

        Assert.Equal(0, panel.TLibraryVoyageRead());
    }

    [Fact]
    public void VoyageRead_RowChosen_ReportsItsId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LLibrary panel = TLibraryPrepare(engine);
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));

        panel.LLibraryPanel.TPanelRowSelect(water.LEntryId);

        Assert.Equal(water.LEntryId, panel.TLibraryVoyageRead());
        Assert.Equal(["water"], panel.TLibraryRowsRead()
            .Where(row => row.LVistaRowChosen)
            .Select(row => row.LVistaRowHeadword));
    }

    [Fact]
    public void VoyageRead_ChosenDeleted_ReportsZero()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LLibrary panel = TLibraryPrepare(engine);
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        panel.LLibraryPanel.TPanelRowSelect(water.LEntryId);

        panel.LLibraryPanel.TPanelDelete();

        Assert.Equal(0, panel.TLibraryVoyageRead());
        Assert.True(panel.LLibraryIndexEmpty);
    }

    private static LLibrary TLibraryPrepare(LEngine engine)
    {
        LLibrary panel = TInterfaceDeportment.TLibraryCreate(engine, () => false, () => true, () => true);
        panel.TLibraryVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        return panel;
    }
}
